using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class OpremaRecenzija
    {
        [Key]
        public int ID { get; set; }
        

        public int RecenzijaID { get; set; }
        
        public Recenzija Recenzija { get; set; }

        public int OpremaID { get; set; }
        
        public Oprema Oprema { get; set; }
    }
}
