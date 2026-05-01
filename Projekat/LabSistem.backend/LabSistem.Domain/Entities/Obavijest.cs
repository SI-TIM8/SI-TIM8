using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Obavijest
    {
        [Key]
        public int ID { get; set; }
       

        [Required, StringLength(100)]
        public string Novosti { get; set; }
       

        public bool Dostupnost { get; set; }
       

        // Foreign Key
        public int TerminID { get; set; }
        
        public Termin Termin { get; set; }
    }
}
