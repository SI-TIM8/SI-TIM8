namespace LABsistem.Bll.DTOs.Korisnik
{
    /// <summary>
    /// DTO za prikaz korisnika u listi ili detaljima
    /// </summary>
    public class GetKorisnikDto
    {
        /// <summary>
        /// Jedinstveni identifikator korisnika
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ime i prezime korisnika
        /// </summary>
        public string ImePrezime { get; set; } = string.Empty;

        /// <summary>
        /// Email adresa korisnika
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Korisničko ime
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Uloga korisnika (Admin, Profesor, Student, Tehnicar)
        /// </summary>
        public string Uloga { get; set; } = string.Empty;

        /// <summary>
        /// Datum kreiranja korisnika
        /// </summary>
        public DateTime? DatumKreiranja { get; set; }
    }

    /// <summary>
    /// DTO za ažuriranje korisničkog profila
    /// </summary>
    public class UpdateKorisnikDto
    {
        /// <summary>
        /// Novo ime i prezime
        /// </summary>
        public string? ImePrezime { get; set; }

        /// <summary>
        /// Nova email adresa
        /// </summary>
        public string? Email { get; set; }
    }

    /// <summary>
    /// DTO za promjenu lozinke
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Stara lozinka
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Nova lozinka
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Potvrda nove lozinke
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
