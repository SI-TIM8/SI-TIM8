using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using LABsistem.Domain.Enums;

namespace LABsistem.Domain.Entities
{
    
    public class Korisnik
    {
        [Key]
        public int ID { get; set; }
        

        [Required, StringLength(100)]
        public string ImePrezime { get; set; }
       

        [Required, StringLength(100)]
        public string Email { get; set; }
        

        [Required, StringLength(50)]
        public string Username { get; set; }
        

        [Required, StringLength(100)]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? DeactivatedAt { get; set; }


        // Za RBAC (US30) 
        //[Required]
        public UlogaKorisnika Uloga { get; set; }

        // Relacije
        public ICollection<Kabinet> Kabineti { get; set; }
        
        public ICollection<Termin> KreiraniTermini { get; set; }

        public ICollection<Evidencija> Evidencije { get; set; }

        public ICollection<KorisnikObjekat> KorisnikObjekti { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }


    }
}
