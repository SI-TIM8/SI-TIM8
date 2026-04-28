namespace LABsistem.Bll.DTOs.Recenzija
{
    /// <summary>
    /// DTO za prikaz recenzije
    /// </summary>
    public class GetRecenzijaDto
    {
        public int Id { get; set; }
        public string Komentar { get; set; } = string.Empty;
        public int Ocjena { get; set; }
        public int? OpremaId { get; set; }
        public DateTime? DatumKreiranja { get; set; }
    }

    /// <summary>
    /// DTO za kreiranje nove recenzije
    /// </summary>
    public class CreateRecenzijaDto
    {
        public int OpremaId { get; set; }
        public string Komentar { get; set; } = string.Empty;
        public int Ocjena { get; set; }
    }
}
