using Moq;
using AutoFixture;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class EvidencijaServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IEvidencijaRepository> _repoMock;
        private readonly EvidencijaService _service;

        public EvidencijaServiceTests()
        {
            _fixture = new Fixture();
            // Izbjegavanje cirkularnih referenci u entitetima
            _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _repoMock = new Mock<IEvidencijaRepository>();
            _service = new EvidencijaService(_repoMock.Object);
        }

        [Fact]
        public async Task VratiSveEvidencije_VracaMapiranuListuDTOa()
        {
            
            var evidencija = _fixture.Create<Evidencija>();
            var mockData = new List<(Evidencija evidencija, string opremaNaziv, string? opremaKategorija, int opremaSerijskiBroj, int opremaStanje, int opremaKabinetID, string? opremaKabinetNaziv, string? opremaZgradaNaziv, string korisnikImePrezime, string? profesorImePrezime, string? obradioImePrezime, DateTime? terminDatum, TimeSpan? terminVrijemePocetka, TimeSpan? terminVrijemeKraja)>
            {
                (evidencija, "Test Oprema", "Mjerni uredjaj", 123, 1, 7, "Kabinet A", "Zgrada 1", "Marko Markovic", null, null, null, null, null)
            };

            _repoMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(mockData);

            
            var result = await _service.VratiSveEvidencije();

            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Test Oprema", list[0].OpremaNaziv);
            Assert.Equal("Marko Markovic", list[0].KorisnikImePrezime);
            Assert.Equal(evidencija.Komentar, list[0].Komentar);
        }

        [Fact]
        public async Task KreirajEvidenciju_PozivaAddAsyncURepozitoriju()
        {
            var dto = _fixture.Create<EvidencijaCreateDTO>();

            
            await _service.KreirajEvidenciju(dto);

            
            _repoMock.Verify(r => r.AddAsync(It.Is<Evidencija>(e =>
                e.Status == dto.Status &&
                e.OpremaID == dto.OpremaID)), Times.Once);
        }

        [Fact]
        public async Task AzurirajStatus_PostojeciId_MijenjaStatus()
        {
            
            var postojeca = _fixture.Create<Evidencija>();
            _repoMock.Setup(r => r.GetByIdAsync(postojeca.ID)).ReturnsAsync(postojeca);

            
            await _service.AzurirajStatus(postojeca.ID, "Završeno");

            
            Assert.Equal("Završeno", postojeca.Status);
            _repoMock.Verify(r => r.UpdateAsync(postojeca), Times.Once);
        }
    }
}