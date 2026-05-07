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
        public async Task<IActionResult> Get()
        {
            var oprema = await _service.VratiSvuOpremu();
            return Ok(oprema);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Post([FromBody] OpremaCreateDTO dto)
        {
            await _service.KreirajOpremu(dto);
            return Ok(new { message = "Oprema uspjesno dodana" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Put(int id, [FromBody] OpremaCreateDTO dto)
        {
            await _service.AzurirajOpremu(id, dto);
            return Ok(new { message = "Oprema azurirana" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiOpremu(id);
            return Ok(new { message = "Oprema obrisana" });
        }
    }
}
