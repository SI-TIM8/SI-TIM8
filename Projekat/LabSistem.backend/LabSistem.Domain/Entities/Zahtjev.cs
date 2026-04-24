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
        

        public bool Status { get; set; }
        

        [StringLength(200)]
        public string Komentar { get; set; }
        

        // Foreign Keys
        public int TerminID { get; set; }
        
        public Termin Termin { get; set; }

        public int KabinetID { get; set; }
        
        public Kabinet Kabinet { get; set; }
    }
}
