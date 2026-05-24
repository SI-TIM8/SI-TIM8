
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LABsistem.Domain.Entities;
using LABsistem.Dal.Interfaces;
using LABsistem.Dal.Db;
using LabSistem.Domain.Enums;

namespace LABsistem.Dal.Repositories
{
    public class TerminRepository : ITerminRepository
    {
        private readonly LabSistemDbContext _context;
        public TerminRepository(LabSistemDbContext context) => _context = context;

        public async Task<IEnumerable<(Termin termin, string kreatorIme, string kabinetNaziv)>> GetAllWithDetailsAsync()
        {
            return await (
                from t in _context.Termini
                join k in _context.Korisnici on t.KreatorID equals k.ID into kGroup
                from k in kGroup.DefaultIfEmpty()
                join kb in _context.Kabineti on t.KabinetID equals kb.ID into kbGroup
                from kb in kbGroup.DefaultIfEmpty()
                select new { t, KreatorIme = k != null ? k.ImePrezime : "N/A", KabinetNaziv = kb != null ? kb.Naziv : "N/A" }
            ).ToListAsync()
            .ContinueWith(result => result.Result.Select(x => (x.t, x.KreatorIme, x.KabinetNaziv)));
        }

        public async Task<Termin?> GetByIdAsync(int id) =>
            await _context.Termini.FindAsync(id);

        public async Task AddAsync(Termin termin)
        {
            await _context.Termini.AddAsync(termin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Termin termin)
        {
            _context.Termini.Update(termin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var termin = await _context.Termini.FindAsync(id);
            if (termin != null)
            {
                var povezaneObavijesti = await _context.Obavijesti
                    .Where(o => o.TerminID == id)
                    .ToListAsync();

                foreach (var obavijest in povezaneObavijesti)
                {
                    obavijest.TerminID = null;
                }

                _context.Termini.Remove(termin);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Termin>> GetSlobodniTerminiAsync() =>
            await _context.Termini
                .Where(t => t.StatusTermina == StatusTermina.Slobodan)
                .Include(t => t.Kabinet)
                .Include(t => t.Kreator)
                .ToListAsync();

        public async Task<IEnumerable<Termin>> GetTerminiProfesoraAsync(int profesorId) =>
            await _context.Termini
                .Where(t => t.ProfesorID == profesorId)
                .Include(t => t.Kabinet)
                .Include(t => t.Zahtjevi)
                .ToListAsync();

        public async Task<Termin?> GetByIdWithDetailsAsync(int id) =>
            await _context.Termini
                .Include(t => t.Kabinet)
                .Include(t => t.Profesor)
                .Include(t => t.Zahtjevi)
                    .ThenInclude(z => z.Student)
                .FirstOrDefaultAsync(t => t.ID == id);
    }
}
