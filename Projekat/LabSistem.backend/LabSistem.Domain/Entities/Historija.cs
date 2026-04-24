using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Historija
    {
        [Key]
        public int ID { get; set; }
        
        public DateTime Datum { get; set; }
        

        public int TerminID { get; set; }
        
    }
}
