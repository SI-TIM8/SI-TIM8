using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.AspNetCore.Hosting;

namespace LABsistem.Api.Services
{
    public class OpremaService : IOpremaService
    {
        private readonly IOpremaRepository _repo;
        private readonly Validators.IOpremaValidator _validator;
        private readonly IWebHostEnvironment? _environment;

        public OpremaService(IOpremaRepository repo, Validators.IOpremaValidator validator, IWebHostEnvironment? environment = null)
        {
            _repo = repo;
            _validator = validator;
            _environment = environment;
        }

        public async Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto, OpremaDokumentacijaUpload? dokumentacija = null)
        {
            _validator.ValidateSave(dto.Naziv, dto.Kategorija, dto.KabinetID);
            ValidateDocumentationUrl(dto.DokumentacijaUrl);

            var postojecaOprema = await _repo.GetAllAsync();
            var nextSerijskiBroj = postojecaOprema.Any()
                ? postojecaOprema.Max(o => o.SerijskiBroj) + 1
                : 1;

            var nova = new Oprema
            {
                Naziv = dto.Naziv,
                Kategorija = dto.Kategorija,
                SerijskiBroj = nextSerijskiBroj,
                stanje = (StatusOpreme)dto.Stanje,
                KabinetID = dto.KabinetID,
                KreatorID = dto.KreatorID,
                IsArchived = false,
                ArchivedAtUtc = null,
                DokumentacijaUrl = !string.IsNullOrWhiteSpace(dto.DokumentacijaUrl) 
                    ? dto.DokumentacijaUrl.Trim() 
                    : null
            };

            if (dokumentacija != null)
            {
                var dokumentacijaInfo = await SaveDocumentationFileAsync(dokumentacija);
                nova.DokumentacijaFilePath = dokumentacijaInfo.FilePath;
                nova.DokumentacijaFileName = dokumentacijaInfo.FileName;
            }

            await _repo.AddAsync(nova);
            var dokumentacijaFileName = ResolveDokumentacijaFileName(nova.DokumentacijaFileName, nova.DokumentacijaFilePath);
            return new OpremaDTO
            {
                ID = nova.ID,
                Naziv = nova.Naziv,
                Kategorija = nova.Kategorija,
                SerijskiBroj = nova.SerijskiBroj,
                IsArchived = nova.IsArchived,
                ArchivedAtUtc = nova.ArchivedAtUtc,
                DokumentacijaUrl = nova.DokumentacijaUrl,
                DokumentacijaFileName = dokumentacijaFileName
            };
        }

        public async Task<IEnumerable<OpremaDTO>> VratiSvuOpremu(string prikaz)
        {
            var rezultat = await _repo.GetAllWithKabinetAsync();
            var filtriranaOprema = rezultat.Where(x => prikaz switch
            {
                "arhivirana" => x.oprema.IsArchived,
                "sve" => true,
                _ => !x.oprema.IsArchived
            });

            return filtriranaOprema.Select(x => new OpremaDTO
            {
                ID = x.oprema.ID,
                Naziv = x.oprema.Naziv,
                Kategorija = x.oprema.Kategorija,
                SerijskiBroj = x.oprema.SerijskiBroj,
                Stanje = (int)x.oprema.stanje,
                KabinetID = x.oprema.KabinetID,
                KreatorID = x.oprema.KreatorID,
                KabinetNaziv = x.kabinetNaziv,
                ZgradaNaziv = x.zgradaNaziv,
                IsArchived = x.oprema.IsArchived,
                ArchivedAtUtc = x.oprema.ArchivedAtUtc,
                DokumentacijaUrl = x.oprema.DokumentacijaUrl,
                DokumentacijaFileName = ResolveDokumentacijaFileName(
                    x.oprema.DokumentacijaFileName,
                    x.oprema.DokumentacijaFilePath)
            }).ToList();
        }

