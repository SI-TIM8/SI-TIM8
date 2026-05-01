using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Termin
    {
        [Key]
        public int ID { get; set; } 
        public TimeSpan VrijemePocetka { get; set; } 
        public TimeSpan VrijemeKraja { get; set; } 
        public DateTime Datum { get; set; } 
      
        public int KreatorID { get; set; }
        public Korisnik Kreator { get; set; }

       
        public int KabinetID { get; set; }
        public Kabinet Kabinet { get; set; }

        public ICollection<Zahtjev> Zahtjevi { get; set; }
    }
}
