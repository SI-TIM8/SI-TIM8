using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RezervacijaController : ControllerBase
    {
        private readonly IRezervacijaService _service;

        public RezervacijaController(IRezervacijaService service)
        {
            _service = service;
        }

        [HttpPost("rezervisi/{id}")]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Rezervisi(int id, [FromBody] RezervacijaCreateDTO dto)
        {
            var profesorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _service.RezervisiTermin(profesorId, id, dto.LimitOsoba, dto.VidljivoStudentima);
                return Ok(new { message = "Termin uspjesno rezervisan." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("otkazi/{id}")]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> Otkazi(int id)
        {
            var profesorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _service.OtkaziTermin(profesorId, id);
                return Ok(new { message = "Rezervacija otkazana." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("zahtjev/{id}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> PosaljiZahtjev(int id)
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _service.PosaljiZahtjev(studentId, id);
                return Ok(new { message = "Zahtjev uspjesno poslan." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("odgovor/{zahtjevId}")]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> OdgovoriNaZahtjev(int zahtjevId, [FromQuery] bool odobri)
        {
            var profesorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _service.OdgovoriNaZahtjev(profesorId, zahtjevId, odobri);
                return Ok(new { message = odobri ? "Zahtjev odobren." : "Zahtjev odbijen." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("slobodni")]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> GetSlobodni()
        {
            var termini = await _service.GetSlobodniTerminiAsync();
            return Ok(termini);
        }

        [HttpGet("moje")]
        public async Task<IActionResult> GetMoje()
        {
            var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var uloga = User.FindFirstValue(ClaimTypes.Role)?.ToLower();
            
            var termini = await _service.GetMojeRezervacijeAsync(korisnikId, uloga);
            return Ok(termini);
        }

        [HttpGet("dolazni-zahtjevi")]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> GetDolazniZahtjevi()
        {
            var profesorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var zahtjevi = await _service.GetDolazniZahtjeviAsync(profesorId);
            return Ok(zahtjevi);
        }

        [HttpGet("dostupni-studentima")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetDostupniStudentima()
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var termini = await _service.GetDostupniTerminiZaStudenteAsync(studentId);
            return Ok(termini);
        }
    }
}
