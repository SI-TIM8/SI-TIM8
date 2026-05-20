namespace LABsistem.Application.DTOs
{
    public class EvidencijaCreateDTO
    {
        public int OpremaID { get; set; }
        public int KorisnikID { get; set; }
        public int? TerminID { get; set; }
        public string Status { get; set; } = "Kvar";
        public string Komentar { get; set; } = default!;
    }

    public class EvidencijaDTO
    {
        public int ID { get; set; }
        public string Status { get; set; } = default!;
        public string Komentar { get; set; } = default!;
        public string? Rjesenje { get; set; }
        public DateTime PrijavljenoU { get; set; }
        public DateTime? RijesenoU { get; set; }
        public int OpremaID { get; set; }
        public string OpremaNaziv { get; set; } = default!;
        public string? OpremaKategorija { get; set; }
        public int? OpremaSerijskiBroj { get; set; }
        public int? OpremaStanje { get; set; }
        public int? OpremaKabinetID { get; set; }
        public string? OpremaKabinetNaziv { get; set; }
        public string? OpremaZgradaNaziv { get; set; }
        public int KorisnikID { get; set; }
        public string KorisnikImePrezime { get; set; } = default!;
        public int? TerminID { get; set; }
        public DateTime? TerminDatum { get; set; }
        public TimeSpan? TerminVrijemePocetka { get; set; }
        public TimeSpan? TerminVrijemeKraja { get; set; }
        public int? ProfesorID { get; set; }
        public string? ProfesorImePrezime { get; set; }
        public int? ObradioKorisnikID { get; set; }
        public string? ObradioImePrezime { get; set; }
    }

    public class EvidencijaUpdateDTO
    {
        public string Status { get; set; } = default!;
        public string? Rjesenje { get; set; }
    }
}
