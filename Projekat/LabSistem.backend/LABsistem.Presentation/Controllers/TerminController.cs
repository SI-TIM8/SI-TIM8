using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje terminima
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TerminController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve termine sa mogućnošću filtriranja
        /// </summary>
        /// <param name="kabinetId">Filter po kabinetu</param>
        /// <param name="datumOd">Početni datum</param>
        /// <param name="datumDo">Završni datum</param>
        /// <param name="slobodni">Samo slobodni termini</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista termina</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int? kabinetId, [FromQuery] string? datumOd, [FromQuery] string? datumDo, [FromQuery] bool? slobodni, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje termina
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata detalje specifičnog termina
        /// </summary>
        /// <param name="id">ID termina</param>
        /// <response code="200">Detalji termina</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Termin nije pronađen</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Implementirati logiku za dohvatanje termina po ID-u
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Kreira novi termin (samo Profesor ili Tehničar)
        /// </summary>
        /// <param name="request">Podaci novog termina</param>
        /// <response code="201">Termin kreiran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="409">Konflikt - termin se preklapa</response>
        [HttpPost]
        [Authorize(Roles = "Profesor,Tehnicar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] object request)
        {
            // TODO: Implementirati logiku za kreiranje termina
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Ažurira termin
        /// </summary>
        /// <param name="id">ID termina</param>
        /// <param name="request">Ažurirani podaci</param>
        /// <response code="200">Termin ažuriran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Termin nije pronađen</response>
        /// <response code="409">Konflikt - termin se preklapa</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Profesor,Tehnicar,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za ažuriranje termina
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše termin
        /// </summary>
        /// <param name="id">ID termina</param>
        /// <response code="200">Termin obrisan</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Termin nije pronađen</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Profesor,Tehnicar,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje termina
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
