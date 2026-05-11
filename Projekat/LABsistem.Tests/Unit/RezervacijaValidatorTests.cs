using System;
using System.Threading.Tasks;
using LABsistem.Api.Validators;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
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
            var kabinet = new Kabinet { ID = 1, Naziv = "Kabinet 1", Kapacitet = 10 };
            var termin = new Termin 
            { 
                ID = 1, 
                KabinetID = 1, 
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
            var kabinet = new Kabinet { ID = 1, Naziv = "Kabinet 1", Kapacitet = 20 };
            var termin = new Termin 
            { 
                ID = 1, 
                KabinetID = 1, 
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
    }
}
