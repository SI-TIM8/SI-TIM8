namespace LABSistem.Bll.DTOs
{
    public class OpremaCreateDTO
    {
        public string Naziv { get; set; } = default!;
        public int SerijskiBroj { get; set; } // Promijenjeno u int
        public int Stanje { get; set; }
        public int KabinetID { get; set; }
        public int KreatorID { get; set; }
    }

    public class OpremaDTO
    {
        public int ID { get; set; }
        public string Naziv { get; set; } = default!;
        public int SerijskiBroj { get; set; } // Promijenjeno u int
    }
}