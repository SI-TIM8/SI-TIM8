using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje kabinetima (laboratorijama)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KabinetController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve kabinete
        /// </summary>
        /// <param name="objektId">Filter po objektu</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista kabineta</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int? objektId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje kabineta
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata detalje specifičnog kabineta
        /// </summary>
        /// <param name="id">ID kabineta</param>
        /// <response code="200">Detalji kabineta</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Kabinet nije pronađen</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Implementirati logiku za dohvatanje kabineta po ID-u
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Kreira novi kabinet (samo admin)
        /// </summary>
        /// <param name="request">Podaci novog kabineta</param>
        /// <response code="201">Kabinet kreiran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] object request)
        {
            // TODO: Implementirati logiku za kreiranje kabineta
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Ažurira kabinet
        /// </summary>
        /// <param name="id">ID kabineta</param>
        /// <param name="request">Ažurirani podaci</param>
        /// <response code="200">Kabinet ažuriran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Kabinet nije pronađen</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za ažuriranje kabineta
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Blokira vremenski period u kabinetu
        /// </summary>
        /// <param name="id">ID kabineta</param>
        /// <param name="request">Period za blokiranje</param>
        /// <response code="200">Period blokiran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpPost("{id}/block-period")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> BlockPeriod(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za blokiranje perioda
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše kabinet (samo admin)
        /// </summary>
        /// <param name="id">ID kabineta</param>
        /// <response code="200">Kabinet obrisan</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Kabinet nije pronađen</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje kabineta
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
