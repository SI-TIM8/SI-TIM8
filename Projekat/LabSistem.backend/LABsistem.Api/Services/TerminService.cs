using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;

namespace LABsistem.Api.Services
{
    public class TerminService : ITerminService
    {
        private readonly ITerminRepository _repo;
        public TerminService(ITerminRepository repo) => _repo = repo;

        public async Task<IEnumerable<TerminDTO>> VratiSveTermine()
        {
            var rezultat = await _repo.GetAllWithDetailsAsync();
            return rezultat.Select(x => new TerminDTO
            {
                ID = x.termin.ID,
                VrijemePocetka = x.termin.VrijemePocetka,
                VrijemeKraja = x.termin.VrijemeKraja,
                Datum = x.termin.Datum,
                KreatorID = x.termin.KreatorID,
                KreatorIme = x.kreatorIme,
                KabinetID = x.termin.KabinetID,
                KabinetNaziv = x.kabinetNaziv
            }).ToList();
        }

        public async Task KreirajTermin(TerminCreateDTO dto)
        {
            var novi = new Termin
            {
                VrijemePocetka = dto.VrijemePocetka,
                VrijemeKraja = dto.VrijemeKraja,
                Datum = dto.Datum,
                KreatorID = dto.KreatorID,
                KabinetID = dto.KabinetID
            };
            await _repo.AddAsync(novi);
        }

        public async Task<bool> AzurirajTermin(int id, TerminCreateDTO dto)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return false;
            t.VrijemePocetka = dto.VrijemePocetka;
            t.VrijemeKraja = dto.VrijemeKraja;
            t.Datum = dto.Datum;
            t.KreatorID = dto.KreatorID;
            t.KabinetID = dto.KabinetID;
            await _repo.UpdateAsync(t);
            return true;
        }

        public async Task<bool> ObrisiTermin(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
