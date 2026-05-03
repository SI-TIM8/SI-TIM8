using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LABSistem.Bll.DTOs; 

namespace LABsistem.Api.Services
{
    public class OpremaService : IOpremaService
    {
        private readonly IOpremaRepository _repo;
        public OpremaService(IOpremaRepository repo) => _repo = repo;

        public async Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto)
        {
            var nova = new Oprema
            {
                Naziv = dto.Naziv,
                SerijskiBroj = dto.SerijskiBroj,
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
            p.SerijskiBroj = dto.SerijskiBroj;
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
    }
}