using Microsoft.AspNetCore.Mvc;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TerminController : ControllerBase
    {
        private readonly ITerminService _service;
        public TerminController(ITerminService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var termini = await _service.VratiSveTermine();
            return Ok(termini);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TerminCreateDTO dto)
        {
            await _service.KreirajTermin(dto);
            return Ok(new { message = "Termin uspjesno dodan" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TerminCreateDTO dto)
        {
            await _service.AzurirajTermin(id, dto);
            return Ok(new { message = "Termin azuriran" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiTermin(id);
            return Ok(new { message = "Termin obrisan" });
        }
    }
}
