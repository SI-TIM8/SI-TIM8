using LABsistem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Oprema
    {
        [Key]
        public int ID { get; set; } 
        [Required, StringLength(30)]
        public string Naziv { get; set; } 
        [Required, StringLength(40)]
        public string Kategorija { get; set; }
        public int SerijskiBroj { get; set; } 

        
        public StatusOpreme stanje { get; set; } 

        // FK veze
        public int KreatorID { get; set; } 
        public int KabinetID { get; set; } 

        public ICollection<Evidencija> Evidencije { get; set; }

        // Many-to-Many veža sa Zahtjevom kroz ZahtjevOprema
        public ICollection<ZahtjevOprema> Zahtjevi { get; set; } = new List<ZahtjevOprema>();
    }
}
