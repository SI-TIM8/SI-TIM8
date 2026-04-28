namespace LABsistem.Bll.DTOs.Termin
{
    /// <summary>
    /// DTO za prikaz termina
    /// </summary>
    public class GetTerminDto
    {
        public int Id { get; set; }
        public int KabinetId { get; set; }
        public string KabinetNaziv { get; set; } = string.Empty;
        public string Datum { get; set; } = string.Empty;
        public string VrijemePocetka { get; set; } = string.Empty;
        public string VremeKraja { get; set; } = string.Empty;
        public int? Kapacitet { get; set; }
        public int ZauzetoMjesta { get; set; }
        public string? Status { get; set; }
        public DateTime? DatumKreiranja { get; set; }
    }

    /// <summary>
    /// DTO za kreiranje novog termina
    /// </summary>
    public class CreateTerminDto
    {
        public int KabinetId { get; set; }
        public string Datum { get; set; } = string.Empty;
        public string VrijemePocetka { get; set; } = string.Empty;
        public string VremeKraja { get; set; } = string.Empty;
        public int? Kapacitet { get; set; }
    }

    /// <summary>
    /// DTO za ažuriranje termina
    /// </summary>
    public class UpdateTerminDto
    {
        public string? Datum { get; set; }
        public string? VrijemePocetka { get; set; }
        public string? VremeKraja { get; set; }
        public int? Kapacitet { get; set; }
    }
}
