namespace LABsistem.Application.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ZahtjevDTO
    {
        public int ID { get; set; }
        public int TerminID { get; set; }
        public string KabinetNaziv { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan VrijemePocetka { get; set; }
        public TimeSpan VrijemeKraja { get; set; }
        public string StudentIme { get; set; }
        public string StatusZahtjeva { get; set; }
        public DateTime KreiranoU { get; set; }
    }

    public class PosaljiZahtjevDTO
    {
        public List<int>? OpremaIds { get; set; }
    }
}
