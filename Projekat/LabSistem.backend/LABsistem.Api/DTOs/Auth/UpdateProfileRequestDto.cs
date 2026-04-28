namespace LABsistem.Bll.DTOs.Auth
{
    public class UpdateProfileRequestDto
    {
        public string ImePrezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
