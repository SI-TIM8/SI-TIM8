using Moq;
using AutoFixture;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class OpremaServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IOpremaRepository> _repoMock;
        private readonly OpremaService _service;

        public OpremaServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _repoMock = new Mock<IOpremaRepository>();
            _service = new OpremaService(_repoMock.Object);
        }

        [Fact]
        public async Task KreirajOpremu_ValidDto_ReturnsCorrectOpremaDTO()
        {
           
            var dto = _fixture.Create<OpremaCreateDTO>();

           
            var result = await _service.KreirajOpremu(dto);

            
            Assert.NotNull(result);
            Assert.Equal(dto.Naziv, result.Naziv);
            Assert.Equal(dto.SerijskiBroj, result.SerijskiBroj);
            _repoMock.Verify(x => x.AddAsync(It.IsAny<Oprema>()), Times.Once);
        }

        [Fact]
        public async Task VratiSvuOpremu_ReturnsMappedList()
        {
            
            var mockData = new List<(Oprema oprema, string kabinetNaziv, string zgradaNaziv)>
            {
                (new Oprema { ID = 1, Naziv = "Mikroskop", stanje = StatusOpreme.Ispravno }, "Kabinet A", "Zgrada 1")
            };

            _repoMock.Setup(x => x.GetAllWithKabinetAsync()).ReturnsAsync(mockData);

            
            var result = await _service.VratiSvuOpremu();

            
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Mikroskop", list[0].Naziv);
            Assert.Equal("Kabinet A", list[0].KabinetNaziv);
        }

        [Fact]
        public async Task AzurirajOpremu_NonExistingId_ReturnsFalse()
        {
            
            _repoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Oprema)null);
            var dto = _fixture.Create<OpremaCreateDTO>();

            
            var result = await _service.AzurirajOpremu(999, dto);

            
            Assert.False(result);
            _repoMock.Verify(x => x.UpdateAsync(It.IsAny<Oprema>()), Times.Never);
        }

        [Fact]
        public async Task ObrisiOpremu_ExistingId_ReturnsTrue()
        {
            var oprema = _fixture.Create<Oprema>();
            _repoMock.Setup(x => x.GetByIdAsync(oprema.ID)).ReturnsAsync(oprema);

            
            var result = await _service.ObrisiOpremu(oprema.ID);

            
            Assert.True(result);
            _repoMock.Verify(x => x.DeleteAsync(oprema.ID), Times.Once);
        }
    }
}