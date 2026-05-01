using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class KorisnikObjekat
    {
        [Key]
        public int ID { get; set; } 
        public int KorisnikID { get; set; }
        public Korisnik Korisnik { get; set; }
        public int ObjekatID { get; set; }
        public Objekat Objekat { get; set; }
    }
}
