using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin,Profesor,Tehnicar")]
        public async Task<IActionResult> Post([FromBody] EvidencijaCreateDTO dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _service.KreirajEvidenciju(dto, int.Parse(userId));
                return Ok(new { message = "Kvar uspjesno prijavljen" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Put(int id, [FromBody] EvidencijaUpdateDTO dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                await _service.AzurirajStatus(id, dto, int.Parse(userId));
                return Ok(new { message = "Status azuriran" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
