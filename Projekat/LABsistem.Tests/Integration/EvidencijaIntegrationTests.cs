using Microsoft.EntityFrameworkCore;
using LABsistem.Dal.Db;
using LABsistem.Dal.Repositories;
using LABsistem.Api.Services;
using LABsistem.Domain.Entities;
using LABsistem.Application.DTOs;
using Xunit;

namespace LABsistem.Tests.Integration
{
    public class EvidencijaIntegrationTests
    {
        private LabSistemDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LabSistemDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LabSistemDbContext(options);
        }

        [Fact]
        public async Task VratiSveEvidencije_UspjesnoJoinujePodatke()
        {
            
            using var context = GetInMemoryDbContext();

            var korisnik = new Korisnik { ID = 1, ImePrezime = "Jane Doe", Username = "jane", Email = "j@j.com", Password = "x" };
            var oprema = new Oprema { ID = 1, Naziv = "Mikroskop", SerijskiBroj = 12345 };
            var evidencija = new Evidencija { ID = 1, KorisnikID = 1, OpremaID = 1, Status = "Novo", Komentar = "Test" };

            context.Korisnici.Add(korisnik);
            context.Oprema.Add(oprema);
            context.Evidencije.Add(evidencija);
            await context.SaveChangesAsync();

            var repo = new EvidencijaRepository(context);
            var service = new EvidencijaService(repo);

            
            var result = await service.VratiSveEvidencije();

            
            var stavka = result.First();
            Assert.Equal("Jane Doe", stavka.KorisnikImePrezime);
            Assert.Equal("Mikroskop", stavka.OpremaNaziv);
        }

        [Fact]
        public async Task AzurirajStatus_TrajnoSnimaPromjenuUBazu()
        {
            
            using var context = GetInMemoryDbContext();

         
            var evidencija = new Evidencija
            {
                ID = 10,
                Status = "Stari Status",
                Komentar = "Inicijalni komentar", 
                KorisnikID = 1,
                OpremaID = 1
            };

            context.Evidencije.Add(evidencija);
            await context.SaveChangesAsync();
            context.Entry(evidencija).State = EntityState.Detached;

            var repo = new EvidencijaRepository(context);
            var service = new EvidencijaService(repo);

           
            await service.AzurirajStatus(10, "Novi Status");

           
            var izBaze = await context.Evidencije.AsNoTracking().FirstOrDefaultAsync(e => e.ID == 10);
            Assert.NotNull(izBaze);
            Assert.Equal("Novi Status", izBaze.Status);
        }

        [Fact]
        public async Task ObrisiEvidenciju_UklanjaRecordIzBaze()
        {
            
            using var context = GetInMemoryDbContext();

            
            var evidencija = new Evidencija
            {
                ID = 5,
                Status = "Za brisanje",
                Komentar = "Privremeni komentar", 
                KorisnikID = 1,
                OpremaID = 1
            };

            context.Evidencije.Add(evidencija);
            await context.SaveChangesAsync();

            var repo = new EvidencijaRepository(context);
            var service = new EvidencijaService(repo);

           
            await service.ObrisiEvidenciju(5);

            
            var postojeca = await context.Evidencije.FindAsync(5);
            Assert.Null(postojeca);
        }
    }
}