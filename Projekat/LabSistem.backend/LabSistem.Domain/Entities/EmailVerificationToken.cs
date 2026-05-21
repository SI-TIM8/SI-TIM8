using System;
using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    public class EmailVerificationToken
    {
        [Key]
        public int EmailVerificationTokenID { get; set; }

        public int KorisnikID { get; set; }

        [Required, StringLength(254, MinimumLength = 5)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(128)]
        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }

        public Korisnik Korisnik { get; set; } = null!;
    }
}
