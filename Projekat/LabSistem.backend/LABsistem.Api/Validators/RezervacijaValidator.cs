using System;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Dal.Db;
using LabSistem.Domain.Enums;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Api.Validators
{
    public interface IRezervacijaValidator
    {
        Task ValidateRezervacija(int terminId, int profesorId, int limitOsoba);
        Task ValidateOtkazivanje(int terminId, int profesorId);
        Task ValidateZahtjev(int studentId, int terminId);
        Task ValidateOdgovor(int zahtjevId, int profesorId, bool odobri);
    }

    public class RezervacijaValidator : IRezervacijaValidator
    {
        private readonly LabSistemDbContext _context;

        public RezervacijaValidator(LabSistemDbContext context)
        {
            _context = context;
        }

        public async Task ValidateRezervacija(int terminId, int profesorId, int limitOsoba)
        {
            var termin = await _context.Termini
                .Include(t => t.Kabinet)
                .FirstOrDefaultAsync(t => t.ID == terminId);

            if (termin == null)
                throw new Exception("Termin ne postoji.");

            if (termin.Kabinet == null)
                throw new Exception("Kabinet za ovaj termin ne postoji.");

            if (limitOsoba > termin.Kabinet.Kapacitet)
                throw new Exception($"Limit osoba ({limitOsoba}) ne može biti veći od kapaciteta kabineta ({termin.Kabinet.Kapacitet}).");

            if (termin.StatusTermina != StatusTermina.Slobodan)
                throw new Exception("Termin je već rezervisan.");

            var profesorKonflikt = await _context.Termini.AnyAsync(t =>
                t.ProfesorID == profesorId &&
                t.StatusTermina == StatusTermina.Rezervisan &&
                t.Datum == termin.Datum &&
                t.VrijemePocetka < termin.VrijemeKraja &&
                termin.VrijemePocetka < t.VrijemeKraja
            );

            if (profesorKonflikt)
                throw new Exception("Profesor već ima termin u ovom vremenu na ovaj dan.");

            if (limitOsoba < 0)
                throw new Exception("Limit osoba ne može biti negativan broj.");
        }

        public async Task ValidateOtkazivanje(int terminId, int profesorId)
        {
            var termin = await _context.Termini.FirstOrDefaultAsync(t => t.ID == terminId);

            if (termin == null)
                throw new Exception("Termin ne postoji.");

            if (termin.ProfesorID != profesorId)
                throw new Exception("Nemate pravo otkazati ovaj termin.");

            var terminStart = termin.Datum.Date.Add(termin.VrijemePocetka);
            if (terminStart <= DateTime.Now.AddHours(24))
                throw new Exception("Termin se može otkazati najkasnije 24h ranije.");
        }

        public async Task ValidateZahtjev(int studentId, int terminId)
        {
            var termin = await _context.Termini.FirstOrDefaultAsync(t => t.ID == terminId);

            if (termin == null)
                throw new Exception("Termin ne postoji.");

            if (termin.StatusTermina != StatusTermina.Rezervisan)
                throw new Exception("Termin nije rezervisan.");

            if (!termin.VidljivoStudentima)
                throw new Exception("Termin nije vidljiv studentima.");

            var postojiPrijava = await _context.Zahtjevi.AnyAsync(z =>
                z.StudentID == studentId &&
                z.TerminID == terminId &&
                z.StatusZahtjeva != StatusZahtjeva.Otkazan
            );

            if (postojiPrijava)
                throw new Exception("Već ste poslali zahtjev za ovaj termin.");

            var brojOdobrenih = await _context.Zahtjevi.CountAsync(z =>
                z.TerminID == terminId &&
                z.StatusZahtjeva == StatusZahtjeva.Odobren
            );

            if (termin.LimitOsoba.HasValue && brojOdobrenih >= termin.LimitOsoba.Value)
                throw new Exception("Termin je popunjen.");

            var konflikt = await _context.Zahtjevi
                .Include(z => z.Termin)
                .AnyAsync(z =>
                    z.StudentID == studentId &&
                    z.StatusZahtjeva == StatusZahtjeva.Odobren &&
                    z.Termin.Datum == termin.Datum &&
                    z.Termin.VrijemePocetka < termin.VrijemeKraja &&
                    termin.VrijemePocetka < z.Termin.VrijemeKraja
                );

            if (konflikt)
                throw new Exception("Student već ima drugi termin u ovom vremenu.");
        }

        public async Task ValidateOdgovor(int zahtjevId, int profesorId, bool odobri)
        {
            var zahtjev = await _context.Zahtjevi
                .Include(z => z.Termin)
                .FirstOrDefaultAsync(z => z.ID == zahtjevId);

            if (zahtjev == null)
                throw new Exception("Zahtjev ne postoji.");

            if (zahtjev.Termin.ProfesorID != profesorId)
                throw new Exception("Nemate pravo upravljati ovim zahtjevom.");

            if (odobri)
            {
                var brojOdobrenih = await _context.Zahtjevi.CountAsync(z =>
                    z.TerminID == zahtjev.TerminID &&
                    z.StatusZahtjeva == StatusZahtjeva.Odobren
                );

                if (zahtjev.Termin.LimitOsoba.HasValue &&
                    brojOdobrenih >= zahtjev.Termin.LimitOsoba.Value)
                {
                    throw new Exception("Termin je popunjen.");
                }

                var konflikt = await _context.Zahtjevi
                    .Include(z => z.Termin)
                    .AnyAsync(z =>
                        z.StudentID == zahtjev.StudentID &&
                        z.StatusZahtjeva == StatusZahtjeva.Odobren &&
                        z.Termin.Datum == zahtjev.Termin.Datum &&
                        z.Termin.VrijemePocetka < zahtjev.Termin.VrijemeKraja &&
                        zahtjev.Termin.VrijemePocetka < z.Termin.VrijemeKraja
                    );

                if (konflikt)
                    throw new Exception("Student već ima drugi termin u ovom vremenu.");
            }
        }
    }
}
