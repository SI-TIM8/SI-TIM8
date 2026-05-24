using System.IO;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LABsistem.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpremaController : ControllerBase
    {
        private readonly IOpremaService _service;

        public OpremaController(IOpremaService service) => _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Profesor,Tehnicar")]
        public async Task<IActionResult> Get([FromQuery] string prikaz = "aktivna")
        {
            var oprema = await _service.VratiSvuOpremu(prikaz);
            return Ok(oprema);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Post([FromForm] OpremaUpsertRequest request)
        {
            var dto = MapToCreateDto(request);
            var upload = BuildDokumentacijaUpload(request.DokumentacijaFile);
            await _service.KreirajOpremu(dto, upload);
            return Ok(new { message = "Oprema uspjesno dodana." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Put(int id, [FromForm] OpremaUpsertRequest request)
        {
            var dto = MapToCreateDto(request);
            var upload = BuildDokumentacijaUpload(request.DokumentacijaFile);
            await _service.AzurirajOpremu(id, dto, upload);
            return Ok(new { message = "Oprema azurirana." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Delete(int id)
        {
            var uspjeh = await _service.ArhivirajOpremu(id);
            if (!uspjeh) return NotFound();
            return Ok(new { message = "Oprema arhivirana." });
        }

        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin,Tehnicar")]
        public async Task<IActionResult> Restore(int id)
        {
            var uspjeh = await _service.VratiIzArhive(id);
            if (!uspjeh) return NotFound();
            return Ok(new { message = "Oprema vraćena iz arhive." });
        }

        [HttpGet("kabinet/{kabinetId}")]
        [Authorize]
        public async Task<IActionResult> GetPoKabinetu(int kabinetId)
        {
            var oprema = await _service.VratiOpremuPoKabinetu(kabinetId);
            return Ok(oprema);
        }

        [HttpGet("{id}/documentation/file")]
        [Authorize]
        public async Task<IActionResult> GetDocumentationFile(int id)
        {
            var dokumentacija = await _service.VratiDokumentacijuFajlAsync(id);
            if (dokumentacija == null || !System.IO.File.Exists(dokumentacija.FilePath))
            {
                return NotFound();
            }

            return PhysicalFile(dokumentacija.FilePath, "application/pdf", dokumentacija.FileName, enableRangeProcessing: true);
        }

        private static OpremaCreateDTO MapToCreateDto(OpremaUpsertRequest request)
        {
            return new OpremaCreateDTO
            {
                Naziv = request.Naziv,
                Kategorija = request.Kategorija,
                SerijskiBroj = request.SerijskiBroj,
                Stanje = request.Stanje,
                KabinetID = request.KabinetID,
                KreatorID = request.KreatorID,
                DokumentacijaUrl = request.DokumentacijaUrl
            };
        }

        private static OpremaDokumentacijaUpload? BuildDokumentacijaUpload(IFormFile? file)
        {
            if (file == null)
            {
                return null;
            }

            return new OpremaDokumentacijaUpload(
                file.FileName,
                file.ContentType ?? string.Empty,
                file.OpenReadStream,
                file.Length);
        }
    }
}
