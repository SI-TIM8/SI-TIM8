namespace LABsistem.Bll.DTOs.Auth
{
    /// <summary>
    /// DTO za login odgovor sa JWT tokenom i korisničkim informacijama
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT token za autentifikaciju
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// ID prijavljenog korisnika
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Korisničko ime
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Uloga korisnika (Admin, Profesor, Student, Tehnicar)
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
