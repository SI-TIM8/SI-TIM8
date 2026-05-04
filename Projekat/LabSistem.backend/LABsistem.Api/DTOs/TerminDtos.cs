namespace LABsistem.Application.DTOs
{
    using System;

    public class TerminCreateDTO
    {
        public TimeSpan VrijemePocetka { get; set; }
        public TimeSpan VrijemeKraja { get; set; }
        public DateTime Datum { get; set; }
        public int KreatorID { get; set; }
        public int KabinetID { get; set; }
    }

    public class TerminDTO
    {
        public int ID { get; set; }
        public TimeSpan VrijemePocetka { get; set; }
        public TimeSpan VrijemeKraja { get; set; }
        public DateTime Datum { get; set; }
        public int KreatorID { get; set; }
        public string KreatorIme { get; set; } = default!;
        public int KabinetID { get; set; }
        public string KabinetNaziv { get; set; } = default!;
    }
}
