using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Evidencija
    {
        [Key]
        public int ID { get; set; }
        

        [StringLength(20)]
        public string Status { get; set; }
        

        [StringLength(20)]
        public string Komentar { get; set; }
        

        // Foreign Keys
        public int OpremaID { get; set; }
        
        public Oprema Oprema { get; set; }

        public int KorisnikID { get; set; }
        
        public Korisnik Korisnik { get; set; }
    }
}
