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

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { Message = "Neispravan korisnicki identitet." });
            }

            var profile = await _authService.GetProfileAsync(userId);
            if (profile is null)
            {
                return NotFound(new { Message = "Profil nije pronadjen." });
            }

            return Ok(profile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { Message = "Neispravan korisnicki identitet." });
            }

            var result = await _authService.UpdateProfileAsync(userId, request);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, Profile = result.Profile });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { Message = "Neispravan korisnicki identitet." });
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });
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
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrWhiteSpace(jti))
            {
                return BadRequest(new { Message = "Token ne sadrzi jti." });
            }

            var expirationClaim = User.FindFirstValue(JwtRegisteredClaimNames.Exp);
            if (string.IsNullOrWhiteSpace(expirationClaim) || !long.TryParse(expirationClaim, out var expirationUnixSeconds))
            {
                return BadRequest(new { Message = "Token ne sadrzi ispravan datum isteka." });
            }

            var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expirationUnixSeconds).UtcDateTime;
            _revokedTokenStore.Revoke(jti, expiresAtUtc);
            return Ok(new { Message = "Odjava uspjesna." });
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out userId);
        }
    }
}
