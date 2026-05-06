using Microsoft.AspNetCore.Mvc;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjekatController : ControllerBase
    {
        private readonly IObjekatService _service;
        public ObjekatController(IObjekatService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.VratiSveObjekte());

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ObjekatCreateDTO dto)
        {
            await _service.KreirajObjekat(dto);
            return Ok(new { message = "Objekat uspješno dodan" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ObjekatCreateDTO dto)
        {
            await _service.AzurirajObjekat(id, dto);
            return Ok(new { message = "Objekat ažuriran" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiObjekat(id);
            return Ok(new { message = "Objekat obrisan" });
        }
    }
}