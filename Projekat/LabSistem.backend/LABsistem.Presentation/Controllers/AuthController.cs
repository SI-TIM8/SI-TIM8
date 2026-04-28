using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
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
        private readonly IRevokedTokenStore _revokedTokenStore;

        public AuthController(
            LabSistemDbContext dbContext,
            IJwtService jwtService,
            IRevokedTokenStore revokedTokenStore)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _revokedTokenStore = revokedTokenStore;
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
                .FirstOrDefaultAsync(x => x.Username == request.Username && x.Password == request.Password);

            if (korisnik is null)
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

            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (!string.IsNullOrWhiteSpace(jti) && _revokedTokenStore.IsRevoked(jti))
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

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return BadRequest("Authorization token nije pronađen.");
            }

            var token = authorizationHeader["Bearer ".Length..].Trim();
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrWhiteSpace(jti))
            {
                return BadRequest("Token ne sadrži jti.");
            }

            _revokedTokenStore.Revoke(jti, jwtToken.ValidTo);
            return Ok(new { Message = "Odjava uspješna." });
        }
    }
}
