using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Application.Services;
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
            var response = await _authService.LoginAsync(
                request,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            if (response == null)
            {
                return Unauthorized(new { Message = "Pogresni kredencijali." });
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var response = await _authService.RefreshAsync(
                request.RefreshToken,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString());

            if (response is null)
            {
                return Unauthorized(new { Message = "Refresh token nije validan." });
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

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPut("users/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateManagedUserRequestDto request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(new { Message = "Neispravan korisnicki identitet." });
            }

            var result = await _authService.UpdateUserAsync(currentUserId, userId, request);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, User = result.User });
        }

        [HttpPost("users/{userId:int}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(new { Message = "Neispravan korisnicki identitet." });
            }

            var result = await _authService.DeactivateUserAsync(currentUserId, userId);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, User = result.User });
        }

        [HttpPost("users/{userId:int}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Unauthorized(new { Message = "Neispravan korisnicki identitet." });
            }

            var result = await _authService.ActivateUserAsync(currentUserId, userId);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, User = result.User });
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
        public async Task<IActionResult> VerifyToken([FromBody] VerifyTokenRequestDto request)
        {
            var principal = _authService.ValidateToken(request.Token);
            if (principal is null)
            {
                return Unauthorized(new { Valid = false });
            }

            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId) || !await _authService.IsUserActiveAsync(userId))
            {
                return Unauthorized(new { Valid = false });
            }

            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (!string.IsNullOrWhiteSpace(jti) &&
                await _revokedTokenStore.IsRevokedAsync(jti))
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
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request)
        {
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (string.IsNullOrWhiteSpace(jti))
            {
                return BadRequest(new { Message = "Token ne sadrzi jti." });
            }

            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { Message = "Authorization header ne sadrzi bearer token." });
            }

            var token = authorizationHeader["Bearer ".Length..].Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                return BadRequest(new { Message = "Token ne sadrzi ispravan datum isteka." });
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expiresAtUtc = jwtToken.ValidTo;

            await _revokedTokenStore.RevokeAsync(jti, expiresAtUtc);

            if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
            {
                await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
            }

            return Ok(new { Message = "Odjava uspjesna." });
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out userId);
        }
    }
}
