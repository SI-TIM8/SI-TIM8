using System;
using System.Threading.Tasks;
using LABsistem.Api.Validators;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LabSistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class RezervacijaValidatorTests
    {
        private LabSistemDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LabSistemDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LabSistemDbContext(options);
        }

        [Fact]
        public async Task ValidateRezervacija_ThrowsException_WhenLimitOsobaExceedsKapacitet()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var kabinet = new Kabinet { ID = 1, Naziv = "Kabinet 1", Kapacitet = 10, KorisnikID = 1, ObjekatID = 1 };
            var termin = new Termin 
            { 
                ID = 1, 
                KabinetID = 1, 
                KreatorID = 1,
                StatusTermina = StatusTermina.Slobodan,
                Datum = DateTime.Now.AddDays(1),
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(12, 0, 0)
            };
            context.Kabineti.Add(kabinet);
            context.Termini.Add(termin);
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => 
                validator.ValidateRezervacija(terminId: 1, profesorId: 1, limitOsoba: 15));
            
            Assert.Contains("ne može biti veći od kapaciteta kabineta", ex.Message);
        }

        [Fact]
        public async Task ValidateRezervacija_Succeeds_WhenLimitOsobaWithinKapacitet()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var kabinet = new Kabinet { ID = 1, Naziv = "Kabinet 1", Kapacitet = 20, KorisnikID = 1, ObjekatID = 1 };
            var termin = new Termin 
            { 
                ID = 1, 
                KabinetID = 1, 
                KreatorID = 1,
                StatusTermina = StatusTermina.Slobodan,
                Datum = DateTime.Now.AddDays(1),
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(12, 0, 0)
            };
            context.Kabineti.Add(kabinet);
            context.Termini.Add(termin);
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            // Act
            await validator.ValidateRezervacija(terminId: 1, profesorId: 1, limitOsoba: 15);

            // Assert: No exception thrown
        }

        [Fact]
        public async Task ValidateOtkazivanje_ThrowsException_WhenLessThan24Hours()
        {
            using var context = GetInMemoryDbContext();
            var terminStart = DateTime.Now.AddHours(10);
            var termin = new Termin
            {
                ID = 2,
                ProfesorID = 2,
                KreatorID = 1,
                Datum = terminStart.Date,
                VrijemePocetka = terminStart.TimeOfDay,
                VrijemeKraja = terminStart.AddHours(1).TimeOfDay,
                StatusTermina = StatusTermina.Rezervisan
            };
            context.Termini.Add(termin);
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            var ex = await Assert.ThrowsAsync<Exception>(() => validator.ValidateOtkazivanje(2, 2));

            Assert.Equal("Termin se može otkazati najkasnije 24h ranije.", ex.Message);
        }

        [Fact]
        public async Task ValidateZahtjev_ThrowsException_WhenTerminNotVisible()
        {
            using var context = GetInMemoryDbContext();
            var termin = new Termin
            {
                ID = 3,
                KreatorID = 1,
                Datum = DateTime.Now.AddDays(2),
                VrijemePocetka = new TimeSpan(9, 0, 0),
                VrijemeKraja = new TimeSpan(10, 0, 0),
                StatusTermina = StatusTermina.Rezervisan,
                VidljivoStudentima = false
            };
            context.Termini.Add(termin);
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            var ex = await Assert.ThrowsAsync<Exception>(() => validator.ValidateZahtjev(10, 3));

            Assert.Equal("Termin nije vidljiv studentima.", ex.Message);
        }

        [Fact]
        public async Task ValidateOdgovor_ThrowsException_WhenLimitReached()
        {
            using var context = GetInMemoryDbContext();
            var termin = new Termin
            {
                ID = 4,
                ProfesorID = 5,
                KreatorID = 1,
                Datum = DateTime.Now.AddDays(2),
                VrijemePocetka = new TimeSpan(11, 0, 0),
                VrijemeKraja = new TimeSpan(12, 0, 0),
                StatusTermina = StatusTermina.Rezervisan,
                LimitOsoba = 1
            };
            context.Termini.Add(termin);
            context.Zahtjevi.Add(new Zahtjev
            {
                TerminID = 4,
                StudentID = 15,
                StatusZahtjeva = StatusZahtjeva.Odobren,
                Komentar = string.Empty
            });
            context.Zahtjevi.Add(new Zahtjev
            {
                ID = 6,
                TerminID = 4,
                StudentID = 16,
                StatusZahtjeva = StatusZahtjeva.NaCekanju,
                Komentar = string.Empty
            });
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            var ex = await Assert.ThrowsAsync<Exception>(() => validator.ValidateOdgovor(6, 5, true));

            Assert.Equal("Termin je popunjen.", ex.Message);
        }
    }
}
