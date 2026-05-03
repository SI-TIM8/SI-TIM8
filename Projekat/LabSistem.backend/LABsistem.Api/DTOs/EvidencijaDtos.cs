namespace LABsistem.Bll.DTOs
{
    public class EvidencijaCreateDTO
    {
        public int OpremaID { get; set; }
        public int KorisnikID { get; set; }
        public string Status { get; set; } = "Kvar";
        public string Komentar { get; set; } = default!;
    }

    public class EvidencijaDTO
    {
        public int ID { get; set; }
        public string Status { get; set; } = default!;
        public string Komentar { get; set; } = default!;
        public int OpremaID { get; set; }
        public string OpremaNaziv { get; set; } = default!;
        public int KorisnikID { get; set; }
        public string KorisnikImePrezime { get; set; } = default!;
    }

    public class EvidencijaUpdateDTO
    {
        public string Status { get; set; } = default!;
    }
}
