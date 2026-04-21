using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Kabinet
    {
        public int KabinetId { get; set; }
        
        public string KabinetName { get; set; }

        public string KabinetDescription { get; set; }

        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        
    }
}
