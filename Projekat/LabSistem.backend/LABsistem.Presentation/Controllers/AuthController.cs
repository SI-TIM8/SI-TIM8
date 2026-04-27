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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized("Pogrešni kredencijali.");
            }

            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { Message = result.Message });
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto request, [FromQuery] UlogaKorisnika uloga)
        {
            var result = await _authService.CreateUserAsync(request, uloga);
            if (!result.Success)
            {
                return BadRequest(result.Message);
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

            return Ok(new
            {
                Valid = true,
                UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = principal.FindFirst(ClaimTypes.Name)?.Value,
                Role = principal.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}
