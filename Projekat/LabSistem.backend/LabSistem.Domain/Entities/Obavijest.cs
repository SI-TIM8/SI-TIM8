using System;
using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    public class Obavijest
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Novosti { get; set; }

        public bool Dostupnost { get; set; } = false;

        public DateTime DatumKreiranja { get; set; } = DateTime.UtcNow;

        // Kome ide notifikacija
        public int KorisnikID { get; set; }
        public Korisnik Korisnik { get; set; }

        // Opcionalno vezana za termin
        public int? TerminID { get; set; }
        public Termin Termin { get; set; }
    }
}