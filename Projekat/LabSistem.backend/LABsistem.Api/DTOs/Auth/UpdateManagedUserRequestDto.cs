using LABsistem.Domain.Enums;

namespace LABsistem.Application.DTOs.Auth
{
    public class UpdateManagedUserRequestDto
    {
        public string ImePrezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public UlogaKorisnika Uloga { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
