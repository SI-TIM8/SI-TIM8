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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var profesorId = int.Parse(userId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var profesorId = int.Parse(userId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var studentId = int.Parse(userId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var profesorId = int.Parse(userId);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var korisnikId = int.Parse(userId);
            var uloga = User.FindFirstValue(ClaimTypes.Role)?.ToLower();
            
            if (uloga == null) return BadRequest("Uloga nije pronađena.");
            var termini = await _service.GetMojeRezervacijeAsync(korisnikId, uloga);
            return Ok(termini);
        }

        [HttpGet("dolazni-zahtjevi")]
        [Authorize(Roles = "Profesor")]
        public async Task<IActionResult> GetDolazniZahtjevi()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var profesorId = int.Parse(userId);
            var zahtjevi = await _service.GetDolazniZahtjeviAsync(profesorId);
            return Ok(zahtjevi);
        }

        [HttpGet("dostupni-studentima")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetDostupniStudentima()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            var studentId = int.Parse(userId);
            var termini = await _service.GetDostupniTerminiZaStudenteAsync(studentId);
            return Ok(termini);
        }
    }
}
