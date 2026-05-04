using Microsoft.AspNetCore.Mvc;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KabinetController : ControllerBase
    {
        private readonly IKabinetService _service;
        public KabinetController(IKabinetService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var kabineti = await _service.VratiSveKabinete();
            return Ok(kabineti);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] KabinetCreateDTO dto)
        {
            await _service.KreirajKabinet(dto);
            return Ok(new { message = "Kabinet uspješno dodan" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] KabinetCreateDTO dto)
        {
            await _service.AzurirajKabinet(id, dto);
            return Ok(new { message = "Kabinet ažuriran" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiKabinet(id);
            return Ok(new { message = "Kabinet obrisan" });
        }
    }
}