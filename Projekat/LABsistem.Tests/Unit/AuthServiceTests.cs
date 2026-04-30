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

    private Korisnik BuildUser(
        string username,
        string email,
        string password,
        UlogaKorisnika role = UlogaKorisnika.Student,
        bool isActive = true)
    {
        return _fixture.Build<Korisnik>()
            .With(k => k.Username, username)
            .With(k => k.Email, email)
            .With(k => k.Password, password)
            .With(k => k.Uloga, role)
            .With(k => k.IsActive, isActive)
            .With(k => k.DeactivatedAt, isActive ? null : DateTime.UtcNow)
            .With(k => k.RefreshTokens, new List<RefreshToken>())
            .Create();
    }

    [Fact]
    public async Task LoginAsync_WithValidHashedPassword_ReturnsAccessAndRefreshTokensAndPersistsSession()
    {
        using var context = GetInMemoryDbContext();
        const string rawPassword = "ValidPassword123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        var korisnik = BuildUser("ValidUser123", "valid.user@test.com", hashedPassword);

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
    public async Task LoginAsync_WithInactiveUser_ReturnsNull()
    {
        using var context = GetInMemoryDbContext();
        const string rawPassword = "InactivePassword123!";
        var korisnik = BuildUser(
            "InactiveUser123",
            "inactive.user@test.com",
            BCrypt.Net.BCrypt.HashPassword(rawPassword),
            isActive: false);

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Username = korisnik.Username,
            Password = rawPassword
        });

        Assert.Null(result);
        Assert.Empty(context.RefreshTokens);
    }

    [Fact]
    public async Task LoginAsync_WithUnhashedPassword_HashesPasswordAndReturnsSession()
    {
        using var context = GetInMemoryDbContext();
        const string plainPassword = "PlainPassword123!";

        var korisnik = BuildUser("LegacyUser123", "legacy.user@test.com", plainPassword);

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

        var korisnik = BuildUser(
            "RefreshUser123",
            "refresh.user@test.com",
            BCrypt.Net.BCrypt.HashPassword(rawPassword));

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
    public async Task RefreshAsync_WithInactiveUser_ReturnsNullAndRevokesToken()
    {
        using var context = GetInMemoryDbContext();
        const string rawPassword = "RefreshInactive123!";
        var korisnik = BuildUser(
            "RefreshInactiveUser",
            "refresh.inactive@test.com",
            BCrypt.Net.BCrypt.HashPassword(rawPassword));

        context.Korisnici.Add(korisnik);
        await context.SaveChangesAsync();

        _jwtServiceMock.Setup(x => x.GenerateToken(korisnik.ID.ToString(), korisnik.Username, korisnik.Uloga.ToString()))
            .Returns("access-token");
        _jwtServiceMock.Setup(x => x.GetTokenExpirationUtc("access-token"))
            .Returns(new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var service = CreateService(context);

        var loginResponse = await service.LoginAsync(new LoginRequestDto
        {
            Username = korisnik.Username,
            Password = rawPassword
        });

        korisnik.IsActive = false;
        korisnik.DeactivatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var refreshResponse = await service.RefreshAsync(loginResponse!.RefreshToken);

        Assert.Null(refreshResponse);
        var storedToken = await context.RefreshTokens.SingleAsync();
        Assert.NotNull(storedToken.RevokedAtUtc);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithExistingToken_MarksItRevoked()
    {
        using var context = GetInMemoryDbContext();

        var korisnik = BuildUser(
            "RevocationUser123",
            "revocation.user@test.com",
            BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"));

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

        var user1 = BuildUser("PrviKorisnik123", "prvi@test.com", BCrypt.Net.BCrypt.HashPassword("Valid123!"));
        var user2 = BuildUser("ZauzetUsername456", "drugi@test.com", BCrypt.Net.BCrypt.HashPassword("Valid123!"));

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
    public async Task UpdateProfileAsync_WithEmailOverMaxLength_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();

        var user = BuildUser("ProfileUser123", "profile.user@test.com", BCrypt.Net.BCrypt.HashPassword("Valid123!"));
        context.Korisnici.Add(user);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var tooLongEmail = $"{new string('a', 246)}@test.com";

        var result = await service.UpdateProfileAsync(user.ID, new UpdateProfileRequestDto
        {
            ImePrezime = "Profile User",
            Email = tooLongEmail,
            Username = "ProfileUser123"
        });

        Assert.False(result.Success);
        Assert.Equal("Email moze imati najvise 254 karaktera.", result.Message);
    }

    [Fact]
    public async Task UpdateUserAsync_WithCurrentUser_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();
        var currentUser = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("Valid123!"), UlogaKorisnika.Admin);
        context.Korisnici.Add(currentUser);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.UpdateUserAsync(currentUser.ID, currentUser.ID, new UpdateManagedUserRequestDto
        {
            ImePrezime = "Admin User",
            Email = currentUser.Email,
            Username = currentUser.Username,
            Uloga = UlogaKorisnika.Admin
        });

        Assert.False(result.Success);
        Assert.Equal("Ne mozete uredjivati vlastiti nalog kroz ovaj panel.", result.Message);
    }

    [Fact]
    public async Task UpdateUserAsync_WithEmptyNewPassword_DoesNotChangePassword()
    {
        using var context = GetInMemoryDbContext();
        const string initialPassword = "CurrentPassword123!";

        var admin = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        var targetUser = BuildUser("ManagedUser", "managed@test.com", BCrypt.Net.BCrypt.HashPassword(initialPassword));

        context.Korisnici.AddRange(admin, targetUser);
        await context.SaveChangesAsync();

        var originalHash = targetUser.Password;
        var service = CreateService(context);

        var result = await service.UpdateUserAsync(admin.ID, targetUser.ID, new UpdateManagedUserRequestDto
        {
            ImePrezime = "Managed User Updated",
            Email = "managed.updated@test.com",
            Username = "ManagedUserUpdated",
            Uloga = UlogaKorisnika.Profesor,
            NewPassword = string.Empty
        });

        Assert.True(result.Success);

        var updatedUser = await context.Korisnici.FindAsync(targetUser.ID);
        Assert.NotNull(updatedUser);
        Assert.Equal(originalHash, updatedUser.Password);
        Assert.Equal(UlogaKorisnika.Profesor, updatedUser.Uloga);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNewPassword_ChangesPassword()
    {
        using var context = GetInMemoryDbContext();
        const string newPassword = "NewPassword123!";

        var admin = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        var targetUser = BuildUser("ManagedUser", "managed@test.com", BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!"));

        context.Korisnici.AddRange(admin, targetUser);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.UpdateUserAsync(admin.ID, targetUser.ID, new UpdateManagedUserRequestDto
        {
            ImePrezime = "Managed User",
            Email = "managed@test.com",
            Username = "ManagedUser",
            Uloga = UlogaKorisnika.Student,
            NewPassword = newPassword
        });

        Assert.True(result.Success);

        var updatedUser = await context.Korisnici.FindAsync(targetUser.ID);
        Assert.NotNull(updatedUser);
        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task DeactivateUserAsync_WithRegularUser_DeactivatesAndRevokesRefreshTokens()
    {
        using var context = GetInMemoryDbContext();
        const string rawPassword = "RegularPassword123!";

        var admin = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        var targetUser = BuildUser("TargetUser", "target@test.com", BCrypt.Net.BCrypt.HashPassword(rawPassword));

        context.Korisnici.AddRange(admin, targetUser);
        await context.SaveChangesAsync();

        _jwtServiceMock.Setup(x => x.GenerateToken(targetUser.ID.ToString(), targetUser.Username, targetUser.Uloga.ToString()))
            .Returns("target-access-token");
        _jwtServiceMock.Setup(x => x.GetTokenExpirationUtc("target-access-token"))
            .Returns(new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("target-refresh-token");

        var service = CreateService(context);
        var loginResponse = await service.LoginAsync(new LoginRequestDto
        {
            Username = targetUser.Username,
            Password = rawPassword
        });

        var result = await service.DeactivateUserAsync(admin.ID, targetUser.ID);

        Assert.True(result.Success);

        var updatedUser = await context.Korisnici.FindAsync(targetUser.ID);
        Assert.NotNull(updatedUser);
        Assert.False(updatedUser.IsActive);
        Assert.NotNull(updatedUser.DeactivatedAt);

        var storedToken = await context.RefreshTokens.SingleAsync();
        Assert.Equal(loginResponse!.UserId, storedToken.KorisnikID);
        Assert.NotNull(storedToken.RevokedAtUtc);
    }

    [Fact]
    public async Task DeactivateUserAsync_WithCurrentUser_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();
        var admin = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        context.Korisnici.Add(admin);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var result = await service.DeactivateUserAsync(admin.ID, admin.ID);

        Assert.False(result.Success);
        Assert.Equal("Ne mozete deaktivirati svoj nalog.", result.Message);
    }

    [Fact]
    public async Task DeactivateUserAsync_WithAdminTarget_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();
        var currentAdmin = BuildUser("AdminOne", "admin1@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        var targetAdmin = BuildUser("AdminTwo", "admin2@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);

        context.Korisnici.AddRange(currentAdmin, targetAdmin);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var result = await service.DeactivateUserAsync(currentAdmin.ID, targetAdmin.ID);

        Assert.False(result.Success);
        Assert.Equal("Prvo uklonite administratorsku ulogu prije deaktivacije korisnika.", result.Message);
    }

    [Fact]
    public async Task ActivateUserAsync_WithInactiveUser_ReactivatesAccount()
    {
        using var context = GetInMemoryDbContext();
        var admin = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        var targetUser = BuildUser(
            "InactiveManagedUser",
            "inactive.managed@test.com",
            BCrypt.Net.BCrypt.HashPassword("ManagedPassword123!"),
            isActive: false);

        context.Korisnici.AddRange(admin, targetUser);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var result = await service.ActivateUserAsync(admin.ID, targetUser.ID);

        Assert.True(result.Success);

        var updatedUser = await context.Korisnici.FindAsync(targetUser.ID);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser.IsActive);
        Assert.Null(updatedUser.DeactivatedAt);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithPasswordOverMaxLength_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();
        const string currentPassword = "CurrentPassword123!";
        var user = BuildUser("PasswordUser", "password.user@test.com", BCrypt.Net.BCrypt.HashPassword(currentPassword));
        context.Korisnici.Add(user);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var tooLongPassword = new string('A', 65);

        var result = await service.ChangePasswordAsync(user.ID, new ChangePasswordRequestDto
        {
            CurrentPassword = currentPassword,
            NewPassword = tooLongPassword,
            ConfirmPassword = tooLongPassword
        });

        Assert.False(result.Success);
        Assert.Equal("Lozinka moze imati najvise 64 karaktera.", result.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidPassword_UpdatesStoredHash()
    {
        using var context = GetInMemoryDbContext();
        const string currentPassword = "CurrentPassword123!";
        const string newPassword = "NewPassword123!";
        var user = BuildUser("PasswordUpdateUser", "password.update@test.com", BCrypt.Net.BCrypt.HashPassword(currentPassword));
        context.Korisnici.Add(user);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.ChangePasswordAsync(user.ID, new ChangePasswordRequestDto
        {
            CurrentPassword = currentPassword,
            NewPassword = newPassword,
            ConfirmPassword = newPassword
        });

        Assert.True(result.Success);

        var updatedUser = await context.Korisnici.FindAsync(user.ID);
        Assert.NotNull(updatedUser);
        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task CreateUserAsync_WithInvalidPassword_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();

        var request = _fixture.Build<RegisterRequestDto>()
            .With(r => r.ImePrezime, "Validno Ime")
            .With(r => r.Username, "ValidUser123")
            .With(r => r.Email, "valid.user123@test.com")
            .With(r => r.Password, "weak12")
            .Create();

        var service = CreateService(context);

        var result = await service.CreateUserAsync(request, UlogaKorisnika.Student);

        Assert.False(result.Success);
        Assert.Contains("Lozinka mora imati najmanje 8 znakova", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_WithPasswordOverMaxLength_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();

        var request = new RegisterRequestDto
        {
            ImePrezime = "Validno Ime",
            Email = "valid.user@test.com",
            Username = "ValidUser123",
            Password = new string('A', 65)
        };

        var service = CreateService(context);
        var result = await service.CreateUserAsync(request, UlogaKorisnika.Student);

        Assert.False(result.Success);
        Assert.Equal("Lozinka moze imati najvise 64 karaktera.", result.Message);
    }

    [Fact]
    public async Task UpdateUserAsync_WithEmailOverMaxLength_ReturnsFalse()
    {
        using var context = GetInMemoryDbContext();
        var admin = BuildUser("AdminUser", "admin@test.com", BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"), UlogaKorisnika.Admin);
        var targetUser = BuildUser("ManagedUser", "managed@test.com", BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!"));

        context.Korisnici.AddRange(admin, targetUser);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var tooLongEmail = $"{new string('a', 246)}@test.com";

        var result = await service.UpdateUserAsync(admin.ID, targetUser.ID, new UpdateManagedUserRequestDto
        {
            ImePrezime = "Managed User",
            Email = tooLongEmail,
            Username = "ManagedUser",
            Uloga = UlogaKorisnika.Student,
            NewPassword = string.Empty
        });

        Assert.False(result.Success);
        Assert.Equal("Email moze imati najvise 254 karaktera.", result.Message);
    }
}
