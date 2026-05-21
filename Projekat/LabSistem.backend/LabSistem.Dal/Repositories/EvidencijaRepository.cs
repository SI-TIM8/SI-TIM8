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

        public async Task<IEnumerable<(Evidencija evidencija, string opremaNaziv, string? opremaKategorija, int opremaSerijskiBroj, int opremaStanje, int opremaKabinetID, string? opremaKabinetNaziv, string? opremaZgradaNaziv, string korisnikImePrezime, string? profesorImePrezime, string? obradioImePrezime, DateTime? terminDatum, TimeSpan? terminVrijemePocetka, TimeSpan? terminVrijemeKraja)>> GetAllWithDetailsAsync()
        {
            return await (
                from e in _context.Evidencije
                join o in _context.Oprema on e.OpremaID equals o.ID
                join k in _context.Korisnici on e.KorisnikID equals k.ID
                join p in _context.Korisnici on e.ProfesorID equals p.ID into professorJoin
                from p in professorJoin.DefaultIfEmpty()
                join obradilac in _context.Korisnici on e.ObradioKorisnikID equals obradilac.ID into obradilacJoin
                from obradilac in obradilacJoin.DefaultIfEmpty()
                join t in _context.Termini on e.TerminID equals t.ID into terminJoin
                from t in terminJoin.DefaultIfEmpty()
                join kabinet in _context.Kabineti on o.KabinetID equals kabinet.ID into kabinetJoin
                from kabinet in kabinetJoin.DefaultIfEmpty()
                join objekat in _context.Objekti on kabinet.ObjekatID equals objekat.ID into objekatJoin
                from objekat in objekatJoin.DefaultIfEmpty()
                select new
                {
                    e,
                    o.Naziv,
                    o.Kategorija,
                    o.SerijskiBroj,
                    OpremaStanje = (int)o.stanje,
                    o.KabinetID,
                    KabinetNaziv = kabinet != null ? (string?)kabinet.Naziv : null,
                    ZgradaNaziv = objekat != null ? (string?)objekat.Lokacija : null,
                    k.ImePrezime,
                    ProfesorImePrezime = p != null ? (string?)p.ImePrezime : null,
                    ObradioImePrezime = obradilac != null ? (string?)obradilac.ImePrezime : null,
                    TerminDatum = t != null ? t.Datum : (DateTime?)null,
                    TerminVrijemePocetka = t != null ? t.VrijemePocetka : (TimeSpan?)null,
                    TerminVrijemeKraja = t != null ? t.VrijemeKraja : (TimeSpan?)null
                }
            ).ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.e, x.Naziv, x.Kategorija, x.SerijskiBroj, x.OpremaStanje, x.KabinetID, x.KabinetNaziv, x.ZgradaNaziv, x.ImePrezime, x.ProfesorImePrezime, x.ObradioImePrezime, x.TerminDatum, x.TerminVrijemePocetka, x.TerminVrijemeKraja)));
        }

        public async Task AddAsync(Evidencija evidencija)
        {
            await _context.Evidencije.AddAsync(evidencija);
            await _context.SaveChangesAsync();
        }

        public async Task<Evidencija?> GetByIdAsync(int id) =>
            await _context.Evidencije.FindAsync(id);

        public async Task UpdateAsync(Evidencija evidencija)
        {
            _context.Evidencije.Update(evidencija);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _context.Evidencije.FindAsync(id);
            if (e != null)
            {
                _context.Evidencije.Remove(e);
                await _context.SaveChangesAsync();
            }
        }
    }
}