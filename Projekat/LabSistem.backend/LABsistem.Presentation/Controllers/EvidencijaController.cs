using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvidencijaController : ControllerBase
    {
        private readonly IEvidencijaService _service;

        public EvidencijaController(IEvidencijaService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Get()
        {
            var evidencije = await _service.VratiSveEvidencije();
            return Ok(evidencije);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Post([FromBody] EvidencijaCreateDTO dto)
        {
            await _service.KreirajEvidenciju(dto);
            return Ok(new { message = "Kvar uspjesno prijavljen" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Put(int id, [FromBody] EvidencijaUpdateDTO dto)
        {
            await _service.AzurirajStatus(id, dto.Status);
            return Ok(new { message = "Status azuriran" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.ObrisiEvidenciju(id);
            return Ok(new { message = "Evidencija obrisana" });
        }
    }
}
