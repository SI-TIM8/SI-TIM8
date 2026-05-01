using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Recenzija
    {
        [Key]
        public int ID { get; set; }
        

        [StringLength(100)]
        public string Komentar { get; set; }
        

        public int Ocjena { get; set; }
        

        public ICollection<OpremaRecenzija> OpremaRecenzije { get; set; }
        
    }
}
