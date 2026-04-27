using LABsistem.Domain.Enums;

namespace LABsistem.Presentation.DTOs
{
    public class CreateKorisnikDto
    {
        public string ImePrezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UlogaKorisnika Uloga { get; set; }
    }
}
