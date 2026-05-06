using Microsoft.EntityFrameworkCore;
using LABsistem.Dal.Db;
using LABsistem.Dal.Repositories;
using LABsistem.Api.Services;
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
            // Arrange
            using var context = GetInMemoryDbContext();
            var repo = new OpremaRepository(context);
            var service = new OpremaService(repo);

            var oprema = new Oprema
            {
                Naziv = "Stari Naziv",
                SerijskiBroj = 123, 
                stanje = StatusOpreme.Ispravno
            };
            context.Oprema.Add(oprema);
            await context.SaveChangesAsync();

            // Detachuj entitet da bi Assert morao ponovo čitati iz baze
            context.Entry(oprema).State = EntityState.Detached;

            var updateDto = new OpremaCreateDTO
            {
                Naziv = "Novi Naziv",
                SerijskiBroj = 45,
                Stanje = (int)StatusOpreme.UKvaru
            };

            // Act
            var result = await service.AzurirajOpremu(oprema.ID, updateDto);

            // Assert
            Assert.True(result);

            // Kreiramo novi context ili čistimo stari da budemo sigurni da čitamo nove podatke
            var updatedEntity = await context.Oprema.FindAsync(oprema.ID);

            Assert.NotNull(updatedEntity);
            Assert.Equal("Novi Naziv", updatedEntity.Naziv);
            Assert.Equal(45, updatedEntity.SerijskiBroj);
            Assert.Equal(StatusOpreme.UKvaru, updatedEntity.stanje);
        }
    }
}