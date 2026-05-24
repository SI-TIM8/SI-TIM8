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
using Microsoft.Extensions.Hosting;

namespace LABsistem.Api.Services
{
    public class OpremaService : IOpremaService
    {
        private readonly IOpremaRepository _repo;
        private readonly Validators.IOpremaValidator _validator;
        private readonly IWebHostEnvironment _environment;
        private readonly string _documentationRoot;
        private const long MaxDocumentationFileSizeBytes = 10 * 1024 * 1024;
        private static readonly HashSet<string> AllowedDocumentationExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf"
        };
        private static readonly HashSet<string> AllowedDocumentationContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf"
        };

        public OpremaService(IOpremaRepository repo, Validators.IOpremaValidator validator, IWebHostEnvironment environment)
        {
            _repo = repo;
            _validator = validator;
            _environment = environment;
            _documentationRoot = Path.Combine(_environment.ContentRootPath, "uploads", "oprema");
        }

        public async Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto, OpremaDokumentacijaUpload? dokumentacija = null)
        {
            _validator.ValidateSave(dto.Naziv, dto.Kategorija, dto.KabinetID);
            ValidateDocumentationInput(dokumentacija, dto.DokumentacijaUrl);

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
                ArchivedAtUtc = null
            };

            if (dokumentacija != null)
            {
                var saved = await SaveDocumentationFileAsync(dokumentacija);
                nova.DokumentacijaFilePath = saved.FilePath;
                nova.DokumentacijaFileName = saved.FileName;
                nova.DokumentacijaUrl = null;
            }
            else if (!string.IsNullOrWhiteSpace(dto.DokumentacijaUrl))
            {
                nova.DokumentacijaUrl = dto.DokumentacijaUrl.Trim();
            }

            await _repo.AddAsync(nova);
            return new OpremaDTO
            {
                ID = nova.ID,
                Naziv = nova.Naziv,
                Kategorija = nova.Kategorija,
                SerijskiBroj = nova.SerijskiBroj,
                IsArchived = nova.IsArchived,
                ArchivedAtUtc = nova.ArchivedAtUtc,
                DokumentacijaUrl = nova.DokumentacijaUrl,
                DokumentacijaFileName = nova.DokumentacijaFileName
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
                DokumentacijaFileName = x.oprema.DokumentacijaFileName
            }).ToList();
        }

        public async Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto, OpremaDokumentacijaUpload? dokumentacija = null)
        {
            _validator.ValidateSave(dto.Naziv, dto.Kategorija, dto.KabinetID);
            ValidateDocumentationInput(dokumentacija, dto.DokumentacijaUrl);

            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;
            p.Naziv = dto.Naziv;
            p.Kategorija = dto.Kategorija;
            p.stanje = (StatusOpreme)dto.Stanje;
            p.KabinetID = dto.KabinetID;

            if (dokumentacija != null)
            {
                var saved = await SaveDocumentationFileAsync(dokumentacija);
                TryDeleteDocumentationFile(p.DokumentacijaFilePath);
                p.DokumentacijaFilePath = saved.FilePath;
                p.DokumentacijaFileName = saved.FileName;
                p.DokumentacijaUrl = null;
            }
            else if (!string.IsNullOrWhiteSpace(dto.DokumentacijaUrl))
            {
                TryDeleteDocumentationFile(p.DokumentacijaFilePath);
                p.DokumentacijaFilePath = null;
                p.DokumentacijaFileName = null;
                p.DokumentacijaUrl = dto.DokumentacijaUrl.Trim();
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
                    DokumentacijaFileName = o.DokumentacijaFileName
                }).ToList();
        }

        public async Task<OpremaDokumentacijaFile?> VratiDokumentacijuFajlAsync(int id)
        {
            var oprema = await _repo.GetByIdAsync(id);
            if (oprema == null || string.IsNullOrWhiteSpace(oprema.DokumentacijaFilePath))
            {
                return null;
            }

            var fullPath = Path.Combine(_environment.ContentRootPath, oprema.DokumentacijaFilePath);
            var fileName = string.IsNullOrWhiteSpace(oprema.DokumentacijaFileName)
                ? "dokumentacija.pdf"
                : oprema.DokumentacijaFileName;
            return new OpremaDokumentacijaFile(fullPath, fileName);
        }

        private void ValidateDocumentationInput(OpremaDokumentacijaUpload? dokumentacija, string? dokumentacijaUrl)
        {
            if (dokumentacija != null && !string.IsNullOrWhiteSpace(dokumentacijaUrl))
            {
                throw new Exception("Mozete priloziti PDF ili unijeti URL, ne oba.");
            }

            if (!string.IsNullOrWhiteSpace(dokumentacijaUrl))
            {
                var trimmed = dokumentacijaUrl.Trim();
                if (trimmed.Length > 500)
                {
                    throw new Exception("URL dokumentacije moze imati najvise 500 karaktera.");
                }

                if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new Exception("URL dokumentacije mora biti ispravan http/https link.");
                }
            }

            if (dokumentacija == null)
            {
                return;
            }

            if (dokumentacija.Length <= 0)
            {
                throw new Exception("PDF fajl je prazan.");
            }

            if (dokumentacija.Length > MaxDocumentationFileSizeBytes)
            {
                throw new Exception("PDF fajl ne smije biti veci od 10MB.");
            }

            var extension = Path.GetExtension(dokumentacija.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedDocumentationExtensions.Contains(extension))
            {
                throw new Exception("Dozvoljeni su samo PDF fajlovi.");
            }

            if (dokumentacija.FileName.Length > 255)
            {
                throw new Exception("Naziv PDF fajla je predugacak.");
            }

            if (!string.IsNullOrWhiteSpace(dokumentacija.ContentType) &&
                !AllowedDocumentationContentTypes.Contains(dokumentacija.ContentType))
            {
                throw new Exception("Dokumentacija mora biti PDF fajl.");
            }
        }

        private async Task<OpremaDokumentacijaFile> SaveDocumentationFileAsync(OpremaDokumentacijaUpload dokumentacija)
        {
            Directory.CreateDirectory(_documentationRoot);

            var extension = Path.GetExtension(dokumentacija.FileName).ToLowerInvariant();
            var storageFileName = $"{Guid.NewGuid():N}{extension}";
            var relativePath = Path.Combine("uploads", "oprema", storageFileName);
            var fullPath = Path.Combine(_environment.ContentRootPath, relativePath);

            await using var sourceStream = dokumentacija.OpenReadStream();
            await using var targetStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await sourceStream.CopyToAsync(targetStream);

            return new OpremaDokumentacijaFile(relativePath, dokumentacija.FileName);
        }

        private void TryDeleteDocumentationFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var fullPath = Path.Combine(_environment.ContentRootPath, relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
