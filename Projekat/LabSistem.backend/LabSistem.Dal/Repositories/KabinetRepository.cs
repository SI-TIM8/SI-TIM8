using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LABsistem.Domain.Entities;
using LABsistem.Dal.Interfaces;
using LABsistem.Dal.Db;

namespace LABsistem.Dal.Repositories
{
    public class KabinetRepository : IKabinetRepository
    {
        private readonly LabSistemDbContext _context;
        public KabinetRepository(LabSistemDbContext context) => _context = context;

        public async Task<IEnumerable<(Kabinet kabinet, string odgovorniKorisnik, string objekatLokacija)>> GetAllWithDetailsAsync()
        {
            return await (
                from k in _context.Kabineti
                join u in _context.Korisnici on k.KorisnikID equals u.ID into uGroup
                from u in uGroup.DefaultIfEmpty()
                join o in _context.Objekti on k.ObjekatID equals o.ID into oGroup
                from o in oGroup.DefaultIfEmpty()
                select new { k, ImePrezime = u != null ? u.ImePrezime : "N/A", Lokacija = o != null ? o.Lokacija : "N/A" }
            ).ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.k, x.ImePrezime, x.Lokacija)));
        }

        public async Task<Kabinet?> GetByIdAsync(int id) =>
            await _context.Kabineti.FindAsync(id);

        public async Task AddAsync(Kabinet kabinet)
        {
            await _context.Kabineti.AddAsync(kabinet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Kabinet kabinet)
        {
            _context.Kabineti.Update(kabinet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var kabinet = await _context.Kabineti.FindAsync(id);
            if (kabinet != null)
            {
                _context.Kabineti.Remove(kabinet);
                await _context.SaveChangesAsync();
            }
        }
    }
}