using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Services;
using LABsistem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za autentifikaciju korisnika - login, logout i token validacija
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Prijava korisnika i generisanje JWT tokena
        /// </summary>
        /// <param name="request">Login kredencijali (username, password)</param>
        /// <returns>JWT token i korisničke informacije</returns>
        /// <response code="200">Uspješan login, vraćen je token</response>
        /// <response code="400">Nedostaju obavezni podaci</response>
        /// <response code="401">Neispravni kredencijali</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { Message = "Pogrešni kredencijali." });
            }

            return Ok(response);
        }

        /// <summary>
        /// Registracija novog korisnika (Student)
        /// </summary>
        /// <param name="request">Podaci za registraciju (ime, email, username, password)</param>
        /// <returns>Potvrda registracije</returns>
        /// <response code="200">Uspješna registracija</response>
        /// <response code="400">Neispravni podaci ili korisnik već postoji</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });
        }

        /// <summary>
        /// Kreiranje korisnika od strane administratora (Profesor ili Tehničar)
        /// </summary>
        /// <param name="request">Podaci novog korisnika</param>
        /// <param name="uloga">Uloga korisnika (Profesor ili Tehnicar)</param>
        /// <returns>Potvrda kreiranja korisnika</returns>
        /// <response code="200">Korisnik uspješno kreiran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="403">Nedovoljna dozvola (samo admin)</response>
        [HttpPost("create-user")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto request, [FromQuery] UlogaKorisnika uloga)
        {
            var result = await _authService.CreateUserAsync(request, uloga);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });
        }

        /// <summary>
        /// Verifikacija trenutno prijavljivanog korisnika (koristi JWT iz headera)
        /// </summary>
        /// <returns>Informacije o prijavljenom korisniku</returns>
        /// <response code="200">Korisnik je validan i prijavljen</response>
        /// <response code="401">Token nije validan ili je istekao</response>
        [HttpGet("verify")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Verifikacija JWT tokena (tokenstring se šalje u body-ju)
        /// </summary>
        /// <param name="request">Objekt sa JWT tokenom za verifikaciju</param>
        /// <returns>Status validnosti tokena i korisničke informacije</returns>
        /// <response code="200">Token je validan</response>
        /// <response code="401">Token nije validan ili je istekao</response>
        [HttpPost("verify-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
