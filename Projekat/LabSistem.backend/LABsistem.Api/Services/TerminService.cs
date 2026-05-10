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
        private readonly Validators.ITerminValidator _validator;

        public TerminService(ITerminRepository repo, Validators.ITerminValidator validator)
        {
            _repo = repo;
            _validator = validator;
        }

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
                KabinetNaziv = x.kabinetNaziv,
                StatusTermina = x.termin.StatusTermina.ToString(),
                LimitOsoba = x.termin.LimitOsoba,
                VidljivoStudentima = x.termin.VidljivoStudentima,
                ProfesorIme = x.termin.Profesor?.ImePrezime,
                BrojOdobrenih = x.termin.Zahtjevi?.Count(z => z.StatusZahtjeva == LABsistem.Domain.Enums.StatusZahtjeva.Odobren) ?? 0
            }).ToList();
        }

        public async Task KreirajTermin(TerminCreateDTO dto)
        {
            _validator.ValidateCreate(dto.Datum, dto.VrijemePocetka, dto.VrijemeKraja);

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

        public async Task<TerminDTO?> GetById(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return null;
            return new TerminDTO
            {
                ID = t.ID,
                VrijemePocetka = t.VrijemePocetka,
                VrijemeKraja = t.VrijemeKraja,
                Datum = t.Datum,
                KreatorID = t.KreatorID,
                KabinetID = t.KabinetID
            };
        }
    }
}
