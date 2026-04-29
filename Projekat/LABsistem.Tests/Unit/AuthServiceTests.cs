using AutoFixture;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Bll.Models;
using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AuthServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly JwtSettings _jwtSettings;

    public AuthServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().FirstOrDefault());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _jwtServiceMock = new Mock<IJwtService>(MockBehavior.Strict);
        _jwtSettings = new JwtSettings
        {
            Key = "TestSuperSecretKeyThatMustBeLongEnough123!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpireMinutes = 30,
            RefreshExpireDays = 7
        };
    }

    private static LabSistemDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LabSistemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LabSistemDbContext(options);
    }

    private AuthService CreateService(LabSistemDbContext context)
    {
        return new AuthService(context, _jwtServiceMock.Object, _jwtSettings);
    }

    [Fact]
    public async Task LoginAsync_WithValidHashedPassword_ReturnsAccessAndRefreshTokensAndPersistsSession()
    {
        using var context = GetInMemoryDbContext();
        const string rawPassword = "ValidPassword123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        var korisnik = _fixture.Build<Korisnik>()
            .With(k => k.Password, hashedPassword)
            .With(k => k.Username, "ValidUser123")
            .With(k => k.Email, "valid.user@test.com")
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        _jwtServiceMock.Setup(x => x.GenerateToken(korisnik.ID.ToString(), korisnik.Username, korisnik.Uloga.ToString()))
            .Returns("access-token");
        _jwtServiceMock.Setup(x => x.GetTokenExpirationUtc("access-token"))
            .Returns(new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var service = CreateService(context);

        var result = await service.LoginAsync(
            new LoginRequestDto { Username = korisnik.Username, Password = rawPassword },
            "127.0.0.1",
            "UnitTestAgent");

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Token);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.Equal(korisnik.ID, result.UserId);

        var persistedRefreshToken = await context.RefreshTokens.SingleAsync();
        Assert.Equal(korisnik.ID, persistedRefreshToken.KorisnikID);
        Assert.Equal("127.0.0.1", persistedRefreshToken.IpAddress);
        Assert.Equal("UnitTestAgent", persistedRefreshToken.DeviceInfo);
        Assert.Null(persistedRefreshToken.RevokedAtUtc);
    }

    [Fact]
    public async Task LoginAsync_WithUnhashedPassword_HashesPasswordAndReturnsSession()
    {
        using var context = GetInMemoryDbContext();
        const string plainPassword = "PlainPassword123!";

        var korisnik = _fixture.Build<Korisnik>()
            .With(k => k.Password, plainPassword)
            .With(k => k.Username, "LegacyUser123")
            .With(k => k.Email, "legacy.user@test.com")
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        _jwtServiceMock.Setup(x => x.GenerateToken(korisnik.ID.ToString(), korisnik.Username, korisnik.Uloga.ToString()))
            .Returns("legacy-access-token");
        _jwtServiceMock.Setup(x => x.GetTokenExpirationUtc("legacy-access-token"))
            .Returns(new DateTime(2030, 1, 1, 13, 0, 0, DateTimeKind.Utc));
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("legacy-refresh-token");

        var service = CreateService(context);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Username = korisnik.Username,
            Password = plainPassword
        });

        Assert.NotNull(result);
        Assert.Equal("legacy-access-token", result.Token);

        var updatedUser = await context.Korisnici.FindAsync(korisnik.ID);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(plainPassword, updatedUser.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, updatedUser.Password));
        Assert.Single(context.RefreshTokens);
    }

    [Fact]
    public async Task RefreshAsync_WithValidRefreshToken_RotatesTokenAndRevokesPreviousOne()
    {
        using var context = GetInMemoryDbContext();
        const string rawPassword = "RefreshPassword123!";

        var korisnik = _fixture.Build<Korisnik>()
            .With(k => k.Password, BCrypt.Net.BCrypt.HashPassword(rawPassword))
            .With(k => k.Username, "RefreshUser123")
            .With(k => k.Email, "refresh.user@test.com")
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        _jwtServiceMock.SetupSequence(x => x.GenerateToken(korisnik.ID.ToString(), korisnik.Username, korisnik.Uloga.ToString()))
            .Returns("access-token-1")
            .Returns("access-token-2");
        _jwtServiceMock.SetupSequence(x => x.GetTokenExpirationUtc(It.IsAny<string>()))
            .Returns(new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc))
            .Returns(new DateTime(2030, 1, 1, 13, 0, 0, DateTimeKind.Utc));
        _jwtServiceMock.SetupSequence(x => x.GenerateRefreshToken())
            .Returns("refresh-token-1")
            .Returns("refresh-token-2");

        var service = CreateService(context);

        var loginResponse = await service.LoginAsync(new LoginRequestDto
        {
            Username = korisnik.Username,
            Password = rawPassword
        });

        var refreshResponse = await service.RefreshAsync(loginResponse!.RefreshToken, "127.0.0.1", "RefreshAgent");

        Assert.NotNull(refreshResponse);
        Assert.Equal("access-token-2", refreshResponse.Token);
        Assert.Equal("refresh-token-2", refreshResponse.RefreshToken);

        var refreshTokens = await context.RefreshTokens
            .OrderBy(x => x.ID)
            .ToListAsync();

        Assert.Equal(2, refreshTokens.Count);
        Assert.NotNull(refreshTokens[0].RevokedAtUtc);
        Assert.NotNull(refreshTokens[0].ReplacedByTokenHash);
        Assert.Null(refreshTokens[1].RevokedAtUtc);
        Assert.Equal("127.0.0.1", refreshTokens[1].IpAddress);
        Assert.Equal("RefreshAgent", refreshTokens[1].DeviceInfo);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithExistingToken_MarksItRevoked()
    {
        using var context = GetInMemoryDbContext();

        var korisnik = _fixture.Build<Korisnik>()
            .With(k => k.Password, BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"))
            .With(k => k.Username, "RevocationUser123")
            .With(k => k.Email, "revocation.user@test.com")
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        _jwtServiceMock.Setup(x => x.GenerateToken(korisnik.ID.ToString(), korisnik.Username, korisnik.Uloga.ToString()))
            .Returns("access-token");
        _jwtServiceMock.Setup(x => x.GetTokenExpirationUtc("access-token"))
            .Returns(new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token-to-revoke");

        var service = CreateService(context);
        var loginResponse = await service.LoginAsync(new LoginRequestDto
        {
            Username = korisnik.Username,
            Password = "ValidPassword123!"
        });

        var revoked = await service.RevokeRefreshTokenAsync(loginResponse!.RefreshToken);

        Assert.True(revoked);
        var storedToken = await context.RefreshTokens.SingleAsync();
        Assert.NotNull(storedToken.RevokedAtUtc);
    }

    [Fact]
    public async Task UpdateProfileAsync_WithDuplicateUsername_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();

        var user1 = _fixture.Build<Korisnik>()
            .With(k => k.Username, "PrviKorisnik123")
            .With(k => k.Email, "prvi@test.com")
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();

        var user2 = _fixture.Build<Korisnik>()
            .With(k => k.Username, "ZauzetUsername456")
            .With(k => k.Email, "drugi@test.com")
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();

        context.Korisnici.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.UpdateProfileAsync(user1.ID, new UpdateProfileRequestDto
        {
            ImePrezime = "Novo Ime",
            Email = "novo@email.com",
            Username = user2.Username
        });

        Assert.False(result.Success);
        Assert.Equal("Username je vec zauzet.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_WithInvalidPassword_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();

        var request = _fixture.Build<RegisterRequestDto>()
            .With(r => r.Username, "ValidUser123")
            .With(r => r.Email, "valid.user123@test.com")
            .With(r => r.Password, "weakpass")
            .Create();

        var service = CreateService(context);

        var result = await service.CreateUserAsync(request, UlogaKorisnika.Student);

        Assert.False(result.Success);
        Assert.Contains("Lozinka mora imati najmanje 8 znakova", result.Message);
    }
}
