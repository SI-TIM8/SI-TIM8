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

        [Fact]
        public async Task ValidateZahtjev_ThrowsException_WhenMaxActiveRequestsReached()
        {
            using var context = GetInMemoryDbContext();
            var studentId = 20;

            var termin = new Termin
            {
                ID = 10,
                KreatorID = 1,
                Datum = DateTime.Today.AddDays(10),
                VrijemePocetka = new TimeSpan(9, 0, 0),
                VrijemeKraja = new TimeSpan(10, 0, 0),
                StatusTermina = StatusTermina.Rezervisan,
                VidljivoStudentima = true,
                LimitOsoba = 10
            };
            context.Termini.Add(termin);

            for (var i = 0; i < RezervacijaValidator.MaxAktivnihZahtjevaPoStudentu; i++)
            {
                var activeTermin = new Termin
                {
                    ID = 100 + i,
                    KreatorID = 1,
                    Datum = DateTime.Today.AddDays(20 + i),
                    VrijemePocetka = new TimeSpan(8, 0, 0),
                    VrijemeKraja = new TimeSpan(9, 0, 0),
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true
                };
                context.Termini.Add(activeTermin);
                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = activeTermin.ID,
                    StatusZahtjeva = i % 2 == 0 ? StatusZahtjeva.NaCekanju : StatusZahtjeva.Odobren,
                    Komentar = string.Empty
                });
            }

            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            var ex = await Assert.ThrowsAsync<Exception>(() => validator.ValidateZahtjev(studentId, termin.ID));

            Assert.Contains("Dostignut je maksimalan broj aktivnih zahtjeva", ex.Message);
        }

        [Fact]
        public async Task ValidateZahtjev_Succeeds_WhenActiveRequestsBelowLimit()
        {
            using var context = GetInMemoryDbContext();
            var studentId = 21;

            var termin = new Termin
            {
                ID = 11,
                KreatorID = 1,
                Datum = DateTime.Today.AddDays(11),
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(11, 0, 0),
                StatusTermina = StatusTermina.Rezervisan,
                VidljivoStudentima = true,
                LimitOsoba = 10
            };
            context.Termini.Add(termin);

            for (var i = 0; i < RezervacijaValidator.MaxAktivnihZahtjevaPoStudentu - 1; i++)
            {
                var activeTermin = new Termin
                {
                    ID = 200 + i,
                    KreatorID = 1,
                    Datum = DateTime.Today.AddDays(30 + i),
                    VrijemePocetka = new TimeSpan(8, 0, 0),
                    VrijemeKraja = new TimeSpan(9, 0, 0),
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true
                };
                context.Termini.Add(activeTermin);
                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = activeTermin.ID,
                    StatusZahtjeva = StatusZahtjeva.NaCekanju,
                    Komentar = string.Empty
                });
            }

            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            await validator.ValidateZahtjev(studentId, termin.ID);
        }

        [Fact]
        public async Task ValidateZahtjev_DoesNotCountCancelledOrRejectedRequests()
        {
            using var context = GetInMemoryDbContext();
            var studentId = 22;

            var termin = new Termin
            {
                ID = 12,
                KreatorID = 1,
                Datum = DateTime.Today.AddDays(12),
                VrijemePocetka = new TimeSpan(12, 0, 0),
                VrijemeKraja = new TimeSpan(13, 0, 0),
                StatusTermina = StatusTermina.Rezervisan,
                VidljivoStudentima = true,
                LimitOsoba = 10
            };
            context.Termini.Add(termin);

            for (var i = 0; i < RezervacijaValidator.MaxAktivnihZahtjevaPoStudentu; i++)
            {
                var inactiveTermin = new Termin
                {
                    ID = 300 + i,
                    KreatorID = 1,
                    Datum = DateTime.Today.AddDays(40 + i),
                    VrijemePocetka = new TimeSpan(8, 0, 0),
                    VrijemeKraja = new TimeSpan(9, 0, 0),
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true
                };
                context.Termini.Add(inactiveTermin);
                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = inactiveTermin.ID,
                    StatusZahtjeva = i % 2 == 0 ? StatusZahtjeva.Otkazan : StatusZahtjeva.Odbijen,
                    Komentar = string.Empty
                });
            }

            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            await validator.ValidateZahtjev(studentId, termin.ID);
        }

        [Fact]
        public async Task ValidateStudentOtkazivanje_ThrowsException_WhenApprovedReservationDoesNotExist()
        {
            using var context = GetInMemoryDbContext();
            var termin = new Termin
            {
                ID = 21,
                KreatorID = 1,
                ProfesorID = 2,
                Datum = DateTime.Today.AddDays(2),
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(11, 0, 0),
                StatusTermina = StatusTermina.Rezervisan
            };
            context.Termini.Add(termin);
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            var ex = await Assert.ThrowsAsync<Exception>(() => validator.ValidateStudentOtkazivanje(21, 100));

            Assert.Equal("Nemate aktivnu odobrenu rezervaciju za ovaj termin.", ex.Message);
        }

        [Fact]
        public async Task ValidateStudentOtkazivanje_ThrowsException_WhenTerminAlreadyStarted()
        {
            using var context = GetInMemoryDbContext();
            var startedAt = DateTime.Now.AddMinutes(-10);
            var termin = new Termin
            {
                ID = 22,
                KreatorID = 1,
                ProfesorID = 2,
                Datum = startedAt.Date,
                VrijemePocetka = startedAt.TimeOfDay,
                VrijemeKraja = startedAt.AddHours(1).TimeOfDay,
                StatusTermina = StatusTermina.Rezervisan
            };
            context.Termini.Add(termin);
            context.Zahtjevi.Add(new Zahtjev
            {
                StudentID = 101,
                TerminID = 22,
                StatusZahtjeva = StatusZahtjeva.Odobren,
                Komentar = string.Empty
            });
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            var ex = await Assert.ThrowsAsync<Exception>(() => validator.ValidateStudentOtkazivanje(22, 101));

            Assert.Equal("Rezervaciju je moguce otkazati samo prije pocetka termina.", ex.Message);
        }

        [Fact]
        public async Task ValidateStudentOtkazivanje_Succeeds_WhenApprovedReservationIsInFuture()
        {
            using var context = GetInMemoryDbContext();
            var termin = new Termin
            {
                ID = 23,
                KreatorID = 1,
                ProfesorID = 2,
                Datum = DateTime.Today.AddDays(3),
                VrijemePocetka = new TimeSpan(14, 0, 0),
                VrijemeKraja = new TimeSpan(15, 0, 0),
                StatusTermina = StatusTermina.Rezervisan
            };
            context.Termini.Add(termin);
            context.Zahtjevi.Add(new Zahtjev
            {
                StudentID = 102,
                TerminID = 23,
                StatusZahtjeva = StatusZahtjeva.Odobren,
                Komentar = string.Empty
            });
            await context.SaveChangesAsync();

            var validator = new RezervacijaValidator(context);

            await validator.ValidateStudentOtkazivanje(23, 102);
        }
    }
}
