using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje opremom
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OpremaController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve opreme sa mogućnošću filtriranja
        /// </summary>
        /// <param name="kabinetId">Filter po kabinetu</param>
        /// <param name="status">Filter po statusu</param>
        /// <param name="search">Pretraga po nazivu</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista opreme</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int? kabinetId, [FromQuery] string? status, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje opreme
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata detalje specifične opreme
        /// </summary>
        /// <param name="id">ID opreme</param>
        /// <response code="200">Detalji opreme</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Oprema nije pronađena</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Implementirati logiku za dohvatanje opreme po ID-u
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Kreira novu opremu (samo Tehničar ili Admin)
        /// </summary>
        /// <param name="request">Podaci nove opreme</param>
        /// <response code="201">Oprema kreiirana</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpPost]
        [Authorize(Roles = "Tehnicar,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] object request)
        {
            // TODO: Implementirati logiku za kreiranje opreme
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Ažurira opremu
        /// </summary>
        /// <param name="id">ID opreme</param>
        /// <param name="request">Ažurirani podaci</param>
        /// <response code="200">Oprema ažurirana</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Oprema nije pronađena</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Tehnicar,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za ažuriranje opreme
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Prijava kvara opreme
        /// </summary>
        /// <param name="id">ID opreme</param>
        /// <param name="request">Opis kvara</param>
        /// <response code="200">Kvar prijavljen</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Oprema nije pronađena</response>
        [HttpPost("{id}/prijava-kvara")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReportError(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za prijavu kvara
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše opremu
        /// </summary>
        /// <param name="id">ID opreme</param>
        /// <response code="200">Oprema obrisana</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Oprema nije pronađena</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Tehnicar,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje opreme
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
