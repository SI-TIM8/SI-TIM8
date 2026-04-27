using System.Security.Claims;
using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LABsistem.Presentation.DTOs.Auth;
using LABsistem.Presentation.Helpers;
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

            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = korisnik.ID,
                Username = korisnik.Username,
                Role = korisnik.Uloga.ToString()
            });
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

            var usernameGreska = InputValidator.ValidirajUsername(request.Username);
            if (usernameGreska is not null)
                return BadRequest(usernameGreska);

            var passwordGreska = InputValidator.ValidirajPassword(request.Password);
            if (passwordGreska is not null)
                return BadRequest(passwordGreska);

            if (request.Password != request.PotvrdaLozinke)
                return BadRequest("Lozinke se ne poklapaju.");

            if (await _dbContext.Korisnici.AnyAsync(x => x.Username == request.Username))
                return Conflict("Username je već zauzet.");

            if (await _dbContext.Korisnici.AnyAsync(x => x.Email == request.Email))
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
    }
}


