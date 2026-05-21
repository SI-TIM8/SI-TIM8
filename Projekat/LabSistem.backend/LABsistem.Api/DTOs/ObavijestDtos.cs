namespace LABsistem.Application.DTOs
{
    public class ObavijestDTO
    {
        public int ID { get; set; }
        public string Novosti { get; set; } = default!;
        public bool Dostupnost { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int? TerminID { get; set; }
    }

    public class OdgovorNaZahtjevDTO
    {
        public int StudentID { get; set; }
        public int TerminID { get; set; }
        public string StudentImePrezime { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public bool StudentEmailVerified { get; set; }
        public DateTime DatumTermina { get; set; }
        public TimeSpan VrijemePocetka { get; set; }
    }
}
