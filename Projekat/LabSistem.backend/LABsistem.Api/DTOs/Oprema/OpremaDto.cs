namespace LABsistem.Bll.DTOs.Oprema
{
    /// <summary>
    /// DTO za prikaz opreme
    /// </summary>
    public class GetOpremaDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public int SerijskiBroj { get; set; }
        public int KabinetId { get; set; }
        public string KabinetNaziv { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DatumKreiranja { get; set; }
    }

    /// <summary>
    /// DTO za kreiranje nove opreme
    /// </summary>
    public class CreateOpremaDto
    {
        public string Naziv { get; set; } = string.Empty;
        public int SerijskiBroj { get; set; }
        public int KabinetId { get; set; }
        public string Status { get; set; } = "Ispravno";
    }

    /// <summary>
    /// DTO za ažuriranje opreme
    /// </summary>
    public class UpdateOpremaDto
    {
        public string? Naziv { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO za prijavu kvara
    /// </summary>
    public class PrijavaKvaraDto
    {
        public string Opis { get; set; } = string.Empty;
        public string? Hitnost { get; set; }
    }
}
