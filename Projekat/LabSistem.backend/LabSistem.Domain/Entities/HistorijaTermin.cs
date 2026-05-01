using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class HistorijaTermin
    {
        [Key]
        public int ID { get; set; }
        
        public int HistorijaID { get; set; }
        
        public int TerminID { get; set; }
        
    }
}
