using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObjekatController : ControllerBase
    {
        private readonly IObjekatService _service;

        public ObjekatController(IObjekatService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Profesor,Tehnicar")]
        public async Task<IActionResult> Get() => Ok(await _service.VratiSveObjekte());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] ObjekatCreateDTO dto)
        {
            await _service.KreirajObjekat(dto);
            return Ok(new { message = "Objekat uspjesno dodan" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] ObjekatCreateDTO dto)
        {
            await _service.AzurirajObjekat(id, dto);
            return Ok(new { message = "Objekat azuriran" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiObjekat(id);
            return Ok(new { message = "Objekat obrisan" });
        }
    }
}
