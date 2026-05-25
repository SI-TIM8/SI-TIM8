using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;

namespace LABsistem.Api.Services
{
    public class KabinetService : IKabinetService
    {
        private readonly IKabinetRepository _repo;
        public KabinetService(IKabinetRepository repo) => _repo = repo;

        public async Task<IEnumerable<KabinetDTO>> VratiSveKabinete()
        {
            var rezultat = await _repo.GetAllWithDetailsAsync();
            return rezultat.Select(x => new KabinetDTO
            {
                ID = x.kabinet.ID,
                Naziv = x.kabinet.Naziv,
                KorisnikID = x.kabinet.KorisnikID,
                OdgovorniKorisnik = x.odgovorniKorisnik,
                ObjekatID = x.kabinet.ObjekatID,
                ObjekatLokacija = x.objekatLokacija,
                Kapacitet = x.kabinet.Kapacitet
            }).ToList();
        }

        public async Task KreirajKabinet(KabinetCreateDTO dto)
        {
            var novi = new Kabinet
            {
                Naziv = dto.Naziv,
                KorisnikID = dto.KorisnikID,
                ObjekatID = dto.ObjekatID,
                Kapacitet = dto.Kapacitet
            };
            await _repo.AddAsync(novi);
        }

        public async Task<bool> AzurirajKabinet(int id, KabinetCreateDTO dto)
        {
            var k = await _repo.GetByIdAsync(id);
            if (k == null) return false;
            k.Naziv = dto.Naziv;
            k.KorisnikID = dto.KorisnikID;
            k.ObjekatID = dto.ObjekatID;
            k.Kapacitet = dto.Kapacitet;
            await _repo.UpdateAsync(k);
            return true;
        }

        public async Task<bool> ObrisiKabinet(int id)
        {
            var k = await _repo.GetByIdAsync(id);
            if (k == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<KabinetDTO?> VratiKabinetPoId(int id)
        {
            var rezultat = await _repo.GetByIdWithDetailsAsync(id);
            if (rezultat == null) return null;
            
            return new KabinetDTO
            {
                ID = rezultat.Value.kabinet.ID,
                Naziv = rezultat.Value.kabinet.Naziv,
                KorisnikID = rezultat.Value.kabinet.KorisnikID,
                OdgovorniKorisnik = rezultat.Value.odgovorniKorisnik,
                ObjekatID = rezultat.Value.kabinet.ObjekatID,
                ObjekatLokacija = rezultat.Value.objekatLokacija,
                Kapacitet = rezultat.Value.kabinet.Kapacitet
            };
        }
    }
}