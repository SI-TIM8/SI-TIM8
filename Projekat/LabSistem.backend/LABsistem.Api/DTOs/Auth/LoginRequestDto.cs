namespace LABsistem.Bll.DTOs.Auth
{
    /// <summary>
    /// DTO za login zahtjev korisnika
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Korisničko ime za prijavu
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Lozinka za prijavu
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
