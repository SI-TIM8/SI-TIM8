using LABsistem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Zahtjev
    {
        [Key]
        public int ID { get; set; }
        

        

        [StringLength(200)]
        public string Komentar { get; set; }
        

      


        public int StudentID { get; set; }

        public Korisnik Student { get; set; }


        // Foreign Keys
        public int TerminID { get; set; }

        public Termin Termin { get; set; }

        public StatusZahtjeva StatusZahtjeva { get; set; }

        public ICollection<ReservationReminderDispatch> ReservationReminderDispatches { get; set; } = [];
    }
}
