namespace LABsistem.Bll.DTOs.Objekat
{
    /// <summary>
    /// DTO za prikaz objekta (zgrade)
    /// </summary>
    public class GetObjekatDto
    {
        public int Id { get; set; }
        public string Lokacija { get; set; } = string.Empty;
        public string? RadnoVrijeme { get; set; }
        public int BrojKabineta { get; set; }
    }

    /// <summary>
    /// DTO za kreiranje novog objekta
    /// </summary>
    public class CreateObjekatDto
    {
        public string Lokacija { get; set; } = string.Empty;
        public string? RadnoVrijeme { get; set; }
    }

    /// <summary>
    /// DTO za ažuriranje objekta
    /// </summary>
    public class UpdateObjekatDto
    {
        public string? Lokacija { get; set; }
        public string? RadnoVrijeme { get; set; }
    }
}
