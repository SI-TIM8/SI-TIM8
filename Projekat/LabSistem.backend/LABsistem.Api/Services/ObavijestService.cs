using LABsistem.Application.DTOs;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LABsistem.Api.Services;

namespace LABsistem.Api.Services
{
    public class ObavijestService : IObavijestService
    {
        private readonly LabSistemDbContext _context;
        public ObavijestService(LabSistemDbContext context) => _context = context;

        public async Task KreirajAsync(int korisnikId, string poruka, int? terminId = null)
        {
            await _context.Obavijesti.AddAsync(new Obavijest
            {
                KorisnikID = korisnikId,
                Novosti = poruka,
                Dostupnost = false,
                DatumKreiranja = DateTime.UtcNow,
                TerminID = terminId
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ObavijestDTO>> VratiZaKorisnikaAsync(int korisnikId)
        {
            return await _context.Obavijesti
                .Where(o => o.KorisnikID == korisnikId)
                .OrderByDescending(o => o.DatumKreiranja)
                .Select(o => new ObavijestDTO
                {
                    ID = o.ID,
                    Novosti = o.Novosti,
                    Dostupnost = o.Dostupnost,
                    DatumKreiranja = o.DatumKreiranja,
                    TerminID = o.TerminID
                })
                .ToListAsync();
        }

        public async Task OznaciKaoProcitanuAsync(int id, int korisnikId)
        {
            var o = await _context.Obavijesti
                .FirstOrDefaultAsync(x => x.ID == id && x.KorisnikID == korisnikId);
            if (o != null)
            {
                o.Dostupnost = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task OznaciSveKaoProcitaneAsync(int korisnikId)
        {
            var neprocitane = await _context.Obavijesti
                .Where(o => o.KorisnikID == korisnikId && !o.Dostupnost)
                .ToListAsync();
            neprocitane.ForEach(o => o.Dostupnost = true);
            await _context.SaveChangesAsync();
        }

        public async Task<int> BrojNeprocitanihAsync(int korisnikId)
        {
            return await _context.Obavijesti
                .CountAsync(o => o.KorisnikID == korisnikId && !o.Dostupnost);
        }
    }
}