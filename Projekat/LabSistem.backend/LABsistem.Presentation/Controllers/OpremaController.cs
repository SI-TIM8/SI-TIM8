using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpremaController : ControllerBase
    {
        private readonly IOpremaService _service;

        public OpremaController(IOpremaService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Profesor,Tehnicar")]
        public async Task<IActionResult> Get([FromQuery] string prikaz = "aktivna")
        {
            var oprema = await _service.VratiSvuOpremu(prikaz);
            return Ok(oprema);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Post([FromBody] OpremaCreateDTO dto)
        {
            await _service.KreirajOpremu(dto);
            return Ok(new { message = "Oprema uspješno dodana." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Put(int id, [FromBody] OpremaCreateDTO dto)
        {
            await _service.AzurirajOpremu(id, dto);
            return Ok(new { message = "Oprema ažurirana." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Delete(int id)
        {
            var uspjeh = await _service.ArhivirajOpremu(id);
            if (!uspjeh) return NotFound();
            return Ok(new { message = "Oprema arhivirana." });
        }

        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Restore(int id)
        {
            var uspjeh = await _service.VratiIzArhive(id);
            if (!uspjeh) return NotFound();
            return Ok(new { message = "Oprema vraćena iz arhive." });
        }

        [HttpGet("kabinet/{kabinetId}")]
        [Authorize]
        public async Task<IActionResult> GetPoKabinetu(int kabinetId)
        {
            var oprema = await _service.VratiOpremuPoKabinetu(kabinetId);
            return Ok(oprema);
        }
    }
}
