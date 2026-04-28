using AutoFixture;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class AuthServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IJwtService> _jwtServiceMock;

    public AuthServiceTests()
    {
        _fixture = new Fixture();
        // Sprečavanje grešaka zbog rekurzivnih ovisnosti u entitetima
        _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _jwtServiceMock = new Mock<IJwtService>();
    }

    private LabSistemDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LabSistemDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new LabSistemDbContext(options);
    }

    [Fact]
    public async Task LoginAsync_WithValidHashedPassword_ReturnsToken()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var rawPassword = "ValidPassword123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        var korisnik = _fixture.Build<Korisnik>()
            .With(k => k.Password, hashedPassword)
            .Create();

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        var request = new LoginRequestDto { Username = korisnik.Username, Password = rawPassword };
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("jwt-token");

        var service = new AuthService(context, _jwtServiceMock.Object);

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(korisnik.ID, result.UserId);
    }

    [Fact]
    public async Task LoginAsync_WithUnHashedPassword_HashesPasswordAndReturnsToken()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var plainPassword = "PlainPassword123!";

        var korisnik = _fixture.Build<Korisnik>()
            .With(k => k.Password, plainPassword) // Neprocesirani string okida SaltParseException
            .Create();

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        var request = new LoginRequestDto { Username = korisnik.Username, Password = plainPassword };
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("jwt-token");

        var service = new AuthService(context, _jwtServiceMock.Object);

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);

        var updatedUser = await context.Korisnici.FindAsync(korisnik.ID);
        Assert.NotEqual(plainPassword, updatedUser.Password); // Provjera da je lozinka hashirana
    }

    [Fact]
    public async Task UpdateProfileAsync_WithDuplicateUsername_ReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();

        // Eksplicitno postavljamo Username bez specijalnih znakova kako bismo prošli Regex validaciju
        var user1 = _fixture.Build<Korisnik>()
            .With(k => k.Username, "PrviKorisnik123")
            .Create();

        var user2 = _fixture.Build<Korisnik>()
            .With(k => k.Username, "ZauzetUsername456")
            .Create();

        context.Korisnici.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var request = new UpdateProfileRequestDto
        {
            ImePrezime = "Novo Ime",
            Email = "test@email.com", // Stavljamo validan format i za email, za svaki slučaj
            Username = user2.Username // Pokušavamo iskoristiti tuđi validan username (ZauzetUsername456)
        };

        var service = new AuthService(context, _jwtServiceMock.Object);

        // Act
        var result = await service.UpdateProfileAsync(user1.ID, request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username je vec zauzet.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_WithInvalidPassword_ReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var request = _fixture.Build<RegisterRequestDto>()
            .With(r => r.Username, "ValidUser123")
            .With(r => r.Password, "weakpass") // Nedostaje veliko slovo, broj i spec. znak
            .Create();

        var service = new AuthService(context, _jwtServiceMock.Object);

        // Act
        var result = await service.CreateUserAsync(request, UlogaKorisnika.Student);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Lozinka mora imati najmanje 8 znakova", result.Message);
    }
}