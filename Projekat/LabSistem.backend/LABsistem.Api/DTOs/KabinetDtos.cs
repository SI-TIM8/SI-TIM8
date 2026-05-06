namespace LABsistem.Application.DTOs
{
    public class KabinetCreateDTO
    {
        public string Naziv { get; set; } = default!;
        public int KorisnikID { get; set; }
        public int ObjekatID { get; set; }
    }

    public class KabinetDTO
    {
        public int ID { get; set; }
        public string Naziv { get; set; } = default!;
        public int KorisnikID { get; set; }
        public string OdgovorniKorisnik { get; set; } = default!;
        public int ObjekatID { get; set; }
        public string ObjekatLokacija { get; set; } = default!;
    }
}