using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje korisnicima
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class KorisnikController : ControllerBase
    {
        /// <summary>
        /// Dohvata sve korisnike sa mogućnošću filtriranja i paginacije
        /// </summary>
        /// <param name="search">Pretraga po imenu ili emailu</param>
        /// <param name="role">Filter po ulozi</param>
        /// <param name="page">Broj stranice</param>
        /// <param name="pageSize">Broj stavki po stranici</param>
        /// <response code="200">Lista korisnika</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Profesor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // TODO: Implementirati logiku za dohvatanje korisnika
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Dohvata detalje specifičnog korisnika
        /// </summary>
        /// <param name="id">ID korisnika</param>
        /// <response code="200">Detalji korisnika</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Korisnik nije pronađen</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            // TODO: Implementirati logiku za dohvatanje korisnika po ID-u
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Ažurira profil korisnika
        /// </summary>
        /// <param name="id">ID korisnika</param>
        /// <param name="request">Ažurirani podaci korisnika</param>
        /// <response code="200">Profil ažuriran</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Korisnik nije pronađen</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za ažuriranje korisnika
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Mijenja lozinku korisnika
        /// </summary>
        /// <param name="id">ID korisnika</param>
        /// <param name="request">Stara i nova lozinka</param>
        /// <response code="200">Lozinka promijenjena</response>
        /// <response code="400">Neispravni podaci</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="404">Korisnik nije pronađen</response>
        [HttpPost("{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] object request)
        {
            // TODO: Implementirati logiku za promjenu lozinke
            return Ok(new { message = "Endpoint nije implementiran" });
        }

        /// <summary>
        /// Briše korisnika iz sistema (samo admin)
        /// </summary>
        /// <param name="id">ID korisnika za brisanje</param>
        /// <response code="200">Korisnik obrisan</response>
        /// <response code="401">Nedostaje JWT token</response>
        /// <response code="403">Nedovoljna dozvola</response>
        /// <response code="404">Korisnik nije pronađen</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            // TODO: Implementirati logiku za brisanje korisnika
            return Ok(new { message = "Endpoint nije implementiran" });
        }
    }
}
