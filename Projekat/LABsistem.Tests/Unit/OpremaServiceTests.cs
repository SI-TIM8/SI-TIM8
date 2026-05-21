using AutoFixture;
using LABsistem.Api.Services;
using LABsistem.Api.Validators;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Moq;
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
            var existingOprema = new List<Oprema>
            {
                new Oprema
                {
                    ID = 1,
                    Naziv = "Postojeća oprema",
                    Kategorija = "Mjerni uređaj",
                    SerijskiBroj = 4,
                    stanje = StatusOpreme.Ispravno,
                    KabinetID = 1,
                    KreatorID = 1
                }
            };
            _repoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(existingOprema);

            var dto = _fixture.Create<OpremaCreateDTO>();

            var result = await _service.KreirajOpremu(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Naziv, result.Naziv);
            Assert.Equal(dto.Kategorija, result.Kategorija);
            Assert.Equal(5, result.SerijskiBroj);
            Assert.False(result.IsArchived);
            _repoMock.Verify(x => x.AddAsync(It.IsAny<Oprema>()), Times.Once);
        }

        [Fact]
        public async Task VratiSvuOpremu_AktivnaLista_ExcludesArchivedEquipment()
        {
            var mockData = new List<(Oprema oprema, string kabinetNaziv, string zgradaNaziv)>
            {
                (
                    new Oprema
                    {
                        ID = 1,
                        Naziv = "Mikroskop",
                        Kategorija = "Optička oprema",
                        stanje = StatusOpreme.Ispravno,
                        IsArchived = false
                    },
                    "Kabinet A",
                    "Zgrada 1"
                ),
                (
                    new Oprema
                    {
                        ID = 2,
                        Naziv = "Stari projektor",
                        Kategorija = "AV oprema",
                        stanje = StatusOpreme.Otpisano,
                        IsArchived = true
                    },
                    "Kabinet B",
                    "Zgrada 2"
                )
            };

            _repoMock.Setup(x => x.GetAllWithKabinetAsync()).ReturnsAsync(mockData);

            var result = await _service.VratiSvuOpremu("aktivna");

            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("Mikroskop", list[0].Naziv);
            Assert.False(list[0].IsArchived);
        }

        [Fact]
        public async Task AzurirajOpremu_NonExistingId_ReturnsFalse()
        {
            _repoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Oprema?)null);
            var dto = _fixture.Create<OpremaCreateDTO>();

            var result = await _service.AzurirajOpremu(999, dto);

            Assert.False(result);
            _repoMock.Verify(x => x.UpdateAsync(It.IsAny<Oprema>()), Times.Never);
        }

        [Fact]
        public async Task ArhivirajOpremu_ExistingId_SetsArchiveFlags()
        {
            var oprema = new Oprema
            {
                ID = 7,
                Naziv = "Laptop",
                Kategorija = "Računari",
                SerijskiBroj = 77,
                stanje = StatusOpreme.Ispravno,
                IsArchived = false
            };

            _repoMock.Setup(x => x.GetByIdAsync(oprema.ID)).ReturnsAsync(oprema);

            var result = await _service.ArhivirajOpremu(oprema.ID);

            Assert.True(result);
            Assert.True(oprema.IsArchived);
            Assert.NotNull(oprema.ArchivedAtUtc);
            _repoMock.Verify(x => x.UpdateAsync(oprema), Times.Once);
        }

        [Fact]
        public async Task VratiIzArhive_ExistingId_ClearsArchiveFlags()
        {
            var oprema = new Oprema
            {
                ID = 8,
                Naziv = "Projektor",
                Kategorija = "AV oprema",
                SerijskiBroj = 88,
                stanje = StatusOpreme.Ispravno,
                IsArchived = true,
                ArchivedAtUtc = DateTime.UtcNow
            };

            _repoMock.Setup(x => x.GetByIdAsync(oprema.ID)).ReturnsAsync(oprema);

            var result = await _service.VratiIzArhive(oprema.ID);

            Assert.True(result);
            Assert.False(oprema.IsArchived);
            Assert.Null(oprema.ArchivedAtUtc);
            _repoMock.Verify(x => x.UpdateAsync(oprema), Times.Once);
        }
    }
}
