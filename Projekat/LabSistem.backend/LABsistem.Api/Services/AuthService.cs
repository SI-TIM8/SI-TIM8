using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Models;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Bll.Services
{
    public class AuthService : IAuthService
    {
        private static readonly Regex UsernameRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);
        private readonly LabSistemDbContext _dbContext;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(LabSistemDbContext dbContext, IJwtService jwtService, JwtSettings jwtSettings)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? deviceInfo = null)
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

            if (!VerifyPassword(korisnik, request.Password))
            {
                return null;
            }

            if (NeedsPasswordUpgrade(korisnik.Password))
            {
                korisnik.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await _dbContext.SaveChangesAsync();
            }

            return await IssueSessionAsync(korisnik, ipAddress, deviceInfo);
        }

        public async Task<LoginResponseDto?> RefreshAsync(string refreshToken, string? ipAddress = null, string? deviceInfo = null)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var refreshTokenHash = HashToken(refreshToken);

            var existingRefreshToken = await _dbContext.RefreshTokens
                .Include(x => x.Korisnik)
                .FirstOrDefaultAsync(x => x.TokenHash == refreshTokenHash);

            if (existingRefreshToken is null ||
                existingRefreshToken.RevokedAtUtc.HasValue ||
                existingRefreshToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return null;
            }

            existingRefreshToken.LastUsedAtUtc = DateTime.UtcNow;
            existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;

            var response = await IssueSessionAsync(
                existingRefreshToken.Korisnik,
                ipAddress,
                deviceInfo,
                existingRefreshToken);

            return response;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            var refreshTokenHash = HashToken(refreshToken);

            var existingRefreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == refreshTokenHash);

            if (existingRefreshToken is null || existingRefreshToken.RevokedAtUtc.HasValue)
            {
                return false;
            }

            existingRefreshToken.RevokedAtUtc = DateTime.UtcNow;
            existingRefreshToken.LastUsedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProfileResponseDto?> GetProfileAsync(int userId)
        {
            var profile = await _dbContext.Korisnici
                .Where(x => x.ID == userId)
                .Select(x => new ProfileResponseDto
                {
                    UserId = x.ID,
                    ImePrezime = x.ImePrezime,
                    Email = x.Email,
                    Username = x.Username,
                    Role = x.Uloga.ToString()
                })
                .FirstOrDefaultAsync();

            if (profile is null)
            {
                return null;
            }

            profile.RecentActivities = await BuildRecentActivitiesAsync(userId);
            return profile;
        }

        public async Task<List<UserListItemDto>> GetUsersAsync()
        {
            return await _dbContext.Korisnici
                .OrderBy(x => x.ImePrezime)
                .Select(x => new UserListItemDto
                {
                    UserId = x.ID,
                    ImePrezime = x.ImePrezime,
                    Email = x.Email,
                    Username = x.Username,
                    Role = x.Uloga.ToString()
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, ProfileResponseDto? Profile)> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
        {
            var korisnik = await _dbContext.Korisnici.FirstOrDefaultAsync(x => x.ID == userId);

            if (korisnik is null)
            {
                return (false, "Korisnik nije pronadjen.", null);
            }

            var validationMessage = ValidateProfileFields(request.ImePrezime, request.Email, request.Username);
            if (validationMessage is not null)
            {
                return (false, validationMessage, null);
            }

            var normalizedUsername = request.Username.Trim();
            var normalizedEmail = request.Email.Trim();

            if (await _dbContext.Korisnici.AnyAsync(x => x.ID != userId && x.Username == normalizedUsername))
            {
                return (false, "Username je vec zauzet.", null);
            }

            if (await _dbContext.Korisnici.AnyAsync(x => x.ID != userId && x.Email == normalizedEmail))
            {
                return (false, "Email je vec zauzet.", null);
            }

            korisnik.ImePrezime = request.ImePrezime.Trim();
            korisnik.Email = normalizedEmail;
            korisnik.Username = normalizedUsername;

            await _dbContext.SaveChangesAsync();

            var updatedProfile = await GetProfileAsync(userId);
            return (true, "Profil je uspjesno azuriran.", updatedProfile);
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            var korisnik = await _dbContext.Korisnici.FirstOrDefaultAsync(x => x.ID == userId);

            if (korisnik is null)
            {
                return (false, "Korisnik nije pronadjen.");
            }

            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return (false, "Sva polja za promjenu lozinke su obavezna.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return (false, "Nova lozinka i potvrda se ne poklapaju.");
            }

            if (!VerifyPassword(korisnik, request.CurrentPassword))
            {
                return (false, "Trenutna lozinka nije ispravna.");
            }

            if (!IsPasswordValid(request.NewPassword))
            {
                return (false, "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak.");
            }

            korisnik.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _dbContext.SaveChangesAsync();

            return (true, "Lozinka je uspjesno promijenjena.");
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga)
        {
            var validationMessage = ValidateRequiredFields(request);
            if (validationMessage is not null)
            {
                return (false, validationMessage);
            }

            if (!IsPasswordValid(request.Password))
            {
                return (false, "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak.");
            }

            var normalizedUsername = request.Username.Trim();
            var normalizedEmail = request.Email.Trim();

            if (await _dbContext.Korisnici.AnyAsync(x => x.Username == normalizedUsername))
            {
                return (false, "Username je vec zauzet.");
            }

            if (await _dbContext.Korisnici.AnyAsync(x => x.Email == normalizedEmail))
            {
                return (false, "Email je vec zauzet.");
            }

            var noviKorisnik = new Korisnik
            {
                ImePrezime = request.ImePrezime.Trim(),
                Email = normalizedEmail,
                Username = normalizedUsername,
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

        private async Task<LoginResponseDto> IssueSessionAsync(
            Korisnik korisnik,
            string? ipAddress,
            string? deviceInfo,
            RefreshToken? previousRefreshToken = null)
        {
            var accessToken = _jwtService.GenerateToken(
                korisnik.ID.ToString(),
                korisnik.Username,
                korisnik.Uloga.ToString());

            var accessTokenExpiresAtUtc = _jwtService.GetTokenExpirationUtc(accessToken);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpireDays);
            var refreshTokenHash = HashToken(refreshToken);

            if (previousRefreshToken is not null)
            {
                previousRefreshToken.ReplacedByTokenHash = refreshTokenHash;
            }

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                KorisnikID = korisnik.ID,
                TokenHash = refreshTokenHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshTokenExpiresAtUtc,
                LastUsedAtUtc = DateTime.UtcNow,
                DeviceInfo = TrimToLength(deviceInfo, 256),
                IpAddress = TrimToLength(ipAddress, 64)
            });

            await _dbContext.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = accessToken,
                AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
                UserId = korisnik.ID,
                Username = korisnik.Username,
                Role = korisnik.Uloga.ToString()
            };
        }

        private static string HashToken(string value)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value.Trim()));
            return Convert.ToHexString(bytes);
        }

        private static string? TrimToLength(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmedValue = value.Trim();
            return trimmedValue.Length <= maxLength
                ? trimmedValue
                : trimmedValue[..maxLength];
        }

        private static bool NeedsPasswordUpgrade(string storedPassword)
        {
            try
            {
                BCrypt.Net.BCrypt.InterrogateHash(storedPassword);
                return false;
            }
            catch (BCrypt.Net.HashInformationException)
            {
                return true;
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return true;
            }
        }

        private static bool VerifyPassword(Korisnik korisnik, string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, korisnik.Password);
            }
            catch (BCrypt.Net.HashInformationException)
            {
                return korisnik.Password == password;
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return korisnik.Password == password;
            }
        }

        private async Task<List<RecentActivityDto>> BuildRecentActivitiesAsync(int userId)
        {
            var recentTermini = await _dbContext.Termini
                .Where(x => x.KreatorID == userId)
                .OrderByDescending(x => x.Datum)
                .ThenByDescending(x => x.VrijemePocetka)
                .Take(3)
                .Select(x => new RecentActivityDto
                {
                    Title = "Kreirali ste termin",
                    Description = $"{x.Datum:dd.MM.yyyy} u {x.VrijemePocetka:hh\\:mm}",
                    Meta = x.Kabinet == null ? "Termin" : $"Kabinet: {x.Kabinet.Naziv}"
                })
                .ToListAsync();

            var recentEvidencije = await _dbContext.Evidencije
                .Where(x => x.KorisnikID == userId)
                .OrderByDescending(x => x.ID)
                .Take(3)
                .Select(x => new RecentActivityDto
                {
                    Title = "Dodali ste evidenciju",
                    Description = x.Oprema == null ? "Evidencija opreme" : $"Oprema: {x.Oprema.Naziv}",
                    Meta = string.IsNullOrWhiteSpace(x.Status) ? $"Evidencija #{x.ID}" : $"Status: {x.Status}"
                })
                .ToListAsync();

            return recentTermini
                .Concat(recentEvidencije)
                .Take(6)
                .ToList();
        }

        private static string? ValidateProfileFields(string imePrezime, string email, string username)
        {
            if (string.IsNullOrWhiteSpace(imePrezime))
            {
                return "Ime i prezime je obavezno.";
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email je obavezan.";
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return "Username je obavezan.";
            }

            if (!UsernameRegex.IsMatch(username.Trim()))
            {
                return "Korisnicko ime moze sadrzavati samo slova i brojeve, bez razmaka i specijalnih znakova.";
            }

            return null;
        }

        private static string? ValidateRequiredFields(RegisterRequestDto request)
        {
            var profileValidationMessage = ValidateProfileFields(request.ImePrezime, request.Email, request.Username);
            if (profileValidationMessage is not null)
            {
                return profileValidationMessage;
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
