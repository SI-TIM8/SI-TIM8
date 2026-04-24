using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LABsistem.Domain.Entities
{
    public class Objekat
    {
        [Key]
        public int ID { get; set; }
        

        [Required, StringLength(20)]
        public string Lokacija { get; set; }
        

        [Required, StringLength(20)]
        public string RadnoVrijeme { get; set; }
      

        // Relacije
        public ICollection<Kabinet> Kabineti { get; set; }
        
        public ICollection<KorisnikObjekat> KorisnikObjekti { get; set; }
        
    }


}
