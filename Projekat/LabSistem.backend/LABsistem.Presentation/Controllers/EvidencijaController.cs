using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za audit log i evidenciju aktivnosti
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EvidencijaController : ControllerBase
    {
        /// <summary>
        /// Dohvata audit log svih aktivnosti u sistemu (samo za Admin)
        /// </summary>
        /// <param name="korisnikId">Filter po korisniku</param>
        /// <param name="datumOd">Početni datum</param>
        /// <param name="datumDo">Završni datum</param>
        /// <param name="akcija">Filter po akciji</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Audit log</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] int? korisnikId, [FromQuery] string? datumOd, [FromQuery] string? datumDo, [FromQuery] string? akcija, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje audit loga
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata evidenciju specifičnog korisnika
        /// </summary>
        /// <param name="korisnikId">ID korisnika</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Evidencija korisnika</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpGet("user/{korisnikId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByUserId(int korisnikId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje evidencije korisnika
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