        public async Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto, OpremaDokumentacijaUpload? dokumentacija = null)
        {
            _validator.ValidateSave(dto.Naziv, dto.Kategorija, dto.KabinetID);
            ValidateDocumentationUrl(dto.DokumentacijaUrl);

            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;
            
            p.Naziv = dto.Naziv;
            p.Kategorija = dto.Kategorija;
            p.stanje = (StatusOpreme)dto.Stanje;
            p.KabinetID = dto.KabinetID;
            if (!string.IsNullOrWhiteSpace(dto.DokumentacijaUrl))
            {
                p.DokumentacijaUrl = dto.DokumentacijaUrl.Trim();
            }

            if (dokumentacija != null)
            {
                var dokumentacijaInfo = await SaveDocumentationFileAsync(dokumentacija, p.DokumentacijaFilePath);
                p.DokumentacijaFilePath = dokumentacijaInfo.FilePath;
                p.DokumentacijaFileName = dokumentacijaInfo.FileName;
            }

            await _repo.UpdateAsync(p);
            return true;
        }

        public async Task<bool> ArhivirajOpremu(int id)
        {
            var postojeca = await _repo.GetByIdAsync(id);
            if (postojeca == null) return false;
            if (postojeca.IsArchived) return true;

            postojeca.IsArchived = true;
            postojeca.ArchivedAtUtc = DateTime.UtcNow;
            await _repo.UpdateAsync(postojeca);
            return true;
        }

        public async Task<bool> VratiIzArhive(int id)
        {
            var postojeca = await _repo.GetByIdAsync(id);
            if (postojeca == null) return false;
            if (!postojeca.IsArchived) return true;

            postojeca.IsArchived = false;
            postojeca.ArchivedAtUtc = null;
            await _repo.UpdateAsync(postojeca);
            return true;
        }

        public async Task<IEnumerable<OpremaDTO>> VratiOpremuPoKabinetu(int kabinetId)
        {
            var oprema = await _repo.GetAllAsync();
            return oprema
                .Where(o => o.KabinetID == kabinetId && !o.IsArchived)
                .Select(o => new OpremaDTO
                {
                    ID = o.ID,
                    Naziv = o.Naziv,
                    Kategorija = o.Kategorija,
                    SerijskiBroj = o.SerijskiBroj,
                    Stanje = (int)o.stanje,
                    IsArchived = o.IsArchived,
                    ArchivedAtUtc = o.ArchivedAtUtc,
                    DokumentacijaUrl = o.DokumentacijaUrl,
                    DokumentacijaFileName = ResolveDokumentacijaFileName(
                        o.DokumentacijaFileName,
                        o.DokumentacijaFilePath)
                }).ToList();
        }

        public async Task<OpremaDokumentacijaFile?> VratiDokumentacijuFajlAsync(int id)
        {
            var oprema = await _repo.GetByIdAsync(id);
            if (oprema == null || string.IsNullOrWhiteSpace(oprema.DokumentacijaFilePath))
            {
                return null;
            }

            var fileName = string.IsNullOrWhiteSpace(oprema.DokumentacijaFileName)
                ? "dokumentacija.pdf"
                : oprema.DokumentacijaFileName;
            return new OpremaDokumentacijaFile(oprema.DokumentacijaFilePath, fileName);
        }

        private void ValidateDocumentationUrl(string? dokumentacijaUrl)
        {
            if (string.IsNullOrWhiteSpace(dokumentacijaUrl))
            {
                return;
            }

            var trimmed = dokumentacijaUrl.Trim();
            if (trimmed.Length > 500)
            {
                throw new Exception("URL dokumentacije može imati najviše 500 karaktera.");
            }

            if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new Exception("URL dokumentacije mora biti ispravan http/https link.");
            }
        }

        private async Task<OpremaDokumentacijaFile> SaveDocumentationFileAsync(
            OpremaDokumentacijaUpload dokumentacija,
            string? existingFilePath = null)
        {
            if (dokumentacija.Length <= 0)
            {
                throw new Exception("Dokumentacija fajl ne može biti prazan.");
            }

            var originalFileName = Path.GetFileName(dokumentacija.FileName);
            var extension = Path.GetExtension(originalFileName);
            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Dokumentacija mora biti PDF fajl.");
            }

            var contentRoot = _environment?.ContentRootPath ?? AppContext.BaseDirectory;
            var uploadsRoot = Path.Combine(contentRoot, "uploads", "oprema");
            Directory.CreateDirectory(uploadsRoot);

            var storedFileName = $"{Guid.NewGuid():N}{extension}";
            var storedFilePath = Path.Combine(uploadsRoot, storedFileName);

            await using (var stream = dokumentacija.OpenReadStream())
            await using (var fileStream = new FileStream(storedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await stream.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrWhiteSpace(existingFilePath) && File.Exists(existingFilePath))
            {
                File.Delete(existingFilePath);
            }

            return new OpremaDokumentacijaFile(storedFilePath, originalFileName);
        }

        private static string? ResolveDokumentacijaFileName(string? dokumentacijaFileName, string? dokumentacijaFilePath)
        {
            if (!string.IsNullOrWhiteSpace(dokumentacijaFileName))
            {
                return dokumentacijaFileName;
            }

            if (string.IsNullOrWhiteSpace(dokumentacijaFilePath))
            {
                return null;
            }

            var extractedName = Path.GetFileName(dokumentacijaFilePath);
            return string.IsNullOrWhiteSpace(extractedName) ? null : extractedName;
        }
    }
}