using Microsoft.AspNetCore.Mvc;
using LABsistem.Api.Services;
using LABsistem.Bll.DTOs;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvidencijaController : ControllerBase
    {
        private readonly IEvidencijaService _service;
        public EvidencijaController(IEvidencijaService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var evidencije = await _service.VratiSveEvidencije();
            return Ok(evidencije);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EvidencijaCreateDTO dto)
        {
            await _service.KreirajEvidenciju(dto);
            return Ok(new { message = "Kvar uspješno prijavljen" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EvidencijaUpdateDTO dto)
        {
            await _service.AzurirajStatus(id, dto.Status);
            return Ok(new { message = "Status ažuriran" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiEvidenciju(id);
            return Ok(new { message = "Evidencija obrisana" });
        }
    }
}