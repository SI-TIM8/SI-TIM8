using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LabSistem.Domain.Enums;
using LABsistem.Api.Services;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public class RezervacijaService : IRezervacijaService
    {
        private readonly LabSistemDbContext _context;
        private readonly Validators.IRezervacijaValidator _validator;

        public RezervacijaService(LabSistemDbContext context, Validators.IRezervacijaValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task RezervisiTermin(
            int profesorId,
            int terminId,
            int limitOsoba,
            bool vidljivoStudentima)
        {
            await _validator.ValidateRezervacija(terminId, profesorId, limitOsoba);

            var termin = await _context.Termini
                .FirstAsync(t => t.ID == terminId);

            termin.ProfesorID = profesorId;
            termin.StatusTermina = StatusTermina.Rezervisan;
            termin.LimitOsoba = limitOsoba;
            termin.VidljivoStudentima = vidljivoStudentima;

            await _context.SaveChangesAsync();
        }

        public async Task OtkaziTermin(int profesorId, int terminId)
        {
            await _validator.ValidateOtkazivanje(terminId, profesorId);

            var termin = await _context.Termini
                .FirstAsync(t => t.ID == terminId);

            var zahtjevi = await _context.Zahtjevi
                .Where(z => z.TerminID == terminId)
                .ToListAsync();

            foreach (var zahtjev in zahtjevi)
            {
                zahtjev.StatusZahtjeva = StatusZahtjeva.Otkazan;
            }

            termin.ProfesorID = null;
            termin.StatusTermina = StatusTermina.Slobodan;
            termin.LimitOsoba = null;
            termin.VidljivoStudentima = false;

            await _context.SaveChangesAsync();
        }

        public async Task PosaljiZahtjev(int studentId, int terminId)
        {
            await _validator.ValidateZahtjev(studentId, terminId);

            var zahtjev = new Zahtjev
            {
                StudentID = studentId,
                TerminID = terminId,
                StatusZahtjeva = StatusZahtjeva.NaCekanju,
                Komentar = string.Empty
            };

            await _context.Zahtjevi.AddAsync(zahtjev);
            await _context.SaveChangesAsync();
        }

        public async Task OdgovoriNaZahtjev(
            int profesorId,
            int zahtjevId,
            bool odobri)
        {
            await _validator.ValidateOdgovor(zahtjevId, profesorId, odobri);

            var zahtjev = await _context.Zahtjevi
                .Include(z => z.Termin)
                .FirstAsync(z => z.ID == zahtjevId);

            if (odobri)
            {
                zahtjev.StatusZahtjeva = StatusZahtjeva.Odobren;
            }
            else
            {
                zahtjev.StatusZahtjeva = StatusZahtjeva.Odbijen;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TerminDTO>> GetSlobodniTerminiAsync()
        {
            return await _context.Termini
                .Where(t => t.StatusTermina == StatusTermina.Slobodan)
                .Include(t => t.Kabinet)
                .Include(t => t.Kreator)
                .Select(t => MapToTerminDto(t))
                .ToListAsync();
        }

        public async Task<IEnumerable<TerminDTO>> GetMojeRezervacijeAsync(int korisnikId, string uloga)
        {
            if (uloga == "profesor")
            {
                return await _context.Termini
                    .Where(t => t.ProfesorID == korisnikId)
                    .Include(t => t.Kabinet)
                    .Include(t => t.Zahtjevi)
                    .Select(t => MapToTerminDto(t))
                    .ToListAsync();
            }
            else // student
            {
                return await _context.Zahtjevi
                    .Include(z => z.Termin)
                        .ThenInclude(t => t.Kabinet)
                    .Include(z => z.Termin)
                        .ThenInclude(t => t.Profesor)
                    .Where(z => z.StudentID == korisnikId && z.StatusZahtjeva == StatusZahtjeva.Odobren)
                    .Select(z => MapToTerminDto(z.Termin))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<ZahtjevDTO>> GetDolazniZahtjeviAsync(int profesorId)
        {
            return await _context.Zahtjevi
                .Include(z => z.Student)
                .Include(z => z.Termin)
                .Where(z => z.Termin.ProfesorID == profesorId && z.StatusZahtjeva == StatusZahtjeva.NaCekanju)
                .Select(z => new ZahtjevDTO
                {
                    ID = z.ID,
                    TerminID = z.TerminID,
                    KabinetNaziv = z.Termin.Kabinet != null ? z.Termin.Kabinet.Naziv : "N/A",
                    Datum = z.Termin.Datum,
                    VrijemePocetka = z.Termin.VrijemePocetka,
                    VrijemeKraja = z.Termin.VrijemeKraja,
                    StudentIme = z.Student.ImePrezime,
                    StatusZahtjeva = z.StatusZahtjeva.ToString(),
                    KreiranoU = DateTime.Now
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TerminDTO>> GetDostupniTerminiZaStudenteAsync(int studentId)
        {
            var termini = await _context.Termini
                .Where(t => t.StatusTermina == StatusTermina.Rezervisan && t.VidljivoStudentima)
                .Include(t => t.Kabinet)
                .Include(t => t.Profesor)
                .Include(t => t.Zahtjevi)
                .ToListAsync();

            return termini.Select(t => {
                var dto = MapToTerminDto(t);
                var studentZahtjev = t.Zahtjevi?.FirstOrDefault(z => z.StudentID == studentId);
                dto.StatusPrijave = studentZahtjev?.StatusZahtjeva.ToString();
                return dto;
            });
        }

        private static TerminDTO MapToTerminDto(Termin t)
        {
            return new TerminDTO
            {
                ID = t.ID,
                Datum = t.Datum,
                VrijemePocetka = t.VrijemePocetka,
                VrijemeKraja = t.VrijemeKraja,
                KabinetID = t.KabinetID,
                KabinetNaziv = t.Kabinet?.Naziv ?? "N/A",
                KreatorID = t.KreatorID,
                KreatorIme = t.Kreator?.ImePrezime ?? "N/A",
                StatusTermina = t.StatusTermina.ToString(),
                ProfesorIme = t.Profesor?.ImePrezime,
                LimitOsoba = t.LimitOsoba,
                VidljivoStudentima = t.VidljivoStudentima,
                BrojOdobrenih = t.Zahtjevi?.Count(z => z.StatusZahtjeva == StatusZahtjeva.Odobren) ?? 0
            };
        }
    }
}

