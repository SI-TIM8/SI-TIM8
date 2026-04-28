using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje objektima (zgradama)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ObjekatController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve objekte
        /// </summary>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista objekata</response>
        /// <response code="401">Nedostaje JWT token</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje objekata
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata detalje specifičnog objekta
        /// </summary>
        /// <param name="id">ID objekta</param>
        /// <response code="200">Detalji objekta</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Objekat nije pronađen</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Implementirati logiku za dohvatanje objekta po ID-u
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Kreira novi objekat (samo admin)
        /// </summary>
        /// <param name="request">Podaci novog objekta</param>
        /// <response code="201">Objekat kreiran</response>
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
            // TODO: Implementirati logiku za kreiranje objekta
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Ažurira objekat
        /// </summary>
        /// <param name="id">ID objekta</param>
        /// <param name="request">Ažurirani podaci</param>
        /// <response code="200">Objekat ažuriran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Objekat nije pronađen</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za ažuriranje objekta
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše objekat (samo admin)
        /// </summary>
        /// <param name="id">ID objekta</param>
        /// <response code="200">Objekat obrisan</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Objekat nije pronađen</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje objekta
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
