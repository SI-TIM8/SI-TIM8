namespace LABsistem.Application.DTOs
{
    public class OpremaCreateDTO
    {
        public string Naziv { get; set; } = default!;
        public int SerijskiBroj { get; set; }
        public int Stanje { get; set; }
        public int KabinetID { get; set; }
        public int KreatorID { get; set; }
    }

    public class OpremaDTO
    {
        public int ID { get; set; }
        public string Naziv { get; set; } = default!;
        public int SerijskiBroj { get; set; }
        public int Stanje { get; set; }
        public int KabinetID { get; set; }
        public string KabinetNaziv { get; set; } = default!;
        public string ZgradaNaziv { get; set; } = "N/A";
        public int KreatorID { get; set; }
    }
}