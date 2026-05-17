using Microsoft.EntityFrameworkCore;
using Moq;
using LABsistem.Dal.Db;
using LABsistem.Dal.Repositories;
using LABsistem.Api.Services;
using LABsistem.Api.Validators;
using LABsistem.Domain.Entities;
using LABsistem.Application.DTOs;
using LABsistem.Domain.Enums;
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
            var service = new OpremaService(repo, new Mock<IOpremaValidator>().Object);

            var oprema = new Oprema
            {
                Naziv = "Stari Naziv",
                Kategorija = "Stara kategorija",
                SerijskiBroj = 123,
                stanje = StatusOpreme.Ispravno,
                KabinetID = 1,
                KreatorID = 1
            };
            context.Oprema.Add(oprema);
            await context.SaveChangesAsync();

            context.Entry(oprema).State = EntityState.Detached;

            var updateDto = new OpremaCreateDTO
            {
                Naziv = "Novi Naziv",
                Kategorija = "Nova kategorija",
                SerijskiBroj = 45,
                Stanje = (int)StatusOpreme.UKvaru,
                KabinetID = 1,
                KreatorID = 1
            };

            var result = await service.AzurirajOpremu(oprema.ID, updateDto);

            Assert.True(result);

            var updatedEntity = await context.Oprema.FindAsync(oprema.ID);

            Assert.NotNull(updatedEntity);
            Assert.Equal("Novi Naziv", updatedEntity.Naziv);
            Assert.Equal("Nova kategorija", updatedEntity.Kategorija);
            Assert.Equal(123, updatedEntity.SerijskiBroj);
            Assert.Equal(StatusOpreme.UKvaru, updatedEntity.stanje);
        }
    }
}
