using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public int PasswordResetTokenID { get; set; }

        [Required]
        public int KorisnikID { get; set; }

        [Required, StringLength(128)]
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }

        public Korisnik Korisnik { get; set; } = null!;
    }
}
