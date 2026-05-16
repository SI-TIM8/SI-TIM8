using LABsistem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LABsistem.Dal.Db;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ObavijestController : ControllerBase
    {
        private readonly IObavijestService _service;
        private readonly LabSistemDbContext _context;

        public ObavijestController(IObavijestService service, LabSistemDbContext context)
        {
            _service = service;
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var lista = await _service.VratiZaKorisnikaAsync(int.Parse(userId));
            return Ok(lista);
        }

        [HttpGet("broj")]
        public async Task<IActionResult> GetBroj()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var broj = await _service.BrojNeprocitanihAsync(int.Parse(userId));
            return Ok(new { broj });
        }

        [HttpPut("{id}/procitana")]
        public async Task<IActionResult> OznaciProcitanu(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            await _service.OznaciKaoProcitanuAsync(id, int.Parse(userId));
            return Ok();
        }

        [HttpPut("sve-procitane")]
        public async Task<IActionResult> OznaciSveProcitane()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            await _service.OznaciSveKaoProcitaneAsync(int.Parse(userId));
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Obrisi(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var o = await _context.Obavijesti
                .FirstOrDefaultAsync(x => x.ID == id && x.KorisnikID == int.Parse(userId));
            if (o == null) return NotFound();
            _context.Obavijesti.Remove(o);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("sve")]
        public async Task<IActionResult> ObrisiSve()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var sve = await _context.Obavijesti
                .Where(o => o.KorisnikID == int.Parse(userId))
                .ToListAsync();
            _context.Obavijesti.RemoveRange(sve);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}