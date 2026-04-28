using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Services;
using LABsistem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRevokedTokenStore _revokedTokenStore;

        public AuthController(IAuthService authService, IRevokedTokenStore revokedTokenStore)
        {
            _authService = authService;
            _revokedTokenStore = revokedTokenStore;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { Message = "Pogresni kredencijali." });
            }

            return Ok(response);
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto request, [FromQuery] UlogaKorisnika uloga)
        {
            var result = await _authService.CreateUserAsync(request, uloga);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });
        }

        [HttpGet("verify")]
        [Authorize]
        public IActionResult VerifyAuthenticatedToken()
        {
            return Ok(new
            {
                Valid = true,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = User.FindFirst(ClaimTypes.Name)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        [HttpPost("verify-token")]
        [AllowAnonymous]
        public IActionResult VerifyToken([FromBody] VerifyTokenRequestDto request)
        {
            var principal = _authService.ValidateToken(request.Token);
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
                UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = principal.FindFirst(ClaimTypes.Name)?.Value,
                Role = principal.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { Message = "Authorization token nije pronadjen." });
            }

            var token = authorizationHeader["Bearer ".Length..].Trim();
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrWhiteSpace(jti))
            {
                return BadRequest(new { Message = "Token ne sadrzi jti." });
            }

            _revokedTokenStore.Revoke(jti, jwtToken.ValidTo);
            return Ok(new { Message = "Odjava uspjesna." });
        }
    }
}
