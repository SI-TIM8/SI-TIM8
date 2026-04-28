namespace LABsistem.Bll.DTOs.Kabinet
{
    /// <summary>
    /// DTO za prikaz kabineta
    /// </summary>
    public class GetKabinetDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public int ObjekatId { get; set; }
        public string ObjekatNaziv { get; set; } = string.Empty;
        public int? Kapacitet { get; set; }
        public string? RadnoVrijeme { get; set; }
        public int? OdgovorniKorisnikId { get; set; }
        public string? OdgovorniKorisnikIme { get; set; }
    }

    /// <summary>
    /// DTO za kreiranje novog kabineta
    /// </summary>
    public class CreateKabinetDto
    {
        public string Naziv { get; set; } = string.Empty;
        public int ObjekatId { get; set; }
        public int? Kapacitet { get; set; }
        public int? OdgovorniKorisnikId { get; set; }
    }

    /// <summary>
    /// DTO za ažuriranje kabineta
    /// </summary>
    public class UpdateKabinetDto
    {
        public string? Naziv { get; set; }
        public int? Kapacitet { get; set; }
        public int? OdgovorniKorisnikId { get; set; }
    }

    /// <summary>
    /// DTO za blokiranje perioda u kabinetu
    /// </summary>
    public class BlockPeriodDto
    {
        public string DatumPocetak { get; set; } = string.Empty;
        public string DatumKraj { get; set; } = string.Empty;
        public string? Razlog { get; set; }
    }
}
