using System.Text.RegularExpressions;

namespace LABsistem.Presentation.Helpers
{
    public static class InputValidator
    {
        public static string? ValidirajUsername(string username)
        {
            if (username.Length < 3 || username.Length > 20)
                return "Username mora imati između 3 i 20 znakova.";

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9._]+$"))
                return "Username smije sadržavati samo slova, brojeve, tačku i donju crtu.";

            return null;
        }

        public static string? ValidirajPassword(string password)
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
