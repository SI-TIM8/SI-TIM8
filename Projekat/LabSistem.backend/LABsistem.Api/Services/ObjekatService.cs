using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;

namespace LABsistem.Api.Services
{
    public class ObjekatService : IObjekatService
    {
        private readonly IObjekatRepository _objekatRepo;
        private readonly IKabinetRepository _kabinetRepo;

        public ObjekatService(IObjekatRepository objekatRepo, IKabinetRepository kabinetRepo)
        {
            _objekatRepo = objekatRepo;
            _kabinetRepo = kabinetRepo;
        }

        public async Task<IEnumerable<ObjekatDTO>> VratiSveObjekte()
        {
            var objekti = await _objekatRepo.GetAllAsync();
            var kabinetiSvi = await _kabinetRepo.GetAllWithDetailsAsync();
            var kabinetiList = kabinetiSvi.ToList();

            return objekti.Select(o => new ObjekatDTO
            {
                ID = o.ID,
                Lokacija = o.Lokacija,
                RadnoVrijeme = o.RadnoVrijeme,
                Kabineti = kabinetiList
                    .Where(x => x.kabinet.ObjekatID == o.ID)
                    .Select(x => new KabinetDTO
                    {
                        ID = x.kabinet.ID,
                        Naziv = x.kabinet.Naziv,
                        KorisnikID = x.kabinet.KorisnikID,
                        OdgovorniKorisnik = x.odgovorniKorisnik,
                        ObjekatID = x.kabinet.ObjekatID,
                        ObjekatLokacija = o.Lokacija
                    }).ToList()
            }).ToList();
        }

        public async Task KreirajObjekat(ObjekatCreateDTO dto)
        {
            await _objekatRepo.AddAsync(new Objekat
            {
                Lokacija = dto.Lokacija,
                RadnoVrijeme = dto.RadnoVrijeme
            });
        }

        public async Task<bool> AzurirajObjekat(int id, ObjekatCreateDTO dto)
        {
            var o = await _objekatRepo.GetByIdAsync(id);
            if (o == null) return false;
            o.Lokacija = dto.Lokacija;
            o.RadnoVrijeme = dto.RadnoVrijeme;
            await _objekatRepo.UpdateAsync(o);
            return true;
        }

        public async Task<bool> ObrisiObjekat(int id)
        {
            var o = await _objekatRepo.GetByIdAsync(id);
            if (o == null) return false;
            await _objekatRepo.DeleteAsync(id);
            return true;
        }
    }
}