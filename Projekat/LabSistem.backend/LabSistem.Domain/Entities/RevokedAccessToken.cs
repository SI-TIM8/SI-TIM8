using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    public class RevokedAccessToken
    {
        [Key]
        public int ID { get; set; }

        [Required, StringLength(128)]
        public string Jti { get; set; } = string.Empty;

        public DateTime RevokedAtUtc { get; set; }

        public DateTime ExpiresAtUtc { get; set; }
    }
}
