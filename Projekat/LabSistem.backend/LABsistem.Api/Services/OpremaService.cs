using System.Collections.Generic;
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
            _validator.ValidateCreate(dto.Naziv, dto.KabinetID);

            var postojecaOprema = await _repo.GetAllAsync();
            var nextSerijskiBroj = postojecaOprema.Any()
                ? postojecaOprema.Max(o => o.SerijskiBroj) + 1
                : 1;

            var nova = new Oprema
            {
                Naziv = dto.Naziv,
                SerijskiBroj = nextSerijskiBroj,
                stanje = (StatusOpreme)dto.Stanje,
                KabinetID = dto.KabinetID,
                KreatorID = dto.KreatorID
            };
            await _repo.AddAsync(nova);
            return new OpremaDTO
            {
                ID = nova.ID,
                Naziv = nova.Naziv,
                SerijskiBroj = nova.SerijskiBroj
            };
        }

        public async Task<IEnumerable<OpremaDTO>> VratiSvuOpremu()
        {
            var rezultat = await _repo.GetAllWithKabinetAsync();
            return rezultat.Select(x => new OpremaDTO
            {
                ID = x.oprema.ID,
                Naziv = x.oprema.Naziv,
                SerijskiBroj = x.oprema.SerijskiBroj,
                Stanje = (int)x.oprema.stanje,
                KabinetID = x.oprema.KabinetID,
                KreatorID = x.oprema.KreatorID,
                KabinetNaziv = x.kabinetNaziv,
                ZgradaNaziv = x.zgradaNaziv
            }).ToList();
        }

        public async Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;
            p.Naziv = dto.Naziv;
            p.stanje = (StatusOpreme)dto.Stanje;
            p.KabinetID = dto.KabinetID;
            await _repo.UpdateAsync(p);
            return true;
        }

        public async Task<bool> ObrisiOpremu(int id)
        {
            var postojeca = await _repo.GetByIdAsync(id);
            if (postojeca == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<OpremaDTO>> VratiOpremuPoKabinetu(int kabinetId)
        {
            var oprema = await _repo.GetAllAsync();
            return oprema
                .Where(o => o.KabinetID == kabinetId)
                .Select(o => new OpremaDTO
                {
                    ID = o.ID,
                    Naziv = o.Naziv,
                    SerijskiBroj = o.SerijskiBroj,
                    Stanje = (int)o.stanje
                }).ToList();
        }
    }
}