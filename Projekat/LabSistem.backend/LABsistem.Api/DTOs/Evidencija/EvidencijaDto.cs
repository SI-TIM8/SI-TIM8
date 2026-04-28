namespace LABsistem.Bll.DTOs.Evidencija
{
    /// <summary>
    /// DTO za prikaz evidencije (audit log)
    /// </summary>
    public class GetEvidencijaDto
    {
        public int Id { get; set; }
        public int? KorisnikId { get; set; }
        public string? KorisnikUsername { get; set; }
        public string Akcija { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public DateTime? ResusDatum { get; set; }
        public string? IpAdresa { get; set; }
    }
}
