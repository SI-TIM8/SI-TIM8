namespace LABsistem.Application.DTOs
{
    public class ObjekatCreateDTO
    {
        public string Lokacija { get; set; } = default!;
        public string RadnoVrijeme { get; set; } = default!;
    }

    public class ObjekatDTO
    {
        public int ID { get; set; }
        public string Lokacija { get; set; } = default!;
        public string RadnoVrijeme { get; set; } = default!;
        public List<KabinetDTO> Kabineti { get; set; } = new();
    }
}