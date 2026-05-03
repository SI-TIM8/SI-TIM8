using System;

namespace LABsistem.Application.DTOs.Auth
{
    public class UserListItemDto
    {
        public int UserId { get; set; }
        public string ImePrezime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? DeactivatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
