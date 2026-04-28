using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje zahtjevima i rezervacijama
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ZahtjevController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve zahtjeve (samo za Profesore i Admin)
        /// </summary>
        /// <param name="status">Filter po statusu</param>
        /// <param name="kabinetId">Filter po kabinetu</param>
        /// <param name="datum">Filter po datumu</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista zahtjeva</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpGet]
        [Authorize(Roles = "Profesor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? kabinetId, [FromQuery] string? datum, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje svih zahtjeva
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata vlastite zahtjeve korisnika
        /// </summary>
        /// <param name="status">Filter po statusu</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista vlastitih zahtjeva</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet("moji")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMine([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje vlastitih zahtjeva
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata detalje specifičnog zahtjeva
        /// </summary>
        /// <param name="id">ID zahtjeva</param>
        /// <response code="200">Detalji zahtjeva</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Zahtjev nije pronađen</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Implementirati logiku za dohvatanje zahtjeva po ID-u
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Kreira novi zahtjev za rezervaciju
        /// </summary>
        /// <param name="request">Podaci zahtjeva</param>
        /// <response code="201">Zahtjev podnesen</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="409">Konflikt - termin je zauzet ili dostignut limit</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] object request)
        {
            // TODO: Implementirati logiku za kreiranje zahtjeva
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Odobrava zahtjev (samo Profesor ili Admin)
        /// </summary>
        /// <param name="id">ID zahtjeva</param>
        /// <param name="request">Komentar odobrenja</param>
        /// <response code="200">Zahtjev odobren</response>
        /// <response code="400">Zahtjev nije u statusu 'Na čekanju'</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Zahtjev nije pronađen</response>
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Profesor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za odobravanje zahtjeva
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Odbija zahtjev (samo Profesor ili Admin)
        /// </summary>
        /// <param name="id">ID zahtjeva</param>
        /// <param name="request">Razlog odbijanja</param>
        /// <response code="200">Zahtjev odbijen</response>
        /// <response code="400">Zahtjev nije u statusu 'Na čekanju'</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Zahtjev nije pronađen</response>
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Profesor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reject(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za odbijanje zahtjeva
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Otkazuje (briše) zahtjev
        /// </summary>
        /// <param name="id">ID zahtjeva</param>
        /// <param name="request">Razlog otkazivanja</param>
        /// <response code="200">Zahtjev otkazan</response>
        /// <response code="400">Zahtjev nije u statusu 'Odobren'</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Zahtjev nije pronađen</response>
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za otkazivanje zahtjeva
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
