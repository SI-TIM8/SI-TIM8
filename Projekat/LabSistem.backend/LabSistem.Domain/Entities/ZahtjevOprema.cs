using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    /// <summary>
    /// Join tabela koja povezuje Zahtjev sa Opremom.
    /// Omogućava studentu da odabere više opreme za jednu rezervaciju.
    /// </summary>
    public class ZahtjevOprema
    {
        [Key]
        public int ID { get; set; }

        // Kompostovani ključ za osiguravanje jedinstvene kombinacije
        public int ZahtjevID { get; set; }
        public Zahtjev Zahtjev { get; set; }

        public int OpremaID { get; set; }
        public Oprema Oprema { get; set; }
    }
}
