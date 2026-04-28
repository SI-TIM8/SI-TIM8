namespace LABsistem.Bll.DTOs.Zahtjev
{
    /// <summary>
    /// DTO za prikaz zahtjeva
    /// </summary>
    public class GetZahtjevDto
    {
        public int Id { get; set; }
        public int TerminId { get; set; }
        public string KabinetNaziv { get; set; } = string.Empty;
        public string Datum { get; set; } = string.Empty;
        public string Vrijeme { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Komentar { get; set; }
        public DateTime? DatumKreiranja { get; set; }
    }

    /// <summary>
    /// DTO za kreiranje novog zahtjeva
    /// </summary>
    public class CreateZahtjevDto
    {
        public int TerminId { get; set; }
        public bool OpremaSveZaTermin { get; set; }
        public List<int> OdabranaOpremaIds { get; set; } = new();
        public string? Napomena { get; set; }
    }

    /// <summary>
    /// DTO za odobravanje zahtjeva
    /// </summary>
    public class ApproveZahtjevDto
    {
        public string? Komentar { get; set; }
    }

    /// <summary>
    /// DTO za odbijanje zahtjeva
    /// </summary>
    public class RejectZahtjevDto
    {
        public string Razlog { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO za otkazivanje zahtjeva
    /// </summary>
    public class CancelZahtjevDto
    {
        public string? Razlog { get; set; }
    }
}
