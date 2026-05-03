using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Bll.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;

namespace LABsistem.Api.Services
{
    public class EvidencijaService : IEvidencijaService
    {
        private readonly IEvidencijaRepository _repo;
        public EvidencijaService(IEvidencijaRepository repo) => _repo = repo;

        public async Task<IEnumerable<EvidencijaDTO>> VratiSveEvidencije()
        {
            var rezultat = await _repo.GetAllWithDetailsAsync();
            return rezultat.Select(x => new EvidencijaDTO
            {
                ID = x.evidencija.ID,
                Status = x.evidencija.Status,
                Komentar = x.evidencija.Komentar,
                OpremaID = x.evidencija.OpremaID,
                OpremaNaziv = x.opremaNaziv,
                KorisnikID = x.evidencija.KorisnikID,
                KorisnikImePrezime = x.korisnikImePrezime
            }).ToList();
        }

        public async Task KreirajEvidenciju(EvidencijaCreateDTO dto)
        {
            var nova = new Evidencija
            {
                Status = dto.Status,
                Komentar = dto.Komentar,
                OpremaID = dto.OpremaID,
                KorisnikID = dto.KorisnikID
            };
            await _repo.AddAsync(nova);
        }

        public async Task AzurirajStatus(int id, string status)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return;
            e.Status = status;
            await _repo.UpdateAsync(e);
        }

        public async Task ObrisiEvidenciju(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}