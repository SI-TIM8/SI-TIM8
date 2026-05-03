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
    public class EvidencijaRepository : IEvidencijaRepository
    {
        private readonly LabSistemDbContext _context;
        public EvidencijaRepository(LabSistemDbContext context) => _context = context;

        public async Task<IEnumerable<(Evidencija evidencija, string opremaNaziv, string korisnikImePrezime)>> GetAllWithDetailsAsync()
        {
            return await (
                from e in _context.Evidencije
                join o in _context.Oprema on e.OpremaID equals o.ID
                join k in _context.Korisnici on e.KorisnikID equals k.ID
                select new { e, o.Naziv, k.ImePrezime }
            ).ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.e, x.Naziv, x.ImePrezime)));
        }

        public async Task AddAsync(Evidencija evidencija)
        {
            await _context.Evidencije.AddAsync(evidencija);
            await _context.SaveChangesAsync();
        }
    }
}