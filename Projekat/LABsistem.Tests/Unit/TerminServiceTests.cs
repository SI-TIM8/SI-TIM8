using Moq;
using AutoFixture;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Api.Validators;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class TerminServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ITerminRepository> _repoMock;
        private readonly Mock<ITerminValidator> _validatorMock;
        private readonly TerminService _service;

        public TerminServiceTests()
        {
            _fixture = new Fixture();
            
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _repoMock = new Mock<ITerminRepository>();
            _validatorMock = new Mock<ITerminValidator>();
            _validatorMock
                .Setup(v => v.ValidateCreateAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            _validatorMock
                .Setup(v => v.ValidateUpdateAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            _service = new TerminService(_repoMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task VratiSveTermine_VracaIspravnoMapiranuListu()
        {
            
            var termin = new Termin { ID = 1, VrijemePocetka = new TimeSpan(8, 0, 0), Datum = DateTime.Today };
            var mockData = new List<(Termin termin, string kreatorIme, string kabinetNaziv)>
            {
                (termin, "Marko Markovic", "Kabinet 101")
            };
            _repoMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(mockData);

            var result = await _service.VratiSveTermine();

            
            var stavka = result.First();
            Assert.Equal("Marko Markovic", stavka.KreatorIme);
            Assert.Equal("Kabinet 101", stavka.KabinetNaziv);
            Assert.Equal(termin.ID, stavka.ID);
        }

        [Fact]
        public async Task KreirajTermin_IspravnoProslijedjujePodatkeRepozitoriju()
        {
            
            var dto = new TerminCreateDTO
            {
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(12, 0, 0),
                Datum = DateTime.Now,
                KreatorID = 1,
                KabinetID = 2
            };

            
            await _service.KreirajTermin(dto);

            _validatorMock.Verify(v => v.ValidateCreateAsync(
                dto.Datum,
                dto.VrijemePocetka,
                dto.VrijemeKraja,
                dto.KabinetID), Times.Once);
            _repoMock.Verify(r => r.AddAsync(It.Is<Termin>(t =>
                t.VrijemePocetka == dto.VrijemePocetka &&
                t.KreatorID == dto.KreatorID)), Times.Once);
        }

        [Fact]
        public async Task AzurirajTermin_KadaTerminPostoji_VracaTrue()
        {
            
            var postojeci = new Termin { ID = 5 };
            var dto = _fixture.Create<TerminCreateDTO>();
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(postojeci);

            
            var result = await _service.AzurirajTermin(5, dto);

            Assert.True(result);
            _validatorMock.Verify(v => v.ValidateUpdateAsync(
                5,
                dto.Datum,
                dto.VrijemePocetka,
                dto.VrijemeKraja,
                dto.KabinetID), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(postojeci), Times.Once);
        }

        [Fact]
        public async Task AzurirajTermin_KadaTerminNePostoji_VracaFalse()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Termin)null);

            
            var result = await _service.AzurirajTermin(99, new TerminCreateDTO());

            
            Assert.False(result);
        }

        [Fact]
        public async Task ObrisiTermin_KadaTerminPostoji_VracaTrue()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Termin { ID = 1 });

            
            var result = await _service.ObrisiTermin(1);

            
            Assert.True(result);
            _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObrisiTermin_KadaTerminNePostoji_VracaFalse()
        {
            
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Termin)null);

            
            var result = await _service.ObrisiTermin(1);

            
            Assert.False(result);
        }

        [Fact]
        public async Task VratiSveTermine_KadaJeBazaPrazna_VracaPraznuListu()
        {
            
            _repoMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(new List<(Termin, string, string)>());

            
            var result = await _service.VratiSveTermine();

            Assert.Empty(result);
        }

        [Fact]
        public async Task KreirajTermin_MapiraDTOuEntitetIspravno()
        {
            
            var dto = _fixture.Create<TerminCreateDTO>();
            Termin mapiraniEntitet = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Termin>()))
                     .Callback<Termin>(t => mapiraniEntitet = t);

          
            await _service.KreirajTermin(dto);

            
            Assert.Equal(dto.Datum, mapiraniEntitet.Datum);
            Assert.Equal(dto.VrijemePocetka, mapiraniEntitet.VrijemePocetka);
        }

        [Fact]
        public async Task AzurirajTermin_MjenjaVrijemeKraja()
        {
           
            var t = new Termin { ID = 1, VrijemeKraja = new TimeSpan(10, 0, 0) };
            var dto = new TerminCreateDTO { VrijemeKraja = new TimeSpan(15, 0, 0) };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(t);

            
            await _service.AzurirajTermin(1, dto);

          
            Assert.Equal(new TimeSpan(15, 0, 0), t.VrijemeKraja);
        }
    }
}
