using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Kabinet
    {
        [Key]
        public int ID { get; set; }
        [Required, StringLength(20)]
        public string Naziv { get; set; }

        // Relacija: Korisnik (1:N) [Izvor: ERD slika, source 46]
        public int KorisnikID { get; set; }
        public Korisnik OdgovorniKorisnik { get; set; }

        // Relacija: Objekat (1:N) [Izvor: ERD slika, source 50]
        public int ObjekatID { get; set; }
        public Objekat MaticniObjekat { get; set; }

        // Kolekcije za navigaciju
        public ICollection<Termin> Termini { get; set; }
        public ICollection<Oprema> Oprema { get; set; }
    }
}
