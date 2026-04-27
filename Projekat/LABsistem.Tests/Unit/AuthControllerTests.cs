using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LABsistem.Presentation.Controllers;
using LABsistem.Presentation.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class AuthControllerTests : IDisposable
    {
        private readonly LabSistemDbContext _dbContext;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
           
            var options = new DbContextOptionsBuilder<LabSistemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new LabSistemDbContext(options);
            _mockJwtService = new Mock<IJwtService>();

            _controller = new AuthController(_dbContext, _mockJwtService.Object);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _dbContext.Korisnici.Add(new Korisnik
            {
                ID = 1,
                ImePrezime = "John Doe",
                Email = "john.doe@test.com",
                Username = "johndoe",
                Password = "securepassword",
                Uloga = UlogaKorisnika.Admin
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "johndoe", Password = "securepassword" };
            var expectedToken = "mocked-jwt-token";

            _mockJwtService.Setup(x => x.GenerateToken("1", "johndoe", "Admin"))
                           .Returns(expectedToken);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);

            Assert.Equal(expectedToken, response.Token);
            Assert.Equal(1, response.UserId);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "nonexistent", Password = "wrongpassword" };

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            
            Assert.Equal("Pogrešni kredencijali.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WithEmptyRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequestDto { Username = "", Password = "" };

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
          
            Assert.Equal("Username i password su obavezni.", badRequestResult.Value);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}