using System.Net.Mail;
using System.Text.RegularExpressions;
using LABsistem.Dal.Db;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Application.Validators
{
    public class AuthBusinessRules
    {
        private static readonly Regex UsernameRegex = new("^[A-Za-z0-9]+$", RegexOptions.Compiled);
        private static readonly Regex FullNameRegex = new(@"^\p{L}+(?:\s+\p{L}+)*$", RegexOptions.Compiled);
        private readonly LabSistemDbContext _dbContext;

        public AuthBusinessRules(LabSistemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string? ValidateProfileFields(string imePrezime, string email, string username)
        {
            var normalizedFullName = imePrezime?.Trim() ?? string.Empty;
            var normalizedEmail = email?.Trim() ?? string.Empty;
            var normalizedUsername = username?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedFullName))
            {
                return "Ime i prezime je obavezno.";
            }

            if (normalizedFullName.Length < 2)
            {
                return "Ime i prezime mora imati najmanje 2 karaktera.";
            }

            if (normalizedFullName.Length > 100)
            {
                return "Ime i prezime moze imati najvise 100 karaktera.";
            }

            if (!FullNameRegex.IsMatch(normalizedFullName))
            {
                return "Ime i prezime moze sadrzavati samo slova i razmake.";
            }

            if (string.IsNullOrWhiteSpace(normalizedEmail))
            {
                return "Email je obavezan.";
            }

            if (normalizedEmail.Length < 5)
            {
                return "Email mora imati najmanje 5 karaktera.";
            }

            if (normalizedEmail.Length > 254)
            {
                return "Email moze imati najvise 254 karaktera.";
            }

            if (!IsEmailValid(normalizedEmail))
            {
                return "Email nije ispravan.";
            }

            if (string.IsNullOrWhiteSpace(normalizedUsername))
            {
                return "Username je obavezan.";
            }

            if (normalizedUsername.Length < 3)
            {
                return "Korisnicko ime mora imati najmanje 3 karaktera.";
            }

            if (normalizedUsername.Length > 30)
            {
                return "Korisnicko ime moze imati najvise 30 karaktera.";
            }

            if (!UsernameRegex.IsMatch(normalizedUsername))
            {
                return "Korisnicko ime moze sadrzavati samo slova i brojeve, bez razmaka i specijalnih znakova.";
            }

            return null;
        }

        public string? ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Lozinka je obavezna.";
            }

            var normalizedPassword = password.Trim();
            if (normalizedPassword.Length < 8)
            {
                return "Lozinka mora imati najmanje 8 znakova.";
            }

            if (normalizedPassword.Length > 64)
            {
                return "Lozinka moze imati najvise 64 karaktera.";
            }

            return null;
        }

        public async Task<string?> CheckIfUsernameTakenAsync(string username, int? excludeUserId = null)
        {
            var normalizedUsername = username.Trim();
            var exists = excludeUserId.HasValue
                ? await _dbContext.Korisnici.AnyAsync(x => x.ID != excludeUserId.Value && x.Username == normalizedUsername)
                : await _dbContext.Korisnici.AnyAsync(x => x.Username == normalizedUsername);

            return exists ? "Username je vec zauzet." : null;
        }

        public async Task<string?> CheckIfEmailTakenAsync(string email, int? excludeUserId = null)
        {
            var normalizedEmail = email.Trim();
            var exists = excludeUserId.HasValue
                ? await _dbContext.Korisnici.AnyAsync(x => x.ID != excludeUserId.Value && x.Email == normalizedEmail)
                : await _dbContext.Korisnici.AnyAsync(x => x.Email == normalizedEmail);

            return exists ? "Email je vec zauzet." : null;
        }

        private static bool IsEmailValid(string email)
        {
            try
            {
                var parsedEmail = new MailAddress(email);
                return parsedEmail.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
