using System.Security.Claims;
using BCrypt.Net;
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
    var korisnik = await _dbContext.Korisnici
        .FirstOrDefaultAsync(x => x.Username == request.Username);

    if (korisnik is null) return null;

    bool isPasswordValid = false;

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

    if (!isPasswordValid) return null;

    
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


        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequestDto request)
        {
            if (!IsPasswordValid(request.Password))
            {
                return (false, "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak.");
            }

            if (await _dbContext.Korisnici.AnyAsync(x => x.Username == request.Username))
            {
                return (false, "Username je već zauzet.");
            }

            var noviKorisnik = new Korisnik
            {
                ImePrezime = request.ImePrezime,
                Email = request.Email,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Uloga = UlogaKorisnika.Student
            };

            _dbContext.Korisnici.Add(noviKorisnik);
            await _dbContext.SaveChangesAsync();

            return (true, "Registracija uspješna.");
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga)
        {
            if (uloga != UlogaKorisnika.Profesor && uloga != UlogaKorisnika.Tehnicar)
            {
                return (false, "Admin može kreirati samo Profesore ili Tehničare.");
            }

            if (!IsPasswordValid(request.Password))
            {
                return (false, "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak.");
            }

            if (await _dbContext.Korisnici.AnyAsync(x => x.Username == request.Username))
            {
                return (false, "Username je već zauzet.");
            }

            var noviKorisnik = new Korisnik
            {
                ImePrezime = request.ImePrezime,
                Email = request.Email,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Uloga = uloga
            };

            _dbContext.Korisnici.Add(noviKorisnik);
            await _dbContext.SaveChangesAsync();

            return (true, "Korisnik uspješno kreiran.");
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            return _jwtService.ValidateToken(token);
        }

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
            return true;
        }
    }
}
