using Moq;
using AutoFixture;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Api.Validators;
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
        private readonly Mock<IOpremaValidator> _validatorMock;
        private readonly OpremaService _service;

        public OpremaServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _repoMock = new Mock<IOpremaRepository>();
            _validatorMock = new Mock<IOpremaValidator>();
            _service = new OpremaService(_repoMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task KreirajOpremu_ValidDto_ReturnsCorrectOpremaDTO()
        {
            // Arrange - simulira postojeću opremu sa max serial brojem 4
            var existingOprema = new List<Oprema>
            {
                new Oprema { ID = 1, Naziv = "Postojeca oprema", Kategorija = "Mjerni uređaj", SerijskiBroj = 4, stanje = StatusOpreme.Ispravno, KabinetID = 1, KreatorID = 1 }
            };
            _repoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(existingOprema);
            
            var dto = _fixture.Create<OpremaCreateDTO>();

            // Act
            var result = await _service.KreirajOpremu(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Naziv, result.Naziv);
            Assert.Equal(dto.Kategorija, result.Kategorija);
            Assert.Equal(5, result.SerijskiBroj);  // max(4) + 1 = 5, serijski broj je sistemski generisan
            _repoMock.Verify(x => x.AddAsync(It.IsAny<Oprema>()), Times.Once);
        }

        [Fact]
        public async Task VratiSvuOpremu_ReturnsMappedList()
        {
            
            var mockData = new List<(Oprema oprema, string kabinetNaziv, string zgradaNaziv)>
            {
                (new Oprema { ID = 1, Naziv = "Mikroskop", Kategorija = "Optička oprema", stanje = StatusOpreme.Ispravno }, "Kabinet A", "Zgrada 1")
            };

            _repoMock.Setup(x => x.GetAllWithKabinetAsync()).ReturnsAsync(mockData);

            
            var result = await _service.VratiSvuOpremu();

            
            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Mikroskop", list[0].Naziv);
            Assert.Equal("Optička oprema", list[0].Kategorija);
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
