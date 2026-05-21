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

    public class StudentZahtjevDTO
    {
        public int ID { get; set; }
        public int TerminID { get; set; }
        public string KabinetNaziv { get; set; } = default!;
        public DateTime Datum { get; set; }
        public TimeSpan VrijemePocetka { get; set; }
        public TimeSpan VrijemeKraja { get; set; }
        public string ProfesorIme { get; set; } = default!;
        public string StatusZahtjeva { get; set; } = default!;
        public bool MozeOtkazati { get; set; }
    }
}
