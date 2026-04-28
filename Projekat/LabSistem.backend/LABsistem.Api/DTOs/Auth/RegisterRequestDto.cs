namespace LABsistem.Bll.DTOs.Auth
{
    /// <summary>
    /// DTO za registraciju novog korisnika
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Ime i prezime korisnika
        /// </summary>
        public string ImePrezime { get; set; } = string.Empty;

        /// <summary>
        /// Email adresa korisnika
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Korisničko ime (jedinstveno)
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Lozinka korisnika
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
