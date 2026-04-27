using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LABsistem.Presentation.DTOs;
using LABsistem.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class KorisnikController : ControllerBase
    {
        private readonly LabSistemDbContext _dbContext;

        public KorisnikController(LabSistemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var korisnici = await _dbContext.Korisnici
                .Select(k => new
                {
                    k.ID,
                    k.ImePrezime,
                    k.Email,
                    k.Username,
                    Uloga = k.Uloga.ToString()
                })
                .ToListAsync();

            return Ok(korisnici);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateKorisnikDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ImePrezime) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Sva polja su obavezna.");
            }

            if (dto.Uloga == UlogaKorisnika.Admin || dto.Uloga == UlogaKorisnika.Student)
                return BadRequest("Admin može kreirati samo Profesor ili Tehnicar naloge.");

            var usernameGreska = ValidirajUsername(dto.Username);
            if (usernameGreska is not null)
                return BadRequest(usernameGreska);

            var passwordGreska = ValidirajPassword(dto.Password);
            if (passwordGreska is not null)
                return BadRequest(passwordGreska);

            if (await _dbContext.Korisnici.AnyAsync(x => x.Username == dto.Username))
                return Conflict("Username je već zauzet.");

            if (await _dbContext.Korisnici.AnyAsync(x => x.Email == dto.Email))
                return Conflict("Email je već registrovan.");

            var korisnik = new Korisnik
            {
                ImePrezime = dto.ImePrezime,
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Uloga = dto.Uloga
            };

            _dbContext.Korisnici.Add(korisnik);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                korisnik.ID,
                korisnik.ImePrezime,
                korisnik.Email,
                korisnik.Username,
                Uloga = korisnik.Uloga.ToString()
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var korisnik = await _dbContext.Korisnici.FindAsync(id);
            if (korisnik is null)
                return NotFound("Korisnik nije pronađen.");

            if (korisnik.Uloga == UlogaKorisnika.Admin)
                return BadRequest("Ne možete obrisati admin nalog.");

            _dbContext.Korisnici.Remove(korisnik);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private static string? ValidirajUsername(string username)
        {
            if (username.Length < 3 || username.Length > 20)
                return "Username mora imati između 3 i 20 znakova.";

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9._]+$"))
                return "Username smije sadržavati samo slova, brojeve, tačku i donju crtu.";

            return null;
        }

        private static string? ValidirajPassword(string password)
        {
            if (password.Length < 8)
                return "Lozinka mora imati najmanje 8 znakova.";

            if (!password.Any(char.IsUpper))
                return "Lozinka mora sadržavati barem jedno veliko slovo.";

            if (!password.Any(char.IsDigit))
                return "Lozinka mora sadržavati barem jedan broj.";

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return "Lozinka mora sadržavati barem jedan specijalni znak.";

            return null;
        }
    }
}

