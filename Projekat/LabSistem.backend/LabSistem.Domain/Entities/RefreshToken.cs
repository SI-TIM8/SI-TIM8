using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int KorisnikID { get; set; }

        [Required, StringLength(128)]
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime LastUsedAtUtc { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        [StringLength(128)]
        public string? ReplacedByTokenHash { get; set; }

        [StringLength(256)]
        public string? DeviceInfo { get; set; }

        [StringLength(64)]
        public string? IpAddress { get; set; }

        public Korisnik Korisnik { get; set; } = null!;
    }
}
