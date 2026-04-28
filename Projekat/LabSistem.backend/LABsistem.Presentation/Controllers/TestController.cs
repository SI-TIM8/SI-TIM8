using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za testiranje API infrastrukture i autentifikacije
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Javni endpoint za testiranje konekcije sa API-jem
        /// </summary>
        /// <returns>Poruka o uspješnoj konekciji</returns>
        /// <response code="200">API je dostupan</response>
        [HttpGet("health")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "healthy",
                Message = "LABsistem API je dostupan",
                Timestamp = DateTime.UtcNow,
                Version = "v1.0"
            });
        }

        /// <summary>
        /// Zaštićeni endpoint koji zahtijeva JWT autentifikaciju
        /// </summary>
        /// <returns>Poruka sa informacijom da je korisnik prijavljen</returns>
        /// <response code="200">Korisnik je autentifikovan</response>
        /// <response code="401">Nedostaje ili je nevaljani JWT token</response>
        [HttpGet("secure-test")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult SecureTest()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "Unknown";
            return Ok(new
            {
                Message = "Pristup je dozvoljen - JWT token je validan",
                AuthenticatedUser = userId,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Zaštićeni endpoint koji zahtijeva Admin ulogu
        /// </summary>
        /// <returns>Poruka dostupna samo administratorima</returns>
        /// <response code="200">Admin je autentifikovan</response>
        /// <response code="401">Nedostaje ili je nevaljani JWT token</response>
        /// <response code="403">Nedovoljna dozvola (samo za Admin ulogu)</response>
        [HttpGet("admin-test")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult AdminTest()
        {
            return Ok(new
            {
                Message = "Pristup je dozvoljen - Vi ste administrator",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
