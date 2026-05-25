using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KabinetController : ControllerBase
    {
        private readonly IKabinetService _service;

        public KabinetController(IKabinetService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Profesor,Tehnicar")]
        public async Task<IActionResult> Get()
        {
            var kabineti = await _service.VratiSveKabinete();
            return Ok(kabineti);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] KabinetCreateDTO dto)
        {
            await _service.KreirajKabinet(dto);
            return Ok(new { message = "Kabinet uspjesno dodan" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] KabinetCreateDTO dto)
        {
            await _service.AzurirajKabinet(id, dto);
            return Ok(new { message = "Kabinet azuriran" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiKabinet(id);
            return Ok(new { message = "Kabinet obrisan" });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Profesor,Tehnicar")]
        public async Task<IActionResult> GetById(int id)
        {
            var kabinet = await _service.VratiKabinetPoId(id);
            if (kabinet == null)
                return NotFound(new { message = "Kabinet nije pronađen" });
            
            return Ok(kabinet);
        }
    }
}
