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

        public DateTime PrijavljenoU { get; set; }

        public DateTime? RijesenoU { get; set; }
        

        [StringLength(20)]
        public string Status { get; set; }
        

        [StringLength(500)]
        public string Komentar { get; set; }
        

        // Foreign Keys
        public int OpremaID { get; set; }
        
        public Oprema Oprema { get; set; }

        public int? TerminID { get; set; }

        public Termin? Termin { get; set; }

        public int? ProfesorID { get; set; }

        public Korisnik? Profesor { get; set; }

        public int? ObradioKorisnikID { get; set; }

        public Korisnik? ObradioKorisnik { get; set; }

        public int KorisnikID { get; set; }
        
        public Korisnik Korisnik { get; set; }

        [StringLength(500)]
        public string? Rjesenje { get; set; }
    }
}
