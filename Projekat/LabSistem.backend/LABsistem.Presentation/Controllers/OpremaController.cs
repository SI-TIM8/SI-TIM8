using Microsoft.AspNetCore.Mvc;
using LABsistem.Bll.Interfaces;
using LABsistem.Dal.Interfaces;
using LABSistem.Bll.DTOs;
using LABsistem.Api.Services;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class OpremaController : ControllerBase
{
    private readonly IOpremaService _service;
    public OpremaController(IOpremaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var oprema = await _service.VratiSvuOpremu();
        return Ok(oprema);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OpremaCreateDTO dto)
    {
        await _service.KreirajOpremu(dto);
        return Ok(new { message = "Oprema uspješno dodana" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] OpremaCreateDTO dto)
    {
        await _service.AzurirajOpremu(id, dto);
        return Ok(new { message = "Oprema ažurirana" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.ObrisiOpremu(id);
        return Ok(new { message = "Oprema obrisana" });
    }
}
}
