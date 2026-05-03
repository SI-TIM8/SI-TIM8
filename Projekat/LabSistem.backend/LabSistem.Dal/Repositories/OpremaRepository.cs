using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LABsistem.Domain.Entities;
using LABsistem.Dal.Interfaces;
using LABsistem.Dal.Db;

namespace LABsistem.Dal.Repositories
{
    public class OpremaRepository : IOpremaRepository
    {
        private readonly LabSistemDbContext _context;
        public OpremaRepository(LabSistemDbContext context) => _context = context;

        public async Task<IEnumerable<Oprema>> GetAllAsync() =>
            await _context.Oprema.Include(o => o.Evidencije).ToListAsync();

        public async Task<Oprema> GetByIdAsync(int id) =>
            await _context.Oprema.FindAsync(id);

        public async Task AddAsync(Oprema oprema)
        {
            await _context.Oprema.AddAsync(oprema);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<(Oprema oprema, string kabinetNaziv, string zgradaNaziv)>> GetAllWithKabinetAsync()
        {
            return await (
                from o in _context.Oprema
                join k in _context.Kabineti on o.KabinetID equals k.ID into kGroup
                from k in kGroup.DefaultIfEmpty()
                join ob in _context.Objekti on k.ObjekatID equals ob.ID into obGroup
                from ob in obGroup.DefaultIfEmpty()
                select new { o, KabinetNaziv = k != null ? k.Naziv : "N/A", ZgradaNaziv = ob != null ? ob.Lokacija : "N/A" }
            ).ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.o, x.KabinetNaziv, x.ZgradaNaziv)));
        }

        public async Task UpdateAsync(Oprema oprema)
        {
            _context.Oprema.Update(oprema);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var oprema = await _context.Oprema.FindAsync(id);
            if (oprema != null)
            {
                _context.Oprema.Remove(oprema);
                await _context.SaveChangesAsync();
            }
        }
    }
}