using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;

namespace LABsistem.Api.Services
{
    public class OpremaService : IOpremaService
    {
        private readonly IOpremaRepository _repo;
        private readonly Validators.IOpremaValidator _validator;

        public OpremaService(IOpremaRepository repo, Validators.IOpremaValidator validator)
        {
            _repo = repo;
            _validator = validator;
        }

        public async Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto)
        {
            _validator.ValidateSave(dto.Naziv, dto.Kategorija, dto.KabinetID);

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
            await _repo.AddAsync(nova);
            return new OpremaDTO
            {
                ID = nova.ID,
                Naziv = nova.Naziv,
                Kategorija = nova.Kategorija,
                SerijskiBroj = nova.SerijskiBroj,
                IsArchived = nova.IsArchived,
                ArchivedAtUtc = nova.ArchivedAtUtc
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
                ArchivedAtUtc = x.oprema.ArchivedAtUtc
            }).ToList();
        }

        public async Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto)
        {
            _validator.ValidateSave(dto.Naziv, dto.Kategorija, dto.KabinetID);

            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;
            p.Naziv = dto.Naziv;
            p.Kategorija = dto.Kategorija;
            p.stanje = (StatusOpreme)dto.Stanje;
            p.KabinetID = dto.KabinetID;
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
                    ArchivedAtUtc = o.ArchivedAtUtc
                }).ToList();
        }
    }
}
