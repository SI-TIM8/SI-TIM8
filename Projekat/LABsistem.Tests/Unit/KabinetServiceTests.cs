using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using Moq;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class KabinetServiceTests
    {
        private readonly Mock<IKabinetRepository> _repoMock;
        private readonly KabinetService _service;

        public KabinetServiceTests()
        {
            _repoMock = new Mock<IKabinetRepository>();
            _service = new KabinetService(_repoMock.Object);
        }

        [Fact]
        public async Task VratiSveKabinete_MapsKapacitet()
        {
            // Arrange
            var kabineti = new List<(Kabinet kabinet, string odgovorniKorisnik, string objekatLokacija)>
            {
                (new Kabinet { ID = 1, Naziv = "K1", Kapacitet = 30, KorisnikID = 1, ObjekatID = 1 }, "Prof 1", "Lok 1")
            };
            _repoMock.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(kabineti);

            // Act
            var result = await _service.VratiSveKabinete();

            // Assert
            var item = result.First();
            Assert.Equal(30, item.Kapacitet);
        }

        [Fact]
        public async Task KreirajKabinet_SetsKapacitet()
        {
            // Arrange
            var dto = new KabinetCreateDTO { Naziv = "New", Kapacitet = 50, KorisnikID = 1, ObjekatID = 1 };

            // Act
            await _service.KreirajKabinet(dto);

            // Assert
            _repoMock.Verify(r => r.AddAsync(It.Is<Kabinet>(k => k.Kapacitet == 50)), Times.Once);
        }

        [Fact]
        public async Task AzurirajKabinet_UpdatesKapacitet()
        {
            // Arrange
            var existing = new Kabinet { ID = 1, Naziv = "Old", Kapacitet = 10 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            var dto = new KabinetCreateDTO { Naziv = "Updated", Kapacitet = 25, KorisnikID = 1, ObjekatID = 1 };

            // Act
            await _service.AzurirajKabinet(1, dto);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(It.Is<Kabinet>(k => k.Kapacitet == 25)), Times.Once);
        }
    }
}
