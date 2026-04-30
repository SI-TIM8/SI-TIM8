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
        

        [Required, StringLength(100, MinimumLength = 2)]
        public string ImePrezime { get; set; }
       

        [Required, StringLength(254, MinimumLength = 5)]
        public string Email { get; set; }
        

        [Required, StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }
        

        [Required, StringLength(64, MinimumLength = 8)]
        public string Password { get; set; }

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
