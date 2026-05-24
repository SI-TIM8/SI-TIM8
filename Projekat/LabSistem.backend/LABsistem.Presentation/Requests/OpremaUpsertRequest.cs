using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LABsistem.Presentation.Requests
{
    public class OpremaUpsertRequest
    {
        [Required]
        [StringLength(30)]
        public string Naziv { get; set; } = default!;

        [Required]
        [StringLength(40)]
        public string Kategorija { get; set; } = default!;

        public int SerijskiBroj { get; set; }

        public int Stanje { get; set; }

        public int KabinetID { get; set; }

        public int KreatorID { get; set; }

        [StringLength(500)]
        public string? DokumentacijaUrl { get; set; }

        public IFormFile? DokumentacijaFile { get; set; }
    }
}
