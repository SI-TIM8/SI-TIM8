using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Bll.Services
{
    public class AuthService : IAuthService
    {
        private readonly LabSistemDbContext _dbContext;
        private readonly IJwtService _jwtService;

        public AuthService(LabSistemDbContext dbContext, IJwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            var korisnickiIdentifikator = request.Username.Trim();

            var korisnik = await _dbContext.Korisnici
                .FirstOrDefaultAsync(x =>
                    x.Username == korisnickiIdentifikator ||
                    x.Email == korisnickiIdentifikator);

            if (korisnik is null)
            {
                return null;
            }

            var isPasswordValid = false;

            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, korisnik.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                if (korisnik.Password == request.Password)
                {
                    isPasswordValid = true;
                    korisnik.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!isPasswordValid)
            {
                return null;
            }

            var token = _jwtService.GenerateToken(
                korisnik.ID.ToString(),
                korisnik.Username,
                korisnik.Uloga.ToString());

            return new LoginResponseDto
            {
                Token = token,
                UserId = korisnik.ID,
                Username = korisnik.Username,
                Role = korisnik.Uloga.ToString()
            };
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga)
        {
            if (uloga != UlogaKorisnika.Profesor && uloga != UlogaKorisnika.Tehnicar)
            {
                return (false, "Admin moze kreirati samo Profesore ili Tehnicare.");
            }

            var validationMessage = ValidateRequiredFields(request);
            if (validationMessage is not null)
            {
                return (false, validationMessage);
            }

            if (!IsPasswordValid(request.Password))
            {
                return (false, "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak.");
            }

            if (await _dbContext.Korisnici.AnyAsync(x => x.Username == request.Username))
            {
                return (false, "Username je vec zauzet.");
            }

            if (await _dbContext.Korisnici.AnyAsync(x => x.Email == request.Email))
            {
                return (false, "Email je vec zauzet.");
            }

            var noviKorisnik = new Korisnik
            {
                ImePrezime = request.ImePrezime.Trim(),
                Email = request.Email.Trim(),
                Username = request.Username.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Uloga = uloga
            };

            _dbContext.Korisnici.Add(noviKorisnik);
            await _dbContext.SaveChangesAsync();

            return (true, "Korisnik uspjesno kreiran.");
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            return _jwtService.ValidateToken(token);
        }

        private static string? ValidateRequiredFields(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ImePrezime))
            {
                return "Ime i prezime je obavezno.";
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return "Email je obavezan.";
            }

            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return "Username je obavezan.";
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return "Lozinka je obavezna.";
            }

            return null;
        }

        private static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
            return true;
        }
    }
}
