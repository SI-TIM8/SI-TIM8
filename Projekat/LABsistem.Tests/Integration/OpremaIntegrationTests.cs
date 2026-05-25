using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Db;
using LABsistem.Dal.Repositories;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LABsistem.Tests.Integration
{
    public class OpremaIntegrationTests
    {
        private LabSistemDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LabSistemDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LabSistemDbContext(options);
        }

        [Fact]
        public async Task AzurirajOpremu_ActuallyUpdatesDataInDatabase()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OpremaRepository(context);
            var service = new OpremaService(repo, new Mock<LABsistem.Api.Validators.IOpremaValidator>().Object);

            var oprema = new Oprema
            {
                Naziv = "Stari naziv",
                Kategorija = "Stara kategorija",
                SerijskiBroj = 123,
                stanje = StatusOpreme.Ispravno,
                KabinetID = 1,
                KreatorID = 1,
                IsArchived = false
            };
            context.Oprema.Add(oprema);
            await context.SaveChangesAsync();

            context.Entry(oprema).State = EntityState.Detached;

            var updateDto = new OpremaCreateDTO
            {
                Naziv = "Novi naziv",
                Kategorija = "Nova kategorija",
                SerijskiBroj = 45,
                Stanje = (int)StatusOpreme.UKvaru,
                KabinetID = 1,
                KreatorID = 1,
                DokumentacijaUrl = "https://example.com/docs.pdf"
            };

            var result = await service.AzurirajOpremu(oprema.ID, updateDto);

            Assert.True(result);

            var updatedEntity = await context.Oprema.FindAsync(oprema.ID);

            Assert.NotNull(updatedEntity);
            Assert.Equal("Novi naziv", updatedEntity!.Naziv);
            Assert.Equal("Nova kategorija", updatedEntity.Kategorija);
            Assert.Equal(123, updatedEntity.SerijskiBroj);
            Assert.Equal(StatusOpreme.UKvaru, updatedEntity.stanje);
        }

        [Fact]
        public async Task ArhivirajIObnoviOpremu_PersistiraArchiveFlags()
        {
            using var context = GetInMemoryDbContext();
            var repo = new OpremaRepository(context);
            var service = new OpremaService(repo, new Mock<LABsistem.Api.Validators.IOpremaValidator>().Object);

            var oprema = new Oprema
            {
                Naziv = "Arhivska oprema",
                Kategorija = "Test kategorija",
                SerijskiBroj = 222,
                stanje = StatusOpreme.Ispravno,
                KabinetID = 1,
                KreatorID = 1
            };
            context.Oprema.Add(oprema);
            await context.SaveChangesAsync();

            var arhivirano = await service.ArhivirajOpremu(oprema.ID);
            Assert.True(arhivirano);

            var nakonArhiviranja = await context.Oprema.FindAsync(oprema.ID);
            Assert.NotNull(nakonArhiviranja);
            Assert.True(nakonArhiviranja!.IsArchived);
            Assert.NotNull(nakonArhiviranja.ArchivedAtUtc);

            var vraceno = await service.VratiIzArhive(oprema.ID);
            Assert.True(vraceno);

            var nakonObnove = await context.Oprema.FindAsync(oprema.ID);
            Assert.NotNull(nakonObnove);
            Assert.False(nakonObnove!.IsArchived);
            Assert.Null(nakonObnove.ArchivedAtUtc);
        }
    }
}