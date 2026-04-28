namespace LABsistem.Bll.DTOs.Obavijest
{
    /// <summary>
    /// DTO za prikaz obavijesti
    /// </summary>
    public class GetObavijestDto
    {
        public int Id { get; set; }
        public string Novosti { get; set; } = string.Empty;
        public bool Dostupnost { get; set; }
        public bool Procitano { get; set; }
        public DateTime? DatumSlanja { get; set; }
        public string? TipObavijesti { get; set; }
    }
}
