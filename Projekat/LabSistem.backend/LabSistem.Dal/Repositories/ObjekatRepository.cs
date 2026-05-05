using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using LABsistem.Domain.Entities;
using LABsistem.Dal.Interfaces;
using LABsistem.Dal.Db;

namespace LABsistem.Dal.Repositories
{
    public class ObjekatRepository : IObjekatRepository
    {
        private readonly LabSistemDbContext _context;
        public ObjekatRepository(LabSistemDbContext context) => _context = context;

        public async Task<IEnumerable<Objekat>> GetAllAsync() =>
            await _context.Objekti.Include(o => o.Kabineti).ToListAsync();

        public async Task<Objekat?> GetByIdAsync(int id) =>
            await _context.Objekti.Include(o => o.Kabineti).FirstOrDefaultAsync(o => o.ID == id);

        public async Task AddAsync(Objekat objekat)
        {
            await _context.Objekti.AddAsync(objekat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Objekat objekat)
        {
            _context.Objekti.Update(objekat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var objekat = await _context.Objekti.FindAsync(id);
            if (objekat != null)
            {
                _context.Objekti.Remove(objekat);
                await _context.SaveChangesAsync();
            }
        }
    }
}