using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje obavijestima korisnika
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ObavijestController : ControllerBase
    {
        /// <summary>
        /// Dohvata obavijesti za trenutnog korisnika
        /// </summary>
        /// <param name="procitano">Filter po statusu čitanja</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista obavijesti</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMine([FromQuery] bool? procitano, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje obavijesti
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Označava obavijest kao pročitanu
        /// </summary>
        /// <param name="id">ID obavijesti</param>
        /// <response code="200">Obavijest označena</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Obavijest nije pronađena</response>
        [HttpPut("{id}/mark-as-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            // TODO: Implementirati logiku za označavanje obavijesti
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše obavijest
        /// </summary>
        /// <param name="id">ID obavijesti</param>
        /// <response code="200">Obavijest obrisana</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Obavijest nije pronađena</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje obavijesti
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
