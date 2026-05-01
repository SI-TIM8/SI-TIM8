using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
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
