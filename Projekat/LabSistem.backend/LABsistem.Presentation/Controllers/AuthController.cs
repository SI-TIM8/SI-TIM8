using System.Security.Claims;
using System.Text.RegularExpressions;
using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LABsistem.Presentation.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LabSistemDbContext _dbContext;
        private readonly IJwtService _jwtService;

        public AuthController(LabSistemDbContext dbContext, IJwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username i password su obavezni.");
            }

            var korisnik = await _dbContext.Korisnici
                .FirstOrDefaultAsync(x => x.Username == request.Username);

            if (korisnik is null || !BCrypt.Net.BCrypt.Verify(request.Password, korisnik.Password))
            {
                return Unauthorized("Pogrešni kredencijali.");
            }

            var token = _jwtService.GenerateToken(
                korisnik.ID.ToString(),
                korisnik.Username,
                korisnik.Uloga.ToString());

            var response = new LoginResponseDto
            {
                Token = token,
                UserId = korisnik.ID,
                Username = korisnik.Username,
                Role = korisnik.Uloga.ToString()
            };

            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ImePrezime) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Sva polja su obavezna.");
            }

            var usernameGreska = ValidirajUsername(request.Username);
            if (usernameGreska is not null)
                return BadRequest(usernameGreska);

            var passwordGreska = ValidirajPassword(request.Password);
            if (passwordGreska is not null)
                return BadRequest(passwordGreska);

            if (request.Password != request.PotvrdaLozinke)
                return BadRequest("Lozinke se ne poklapaju.");

            var usernamePostoji = await _dbContext.Korisnici
                .AnyAsync(x => x.Username == request.Username);
            if (usernamePostoji)
                return Conflict("Username je već zauzet.");

            var emailPostoji = await _dbContext.Korisnici
                .AnyAsync(x => x.Email == request.Email);
            if (emailPostoji)
                return Conflict("Email je već registrovan.");

            var korisnik = new Korisnik
            {
                ImePrezime = request.ImePrezime,
                Email = request.Email,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Uloga = UlogaKorisnika.Student
            };

            _dbContext.Korisnici.Add(korisnik);
            await _dbContext.SaveChangesAsync();

            var token = _jwtService.GenerateToken(
                korisnik.ID.ToString(),
                korisnik.Username,
                korisnik.Uloga.ToString());

            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = korisnik.ID,
                Username = korisnik.Username,
                Role = korisnik.Uloga.ToString()
            });
        }

        [HttpGet("verify")]
        [Authorize]
        public IActionResult VerifyAuthenticatedToken()
        {
            return Ok(new
            {
                Valid = true,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = User.FindFirstValue(ClaimTypes.Name),
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpPost("verify-token")]
        [AllowAnonymous]
        public IActionResult VerifyToken([FromBody] VerifyTokenRequestDto request)
        {
            var principal = _jwtService.ValidateToken(request.Token);
            if (principal is null)
            {
                return Unauthorized(new { Valid = false });
            }

            return Ok(new
            {
                Valid = true,
                UserId = principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = principal.FindFirstValue(ClaimTypes.Name),
                Role = principal.FindFirstValue(ClaimTypes.Role)
            });
        }

        private static string? ValidirajUsername(string username)
        {
            if (username.Length < 3 || username.Length > 20)
                return "Username mora imati između 3 i 20 znakova.";

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9._]+$"))
                return "Username smije sadržavati samo slova, brojeve, tačku i donju crtu.";

            return null;
        }

        private static string? ValidirajPassword(string password)
        {
            if (password.Length < 8)
                return "Lozinka mora imati najmanje 8 znakova.";

            if (!password.Any(char.IsUpper))
                return "Lozinka mora sadržavati barem jedno veliko slovo.";

            if (!password.Any(char.IsDigit))
                return "Lozinka mora sadržavati barem jedan broj.";

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return "Lozinka mora sadržavati barem jedan specijalni znak.";

            return null;
        }
    }
}

