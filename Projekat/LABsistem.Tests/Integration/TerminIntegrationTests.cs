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
    public class TerminIntegrationTests
    {
        private LabSistemDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LabSistemDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LabSistemDbContext(options);
        }

        private static Mock<ITerminValidator> CreateValidatorMock()
        {
            var validatorMock = new Mock<ITerminValidator>();
            validatorMock
                .Setup(v => v.ValidateCreateAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            validatorMock
                .Setup(v => v.ValidateUpdateAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            return validatorMock;
        }

        [Fact]
        public async Task KreirajTermin_TrajnoSpremaUBazu()
        {
           
            using var context = GetInMemoryDbContext();
            var service = new TerminService(new TerminRepository(context), CreateValidatorMock().Object);
            var dto = new TerminCreateDTO
            {
                Datum = DateTime.Now,
                VrijemePocetka = new TimeSpan(9, 0, 0),
                VrijemeKraja = new TimeSpan(11, 0, 0),
                KreatorID = 1
            };

           
            await service.KreirajTermin(dto);

            
            Assert.Equal(1, await context.Termini.CountAsync());
            var sacuvan = await context.Termini.FirstAsync();
            Assert.Equal(new TimeSpan(9, 0, 0), sacuvan.VrijemePocetka);
        }


        [Fact]
        public async Task AzurirajTermin_StvarnoMjenjaPodatkeUMemoriji()
        {
            
            using var context = GetInMemoryDbContext();
            var termin = new Termin { ID = 10, VrijemePocetka = new TimeSpan(8, 0, 0), Datum = DateTime.Now };
            context.Termini.Add(termin);
            await context.SaveChangesAsync();
            context.Entry(termin).State = EntityState.Detached;

            var service = new TerminService(new TerminRepository(context), CreateValidatorMock().Object);
            var updateDto = new TerminCreateDTO { VrijemePocetka = new TimeSpan(16, 0, 0), Datum = DateTime.Now };

            
            await service.AzurirajTermin(10, updateDto);

            
            var izBaze = await context.Termini.FindAsync(10);
            Assert.Equal(new TimeSpan(16, 0, 0), izBaze.VrijemePocetka);
        }

        [Fact]
        public async Task ObrisiTermin_UspjesnoUklanjaIzBaze()
        {
            
            using var context = GetInMemoryDbContext();
            var t = new Termin { ID = 50, Datum = DateTime.Now, VrijemePocetka = TimeSpan.Zero, VrijemeKraja = TimeSpan.Zero };
            context.Termini.Add(t);
            await context.SaveChangesAsync();

            var service = new TerminService(new TerminRepository(context), CreateValidatorMock().Object);

            
            await service.ObrisiTermin(50);

            Assert.Null(await context.Termini.FindAsync(50));
        }

        [Fact]
        public async Task Repository_GetById_VracaNullZaNepostojeci()
        {
            
            using var context = GetInMemoryDbContext();
            var repo = new TerminRepository(context);

            
            var result = await repo.GetByIdAsync(999);

            
            Assert.Null(result);
        }

        [Fact]
        public async Task Repository_GetAllWithDetails_KadaNemaRelacija_VracaNA()
        {
            
            using var context = GetInMemoryDbContext();
            var t = new Termin { ID = 1, KreatorID = 999, KabinetID = 999 }; // Nepostojeci ID-jevi
            context.Termini.Add(t);
            await context.SaveChangesAsync();

            var repo = new TerminRepository(context);

            var rezultati = await repo.GetAllWithDetailsAsync();
            var stavka = rezultati.First();

            
            Assert.Equal("N/A", stavka.kreatorIme);
            Assert.Equal("N/A", stavka.kabinetNaziv);
        }

        [Fact]
        public async Task KreirajTermin_KadaSePreklapaUIstomKabinetu_BacaGresku()
        {
            using var context = GetInMemoryDbContext();
            context.Termini.Add(new Termin
            {
                ID = 1,
                Datum = DateTime.Today.AddDays(1),
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(12, 0, 0),
                KreatorID = 1,
                KabinetID = 5
            });
            await context.SaveChangesAsync();

            var validator = new TerminValidator(context);
            var service = new TerminService(new TerminRepository(context), validator);

            var dto = new TerminCreateDTO
            {
                Datum = DateTime.Today.AddDays(1),
                VrijemePocetka = new TimeSpan(11, 0, 0),
                VrijemeKraja = new TimeSpan(13, 0, 0),
                KreatorID = 2,
                KabinetID = 5
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => service.KreirajTermin(dto));

            Assert.Equal("Vec postoji termin koji se vremenski preklapa u odabranom kabinetu.", exception.Message);
        }
    }
}
