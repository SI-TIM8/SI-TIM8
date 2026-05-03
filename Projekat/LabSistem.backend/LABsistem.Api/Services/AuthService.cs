using System.Security.Claims;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Application.Models;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using LABsistem.Application.Validators;

namespace LABsistem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly LabSistemDbContext _dbContext;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly AuthBusinessRules _businessRules;

        public AuthService(LabSistemDbContext dbContext, IJwtService jwtService, JwtSettings jwtSettings, AuthBusinessRules businessRules)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings;
            _businessRules = businessRules;
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

            if (korisnik is null || korisnik.DeactivatedAt.HasValue)
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

            var now = DateTime.UtcNow;
            var refreshTokenHash = HashToken(refreshToken);
            var useTransactionalRefreshRotation = _dbContext.Database.IsRelational();

            if (useTransactionalRefreshRotation)
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                var refreshResult = await TryRefreshSessionAsync(refreshTokenHash, now, ipAddress, deviceInfo);
                if (refreshResult is null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();
                return refreshResult;
            }

            return await TryRefreshSessionAsync(refreshTokenHash, now, ipAddress, deviceInfo);
        }

        private async Task<LoginResponseDto?> TryRefreshSessionAsync(
            string refreshTokenHash,
            DateTime now,
            string? ipAddress,
            string? deviceInfo)
        {
            var trimmedIpAddress = TrimToLength(ipAddress, 64);
            var trimmedDeviceInfo = TrimToLength(deviceInfo, 256);

            var existingRefreshToken = await _dbContext.RefreshTokens
                .Include(x => x.Korisnik)
                .FirstOrDefaultAsync(x => x.TokenHash == refreshTokenHash);

            if (existingRefreshToken is null ||
                existingRefreshToken.RevokedAtUtc.HasValue ||
                existingRefreshToken.ExpiresAtUtc <= now)
            {
                return null;
            }

            if (existingRefreshToken.Korisnik.DeactivatedAt.HasValue)
            {
                await RevokeRefreshTokenRecordAsync(existingRefreshToken.ID, now);
                return null;
            }

            var accessToken = _jwtService.GenerateToken(
                existingRefreshToken.Korisnik.ID.ToString(),
                existingRefreshToken.Korisnik.Username,
                existingRefreshToken.Korisnik.Uloga.ToString());
            var accessTokenExpiresAtUtc = _jwtService.GetTokenExpirationUtc(accessToken);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var newRefreshTokenExpiresAtUtc = now.AddDays(_jwtSettings.RefreshExpireDays);
            var newRefreshTokenHash = HashToken(newRefreshToken);

            var revoked = await RevokeRefreshTokenRecordAsync(existingRefreshToken.ID, now, newRefreshTokenHash);
            if (!revoked)
            {
                return null;
            }

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                KorisnikID = existingRefreshToken.Korisnik.ID,
                TokenHash = newRefreshTokenHash,
                CreatedAtUtc = now,
                ExpiresAtUtc = newRefreshTokenExpiresAtUtc,
                LastUsedAtUtc = now,
                DeviceInfo = trimmedDeviceInfo,
                IpAddress = trimmedIpAddress
            });

            await _dbContext.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = accessToken,
                AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAtUtc = newRefreshTokenExpiresAtUtc,
                UserId = existingRefreshToken.Korisnik.ID,
                Username = existingRefreshToken.Korisnik.Username,
                Role = existingRefreshToken.Korisnik.Uloga.ToString()
            };
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

        private async Task<bool> RevokeRefreshTokenRecordAsync(int refreshTokenId, DateTime now, string? replacedByTokenHash = null)
        {
            if (_dbContext.Database.IsRelational())
            {
                var query = _dbContext.RefreshTokens
                    .Where(x => x.ID == refreshTokenId && !x.RevokedAtUtc.HasValue);

                var affectedRows = string.IsNullOrWhiteSpace(replacedByTokenHash)
                    ? await query.ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.LastUsedAtUtc, now)
                        .SetProperty(x => x.RevokedAtUtc, now))
                    : await query.ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.LastUsedAtUtc, now)
                        .SetProperty(x => x.RevokedAtUtc, now)
                        .SetProperty(x => x.ReplacedByTokenHash, replacedByTokenHash));

                return affectedRows == 1;
            }

            var trackedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.ID == refreshTokenId);
            if (trackedRefreshToken is null || trackedRefreshToken.RevokedAtUtc.HasValue)
            {
                return false;
            }

            trackedRefreshToken.LastUsedAtUtc = now;
            trackedRefreshToken.RevokedAtUtc = now;

            if (!string.IsNullOrWhiteSpace(replacedByTokenHash))
            {
                trackedRefreshToken.ReplacedByTokenHash = replacedByTokenHash;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserActiveAsync(int userId)
        {
            return await _dbContext.Korisnici
                .Where(x => x.ID == userId)
                .Select(x => x.DeactivatedAt == null)
                .FirstOrDefaultAsync();
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
                    Role = x.Uloga.ToString(),
                    DeactivatedAt = x.DeactivatedAt,
                    Status = x.DeactivatedAt == null ? "Aktivan" : "Deaktiviran"
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
                    Role = x.Uloga.ToString(),
                    DeactivatedAt = x.DeactivatedAt,
                    Status = x.DeactivatedAt == null ? "Aktivan" : "Deaktiviran"
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

            var validationMessage = _businessRules.ValidateProfileFields(request.ImePrezime, request.Email, request.Username);
            if (validationMessage is not null)
            {
                return (false, validationMessage, null);
            }

            var usernameTakenMessage = await _businessRules.CheckIfUsernameTakenAsync(request.Username, userId);
            if (usernameTakenMessage is not null)
            {
                return (false, usernameTakenMessage, null);
            }

            var emailTakenMessage = await _businessRules.CheckIfEmailTakenAsync(request.Email, userId);
            if (emailTakenMessage is not null)
            {
                return (false, emailTakenMessage, null);
            }

            korisnik.ImePrezime = request.ImePrezime.Trim();
            korisnik.Email = request.Email.Trim();
            korisnik.Username = request.Username.Trim();

            await _dbContext.SaveChangesAsync();

            var updatedProfile = await GetProfileAsync(userId);
            return (true, "Profil je uspjesno azuriran.", updatedProfile);
        }

        public async Task<(bool Success, string Message, UserListItemDto? User)> UpdateUserAsync(int currentUserId, int targetUserId, UpdateManagedUserRequestDto request)
        {
            if (currentUserId == targetUserId)
            {
                return (false, "Ne mozete uredjivati vlastiti nalog kroz ovaj panel.", null);
            }

            var korisnik = await _dbContext.Korisnici.FirstOrDefaultAsync(x => x.ID == targetUserId);
            if (korisnik is null)
            {
                return (false, "Korisnik nije pronadjen.", null);
            }

            var validationMessage = _businessRules.ValidateProfileFields(request.ImePrezime, request.Email, request.Username);
            if (validationMessage is not null)
            {
                return (false, validationMessage, null);
            }

            var usernameTakenMessage = await _businessRules.CheckIfUsernameTakenAsync(request.Username, targetUserId);
            if (usernameTakenMessage is not null)
            {
                return (false, usernameTakenMessage, null);
            }

            var emailTakenMessage = await _businessRules.CheckIfEmailTakenAsync(request.Email, targetUserId);
            if (emailTakenMessage is not null)
            {
                return (false, emailTakenMessage, null);
            }

            korisnik.ImePrezime = request.ImePrezime.Trim();
            korisnik.Email = request.Email.Trim();
            korisnik.Username = request.Username.Trim();
            korisnik.Uloga = request.Uloga;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                var passwordValidationMessage = _businessRules.ValidatePassword(request.NewPassword);
                if (passwordValidationMessage is not null)
                {
                    return (false, passwordValidationMessage, null);
                }

                korisnik.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }

            await _dbContext.SaveChangesAsync();

            return (true, "Korisnik je uspjesno azuriran.", MapUserListItem(korisnik));
        }

        public async Task<(bool Success, string Message, UserListItemDto? User)> ActivateUserAsync(int currentUserId, int targetUserId)
        {
            if (currentUserId == targetUserId)
            {
                return (false, "Ne mozete aktivirati vlastiti nalog kroz ovaj panel.", null);
            }

            var korisnik = await _dbContext.Korisnici.FirstOrDefaultAsync(x => x.ID == targetUserId);
            if (korisnik is null)
            {
                return (false, "Korisnik nije pronadjen.", null);
            }

            if (!korisnik.DeactivatedAt.HasValue)
            {
                return (false, "Korisnik je vec aktivan.", null);
            }

            korisnik.DeactivatedAt = null;

            await _dbContext.SaveChangesAsync();

            return (true, "Korisnik je uspjesno aktiviran.", MapUserListItem(korisnik));
        }

        public async Task<(bool Success, string Message, UserListItemDto? User)> DeactivateUserAsync(int currentUserId, int targetUserId)
        {
            if (currentUserId == targetUserId)
            {
                return (false, "Ne mozete deaktivirati svoj nalog.", null);
            }

            var korisnik = await _dbContext.Korisnici
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.ID == targetUserId);

            if (korisnik is null)
            {
                return (false, "Korisnik nije pronadjen.", null);
            }

            if (korisnik.DeactivatedAt.HasValue)
            {
                return (false, "Korisnik je vec deaktiviran.", null);
            }

            if (korisnik.Uloga == UlogaKorisnika.Admin)
            {
                return (false, "Prvo uklonite administratorsku ulogu prije deaktivacije korisnika.", null);
            }

            korisnik.DeactivatedAt = DateTime.UtcNow;
            RevokeUserRefreshTokens(korisnik.RefreshTokens);

            await _dbContext.SaveChangesAsync();

            return (true, "Korisnik je uspjesno deaktiviran.", MapUserListItem(korisnik));
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

            var passwordValidationMessage = _businessRules.ValidatePassword(request.NewPassword);
            if (passwordValidationMessage is not null)
            {
                return (false, passwordValidationMessage);
            }

            korisnik.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _dbContext.SaveChangesAsync();

            return (true, "Lozinka je uspjesno promijenjena.");
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga)
        {
            var validationMessage = _businessRules.ValidateProfileFields(request.ImePrezime, request.Email, request.Username);
            if (validationMessage is not null)
            {
                return (false, validationMessage);
            }

            var passwordValidationMessage = _businessRules.ValidatePassword(request.Password);
            if (passwordValidationMessage is not null)
            {
                return (false, passwordValidationMessage);
            }

            var usernameTakenMessage = await _businessRules.CheckIfUsernameTakenAsync(request.Username);
            if (usernameTakenMessage is not null)
            {
                return (false, usernameTakenMessage);
            }

            var emailTakenMessage = await _businessRules.CheckIfEmailTakenAsync(request.Email);
            if (emailTakenMessage is not null)
            {
                return (false, emailTakenMessage);
            }

            var noviKorisnik = new Korisnik
            {
                ImePrezime = request.ImePrezime.Trim(),
                Email = request.Email.Trim(),
                Username = request.Username.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Uloga = uloga,
                DeactivatedAt = null
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

        private void RevokeUserRefreshTokens(IEnumerable<RefreshToken> refreshTokens)
        {
            foreach (var refreshToken in refreshTokens.Where(x => !x.RevokedAtUtc.HasValue))
            {
                refreshToken.RevokedAtUtc = DateTime.UtcNow;
                refreshToken.LastUsedAtUtc = DateTime.UtcNow;
            }
        }

        private static UserListItemDto MapUserListItem(Korisnik korisnik)
        {
            return new UserListItemDto
            {
                UserId = korisnik.ID,
                ImePrezime = korisnik.ImePrezime,
                Email = korisnik.Email,
                Username = korisnik.Username,
                Role = korisnik.Uloga.ToString(),
                DeactivatedAt = korisnik.DeactivatedAt,
                Status = korisnik.DeactivatedAt == null ? "Aktivan" : "Deaktiviran"
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

    }
}
