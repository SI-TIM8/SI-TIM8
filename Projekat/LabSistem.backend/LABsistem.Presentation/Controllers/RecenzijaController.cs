using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za ocjene i recenzije opreme
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecenzijaController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve recenzije
        /// </summary>
        /// <param name="opremaId">Filter po opremi</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista recenzija</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int? opremaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje recenzija
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Kreira novu recenziju opreme
        /// </summary>
        /// <param name="request">Podaci recenzije</param>
        /// <response code="201">Recenzija kreirana</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] object request)
        {
            // TODO: Implementirati logiku za kreiranje recenzije
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše recenziju
        /// </summary>
        /// <param name="id">ID recenzije</param>
        /// <response code="200">Recenzija obrisana</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Recenzija nije pronađena</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje recenzije
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
