using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using LABsistem.Api.Services;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FrontendBaseUrl"] = "http://localhost:3001",
                ["ReservationReminders:Enabled"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<LabSistemDbContext>>();
            services.RemoveAll<LabSistemDbContext>();
            services.RemoveAll<IEmailNotificationService>();
            services.RemoveAll<TestEmailNotificationService>();

            services.AddDbContext<LabSistemDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddSingleton<TestEmailNotificationService>();
            services.AddSingleton<IEmailNotificationService>(sp =>
                sp.GetRequiredService<TestEmailNotificationService>());
        });
    }
}

public class TestEmailNotificationService : IEmailNotificationService
{
    private readonly object _syncRoot = new();
    private readonly List<PasswordResetEmailRecord> _passwordResetEmails = [];
    private readonly List<ReservationDecisionEmailRecord> _reservationDecisionEmails = [];
    private readonly List<EquipmentFaultEmailRecord> _equipmentFaultEmails = [];
    private readonly List<EmailVerificationEmailRecord> _emailVerificationEmails = [];
    private readonly List<ReservationReminderEmailRecord> _reservationReminderEmails = [];

    public IReadOnlyList<PasswordResetEmailRecord> PasswordResetEmails
    {
        get
        {
            lock (_syncRoot)
            {
                return _passwordResetEmails.ToList();
            }
        }
    }

    public IReadOnlyList<ReservationDecisionEmailRecord> ReservationDecisionEmails
    {
        get
        {
            lock (_syncRoot)
            {
                return _reservationDecisionEmails.ToList();
            }
        }
    }

    public IReadOnlyList<EquipmentFaultEmailRecord> EquipmentFaultEmails
    {
        get
        {
            lock (_syncRoot)
            {
                return _equipmentFaultEmails.ToList();
            }
        }
    }

    public IReadOnlyList<EmailVerificationEmailRecord> EmailVerificationEmails
    {
        get
        {
            lock (_syncRoot)
            {
                return _emailVerificationEmails.ToList();
            }
        }
    }

    public IReadOnlyList<ReservationReminderEmailRecord> ReservationReminderEmails
    {
        get
        {
            lock (_syncRoot)
            {
                return _reservationReminderEmails.ToList();
            }
        }
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            _passwordResetEmails.Clear();
            _reservationDecisionEmails.Clear();
            _equipmentFaultEmails.Clear();
            _emailVerificationEmails.Clear();
            _reservationReminderEmails.Clear();
        }
    }

    public Task<bool> SendReservationDecisionEmailAsync(
        string recipientEmail,
        string recipientName,
        DateTime datumTermina,
        TimeSpan vrijemePocetka,
        bool odobri,
        string? komentar = null,
        CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _reservationDecisionEmails.Add(new ReservationDecisionEmailRecord(
                recipientEmail,
                recipientName,
                datumTermina,
                vrijemePocetka,
                odobri,
                komentar));
        }

        return Task.FromResult(true);
    }

    public Task<bool> SendPasswordResetEmailAsync(
        string recipientEmail,
        string recipientName,
        string resetLink,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _passwordResetEmails.Add(new PasswordResetEmailRecord(
                recipientEmail,
                recipientName,
                resetLink,
                expiresAtUtc));
        }

        return Task.FromResult(true);
    }

    public Task<bool> SendEmailVerificationEmailAsync(
        string recipientEmail,
        string recipientName,
        string verificationLink,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _emailVerificationEmails.Add(new EmailVerificationEmailRecord(
                recipientEmail,
                recipientName,
                verificationLink,
                expiresAtUtc));
        }

        return Task.FromResult(true);
    }

    public Task<bool> SendEquipmentFaultEmailAsync(
        string recipientEmail,
        string recipientName,
        string opremaNaziv,
        DateTime datumTermina,
        TimeSpan vrijemePocetka,
        TimeSpan vrijemeKraja,
        string komentar,
        string? appLinkText = null,
        CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _equipmentFaultEmails.Add(new EquipmentFaultEmailRecord(
                recipientEmail,
                recipientName,
                opremaNaziv,
                datumTermina,
                vrijemePocetka,
                vrijemeKraja,
                komentar,
                appLinkText));
        }

        return Task.FromResult(true);
    }

    public Task<bool> SendReservationReminderEmailAsync(
        string recipientEmail,
        string recipientName,
        DateTime datumTermina,
        TimeSpan vrijemePocetka,
        TimeSpan vrijemeKraja,
        string kabinetNaziv,
        string reminderLeadTimeText,
        CancellationToken cancellationToken = default)
    {
        lock (_syncRoot)
        {
            _reservationReminderEmails.Add(new ReservationReminderEmailRecord(
                recipientEmail,
                recipientName,
                datumTermina,
                vrijemePocetka,
                vrijemeKraja,
                kabinetNaziv,
                reminderLeadTimeText));
        }

        return Task.FromResult(true);
    }
}

public record PasswordResetEmailRecord(
    string RecipientEmail,
    string RecipientName,
    string ResetLink,
    DateTime ExpiresAtUtc);

public record ReservationDecisionEmailRecord(
    string RecipientEmail,
    string RecipientName,
    DateTime DatumTermina,
    TimeSpan VrijemePocetka,
    bool Odobri,
    string? Komentar);

public record EquipmentFaultEmailRecord(
    string RecipientEmail,
    string RecipientName,
    string OpremaNaziv,
    DateTime DatumTermina,
    TimeSpan VrijemePocetka,
    TimeSpan VrijemeKraja,
    string Komentar,
    string? AppLinkText);

public record EmailVerificationEmailRecord(
    string RecipientEmail,
    string RecipientName,
    string VerificationLink,
    DateTime ExpiresAtUtc);

public record ReservationReminderEmailRecord(
    string RecipientEmail,
    string RecipientName,
    DateTime DatumTermina,
    TimeSpan VrijemePocetka,
    TimeSpan VrijemeKraja,
    string KabinetNaziv,
    string ReminderLeadTimeText);

public record MessageResponseDto(string Message);
public record VerifyResetTokenResponseDto(bool Valid, string Message);

public class AuthIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private const string GenericForgotPasswordMessage =
        "Ako nalog sa ovim emailom postoji, poslali smo instrukcije za resetovanje lozinke.";

    private const string InvalidResetTokenMessage =
        "Link za resetovanje lozinke nije validan ili je istekao. Zatražite novi link.";

    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<int> SeedUserAsync(
        string username,
        string email,
        string password,
        UlogaKorisnika role = UlogaKorisnika.Student,
        bool isActive = true,
        bool emailVerified = true,
        bool mustChangePassword = false)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

        var existingUser = await context.Korisnici.FirstOrDefaultAsync(x => x.Username == username);
        if (existingUser is not null)
        {
            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(password);
            existingUser.Email = email;
            existingUser.Uloga = role;
            existingUser.DeactivatedAt = isActive ? null : DateTime.UtcNow;
            existingUser.EmailVerified = emailVerified;
            existingUser.EmailVerifiedAtUtc = emailVerified ? DateTime.UtcNow : null;
            existingUser.MustChangePassword = mustChangePassword;
            await context.SaveChangesAsync();
            return existingUser.ID;
        }

        var user = new Korisnik
        {
            ImePrezime = "Test User",
            Email = email,
            EmailVerified = emailVerified,
            EmailVerifiedAtUtc = emailVerified ? DateTime.UtcNow : null,
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Uloga = role,
            MustChangePassword = mustChangePassword,
            DeactivatedAt = isActive ? null : DateTime.UtcNow
        };

        context.Korisnici.Add(user);
        await context.SaveChangesAsync();
        return user.ID;
    }

    private TestEmailNotificationService GetEmailService()
    {
        using var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TestEmailNotificationService>();
    }

    private static string ExtractTokenFromLink(string resetLink)
    {
        var uri = new Uri(resetLink);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        return query["token"].ToString();
    }

    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken.Trim()));
        return Convert.ToHexString(bytes);
    }

    [Fact]
    public async Task LoginEndpoint_WithValidCredentials_ReturnsAccessAndRefreshTokens()
    {
        await SeedUserAsync("integrationuser", "integration@test.com", "ValidPassword123!");

        var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "integrationuser",
            Password = "ValidPassword123!"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.False(string.IsNullOrWhiteSpace(payload.RefreshToken));
        Assert.True(payload.AccessTokenExpiresAtUtc > DateTime.UtcNow);
        Assert.True(payload.RefreshTokenExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginEndpoint_WithInactiveUser_ReturnsUnauthorized()
    {
        await SeedUserAsync(
            "inactiveintegration",
            "inactive.integration@test.com",
            "ValidPassword123!",
            isActive: false);

        var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "inactiveintegration",
            Password = "ValidPassword123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Pristup ovom nalogu je blokiran.", payload);
    }

    [Fact]
    public async Task LoginEndpoint_WithInvalidCredentials_ReturnsUnauthorizedWithGenericMessage()
    {
        await SeedUserAsync("invalidintegration", "invalid.integration@test.com", "ValidPassword123!");

        var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "invalidintegration",
            Password = "PogresnaLozinka123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Pogresni kredencijali.", payload);
    }

    [Fact]
    public async Task RefreshEndpoint_WithValidRefreshToken_ReturnsNewSession()
    {
        await SeedUserAsync("refreshintegration", "refresh.integration@test.com", "ValidPassword123!");

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "refreshintegration",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        var refreshResponse = await _client.PostAsJsonAsync("/api/Auth/refresh", new RefreshTokenRequestDto
        {
            RefreshToken = loginPayload!.RefreshToken
        });

        refreshResponse.EnsureSuccessStatusCode();
        var refreshPayload = await refreshResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(refreshPayload);
        Assert.NotEqual(loginPayload.Token, refreshPayload.Token);
        Assert.NotEqual(loginPayload.RefreshToken, refreshPayload.RefreshToken);
    }

    [Fact]
    public async Task LogoutEndpoint_WithRefreshToken_PreventsFurtherRefresh()
    {
        await SeedUserAsync("logoutintegration", "logout.integration@test.com", "ValidPassword123!");

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "logoutintegration",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var logoutRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/logout")
        {
            Content = JsonContent.Create(new LogoutRequestDto
            {
                RefreshToken = loginPayload!.RefreshToken
            })
        };
        logoutRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Token);

        var logoutResponse = await _client.SendAsync(logoutRequest);
        logoutResponse.EnsureSuccessStatusCode();

        var refreshResponse = await _client.PostAsJsonAsync("/api/Auth/refresh", new RefreshTokenRequestDto
        {
            RefreshToken = loginPayload.RefreshToken
        });

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task VerifyEndpoint_WithDeactivatedUserAccessToken_ReturnsUnauthorized()
    {
        var userId = await SeedUserAsync("verifyinactive", "verify.inactive@test.com", "ValidPassword123!");

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "verifyinactive",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var user = await context.Korisnici.SingleAsync(x => x.ID == userId);
            user.DeactivatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        using var verifyRequest = new HttpRequestMessage(HttpMethod.Get, "/api/Auth/verify");
        verifyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Token);

        var verifyResponse = await _client.SendAsync(verifyRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, verifyResponse.StatusCode);
    }

    [Fact]
    public async Task DeactivateEndpoint_RevokesRefreshTokenAndBlocksRefresh()
    {
        await SeedUserAsync("adminintegration", "admin.integration@test.com", "AdminPassword123!", UlogaKorisnika.Admin);
        var targetUserId = await SeedUserAsync("targetintegration", "target.integration@test.com", "ValidPassword123!");

        var adminLogin = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "adminintegration",
            Password = "AdminPassword123!"
        });
        adminLogin.EnsureSuccessStatusCode();
        var adminPayload = await adminLogin.Content.ReadFromJsonAsync<LoginResponseDto>();

        var targetLogin = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "targetintegration",
            Password = "ValidPassword123!"
        });
        targetLogin.EnsureSuccessStatusCode();
        var targetPayload = await targetLogin.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var deactivateRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/Auth/users/{targetUserId}/deactivate");
        deactivateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminPayload!.Token);

        var deactivateResponse = await _client.SendAsync(deactivateRequest);
        deactivateResponse.EnsureSuccessStatusCode();

        var refreshResponse = await _client.PostAsJsonAsync("/api/Auth/refresh", new RefreshTokenRequestDto
        {
            RefreshToken = targetPayload!.RefreshToken
        });

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateProfileEndpoint_WithValidPayload_UpdatesProfileData()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync("profileintegration", "profile.integration@test.com", "ValidPassword123!");

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "profileintegration",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var updateRequest = new HttpRequestMessage(HttpMethod.Put, "/api/Auth/profile")
        {
            Content = JsonContent.Create(new UpdateProfileRequestDto
            {
                ImePrezime = "Profile Integration Updated",
                Email = "profile.updated@test.com",
                Username = "profileupdated"
            })
        };
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Token);

        var updateResponse = await _client.SendAsync(updateRequest);

        updateResponse.EnsureSuccessStatusCode();
        var payload = await updateResponse.Content.ReadAsStringAsync();
        Assert.Contains("Profil je uspješno ažuriran.", payload);
        Assert.Contains("profileupdated", payload);

        var verificationEmail = Assert.Single(emailService.EmailVerificationEmails);
        Assert.Equal("profile.updated@test.com", verificationEmail.RecipientEmail);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var updatedUser = await context.Korisnici.SingleAsync(x => x.Username == "profileupdated");
        Assert.Equal("profile.updated@test.com", updatedUser.Email);
        Assert.False(updatedUser.EmailVerified);
        Assert.Null(updatedUser.EmailVerifiedAtUtc);
    }

    [Fact]
    public async Task CreateUserEndpoint_WithAdmin_SendsVerificationEmail_AndMarksUserAsUnverified()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync("admincreateverify", "admin.create.verify@test.com", "AdminPassword123!", UlogaKorisnika.Admin);

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "admincreateverify",
            Password = "AdminPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/create-user?uloga=Student")
        {
            Content = JsonContent.Create(new RegisterRequestDto
            {
                ImePrezime = "Email Verify Student",
                Email = "email.verify.student@test.com",
                Username = "emailverifystudent",
                Password = "ValidPassword123!"
            })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Token);

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        var verificationEmail = Assert.Single(emailService.EmailVerificationEmails);
        Assert.Equal("email.verify.student@test.com", verificationEmail.RecipientEmail);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var createdUser = await context.Korisnici.SingleAsync(x => x.Username == "emailverifystudent");
        Assert.False(createdUser.EmailVerified);
        Assert.Null(createdUser.EmailVerifiedAtUtc);
        Assert.True(createdUser.MustChangePassword);

        var storedToken = await context.EmailVerificationTokens.SingleAsync(x => x.KorisnikID == createdUser.ID);
        Assert.Equal(createdUser.Email, storedToken.Email);
        Assert.Null(storedToken.UsedAtUtc);
    }

    [Theory]
    [InlineData(UlogaKorisnika.Student, "firstloginstudent", "first.login.student@test.com")]
    [InlineData(UlogaKorisnika.Profesor, "firstloginprofesor", "first.login.profesor@test.com")]
    [InlineData(UlogaKorisnika.Tehnicar, "firstlogintehnicar", "first.login.tehnicar@test.com")]
    [InlineData(UlogaKorisnika.Admin, "firstloginadmin", "first.login.admin@test.com")]
    public async Task LoginEndpoint_WithAdminCreatedUserOfAnyRole_ReturnsMustChangePasswordFlag(
        UlogaKorisnika role,
        string username,
        string email)
    {
        await SeedUserAsync("admincreateverify2", "admin.create.verify2@test.com", "AdminPassword123!", UlogaKorisnika.Admin);

        var adminLogin = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "admincreateverify2",
            Password = "AdminPassword123!"
        });
        adminLogin.EnsureSuccessStatusCode();
        var adminPayload = await adminLogin.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/Auth/create-user?uloga={role}")
        {
            Content = JsonContent.Create(new RegisterRequestDto
            {
                ImePrezime = "First Login User",
                Email = email,
                Username = username,
                Password = "ValidPassword123!"
            })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminPayload!.Token);

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        var userLogin = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = username,
            Password = "ValidPassword123!"
        });
        userLogin.EnsureSuccessStatusCode();

        var userPayload = await userLogin.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(userPayload);
        Assert.True(userPayload!.MustChangePassword);
    }

    [Fact]
    public async Task MustChangePasswordUser_CannotAccessProfileUntilPasswordIsChanged()
    {
        await SeedUserAsync(
            "firstloginblock",
            "first.login.block@test.com",
            "ValidPassword123!",
            mustChangePassword: true);

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "firstloginblock",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginPayload);
        Assert.True(loginPayload!.MustChangePassword);

        using var profileRequest = new HttpRequestMessage(HttpMethod.Get, "/api/Auth/profile");
        profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Token);

        var blockedProfileResponse = await _client.SendAsync(profileRequest);
        Assert.Equal(HttpStatusCode.Forbidden, blockedProfileResponse.StatusCode);

        using var changePasswordRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/change-password")
        {
            Content = JsonContent.Create(new ChangePasswordRequestDto
            {
                NewPassword = "NovaValidna123!",
                ConfirmPassword = "NovaValidna123!"
            })
        };
        changePasswordRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Token);

        var changePasswordResponse = await _client.SendAsync(changePasswordRequest);
        changePasswordResponse.EnsureSuccessStatusCode();

        var profileAfterChangeRequest = new HttpRequestMessage(HttpMethod.Get, "/api/Auth/profile");
        profileAfterChangeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Token);

        var profileAfterChangeResponse = await _client.SendAsync(profileAfterChangeRequest);
        profileAfterChangeResponse.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var updatedUser = await context.Korisnici.SingleAsync(x => x.Username == "firstloginblock");
        Assert.False(updatedUser.MustChangePassword);
    }

    [Fact]
    public async Task VerifyEmailEndpoint_WithValidToken_MarksEmailAsVerified()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync("adminverifyemail", "admin.verify.email@test.com", "AdminPassword123!", UlogaKorisnika.Admin);

        var adminLogin = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "adminverifyemail",
            Password = "AdminPassword123!"
        });
        adminLogin.EnsureSuccessStatusCode();
        var adminPayload = await adminLogin.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/create-user?uloga=Student")
        {
            Content = JsonContent.Create(new RegisterRequestDto
            {
                ImePrezime = "Verify Flow Student",
                Email = "verify.flow.student@test.com",
                Username = "verifyflowstudent",
                Password = "ValidPassword123!"
            })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminPayload!.Token);

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.EnsureSuccessStatusCode();

        var verificationEmail = Assert.Single(emailService.EmailVerificationEmails);
        var token = ExtractTokenFromLink(verificationEmail.VerificationLink);

        var verifyResponse = await _client.GetAsync($"/api/Auth/verify-email?token={Uri.EscapeDataString(token)}");
        verifyResponse.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var verifiedUser = await context.Korisnici.SingleAsync(x => x.Username == "verifyflowstudent");
        Assert.True(verifiedUser.EmailVerified);
        Assert.NotNull(verifiedUser.EmailVerifiedAtUtc);

        var storedToken = await context.EmailVerificationTokens.SingleAsync(x => x.KorisnikID == verifiedUser.ID);
        Assert.NotNull(storedToken.UsedAtUtc);
    }

    [Fact]
    public async Task ResendVerificationEmailEndpoint_WithUnverifiedEmail_SendsVerificationEmail()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync(
            "resendverify",
            "resend.verify@test.com",
            "ValidPassword123!",
            emailVerified: false);

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "resendverify",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var resendRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/resend-verification-email");
        resendRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Token);

        var resendResponse = await _client.SendAsync(resendRequest);
        resendResponse.EnsureSuccessStatusCode();

        var verificationEmail = Assert.Single(emailService.EmailVerificationEmails);
        Assert.Equal("resend.verify@test.com", verificationEmail.RecipientEmail);
    }

    [Fact]
    public async Task ForgotPasswordEndpoint_WithUnverifiedEmail_ReturnsGenericSuccessWithoutSendingEmail()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync(
            "forgotunverified",
            "forgot.unverified@test.com",
            "ValidPassword123!",
            emailVerified: false);

        int tokenCountBeforeRequest;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            tokenCountBeforeRequest = await context.PasswordResetTokens.CountAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/Auth/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "forgot.unverified@test.com"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();

        Assert.NotNull(payload);
        Assert.Equal(GenericForgotPasswordMessage, payload!.Message);
        Assert.Empty(emailService.PasswordResetEmails);

        using var verificationScope = _factory.Services.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var tokenCountAfterRequest = await verificationContext.PasswordResetTokens.CountAsync();
        Assert.Equal(tokenCountBeforeRequest, tokenCountAfterRequest);
    }

    [Fact]
    public async Task ChangePasswordEndpoint_WithValidPayload_AllowsLoginWithNewPassword()
    {
        await SeedUserAsync("changepasswordintegration", "change.password@test.com", "ValidPassword123!");

        var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "changepasswordintegration",
            Password = "ValidPassword123!"
        });
        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        using var changePasswordRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/change-password")
        {
            Content = JsonContent.Create(new ChangePasswordRequestDto
            {
                CurrentPassword = "ValidPassword123!",
                NewPassword = "NovaValidna123!",
                ConfirmPassword = "NovaValidna123!"
            })
        };
        changePasswordRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Token);

        var changePasswordResponse = await _client.SendAsync(changePasswordRequest);
        changePasswordResponse.EnsureSuccessStatusCode();

        var oldLoginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "changepasswordintegration",
            Password = "ValidPassword123!"
        });

        var newLoginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "changepasswordintegration",
            Password = "NovaValidna123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginResponse.StatusCode);
        newLoginResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task VerifyTokenEndpoint_WithInvalidToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/verify-token", new VerifyTokenRequestDto
        {
            Token = "neispravan.jwt.token"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPasswordEndpoint_WithExistingEmail_ReturnsGenericSuccess_StoresHashedToken_AndSendsEmail()
    {
        var emailService = GetEmailService();
        emailService.Clear();

        var userId = await SeedUserAsync("forgotexisting", "forgot.existing@test.com", "ValidPassword123!");

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            context.PasswordResetTokens.Add(new PasswordResetToken
            {
                KorisnikID = userId,
                TokenHash = "OLDTOKENHASH",
                CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(20)
            });
            await context.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/Auth/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "forgot.existing@test.com"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal(GenericForgotPasswordMessage, payload!.Message);

        var sentEmail = Assert.Single(emailService.PasswordResetEmails);
        Assert.Equal("forgot.existing@test.com", sentEmail.RecipientEmail);
        Assert.Contains("/reset-password?token=", sentEmail.ResetLink);
        var rawToken = ExtractTokenFromLink(sentEmail.ResetLink);

        using var verificationScope = _factory.Services.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var storedTokens = await verificationContext.PasswordResetTokens
            .Where(x => x.KorisnikID == userId)
            .OrderBy(x => x.PasswordResetTokenID)
            .ToListAsync();

        Assert.Equal(2, storedTokens.Count);
        Assert.NotNull(storedTokens[0].UsedAtUtc);
        Assert.Equal(HashToken(rawToken), storedTokens[1].TokenHash);
        Assert.NotEqual(rawToken, storedTokens[1].TokenHash);
        Assert.Null(storedTokens[1].UsedAtUtc);
        Assert.True(storedTokens[1].ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task ForgotPasswordEndpoint_WithUnknownEmail_ReturnsSameGenericSuccessWithoutSendingEmail()
    {
        var emailService = GetEmailService();
        emailService.Clear();

        int tokenCountBeforeRequest;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            tokenCountBeforeRequest = await context.PasswordResetTokens.CountAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/Auth/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "nepostojeci@test.com"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal(GenericForgotPasswordMessage, payload!.Message);
        Assert.Empty(emailService.PasswordResetEmails);

        using var verificationScope = _factory.Services.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var tokenCountAfterRequest = await verificationContext.PasswordResetTokens.CountAsync();
        Assert.Equal(tokenCountBeforeRequest, tokenCountAfterRequest);
    }

    [Fact]
    public async Task ForgotPasswordEndpoint_WithInvalidEmail_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "neispravan-email"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Email nije ispravan.", payload);
    }

    [Fact]
    public async Task VerifyResetTokenEndpoint_WithValidToken_ReturnsValidTrue()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync("verifyreset", "verify.reset@test.com", "ValidPassword123!");

        await _client.PostAsJsonAsync("/api/Auth/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "verify.reset@test.com"
        });

        var email = Assert.Single(emailService.PasswordResetEmails);
        var token = ExtractTokenFromLink(email.ResetLink);

        var response = await _client.GetAsync($"/api/Auth/verify-reset-token?token={Uri.EscapeDataString(token)}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<VerifyResetTokenResponseDto>();
        Assert.NotNull(payload);
        Assert.True(payload.Valid);
        Assert.Equal(string.Empty, payload.Message);
    }

    [Fact]
    public async Task ResetPasswordEndpoint_WithValidToken_UpdatesPassword_AndMarksTokenUsed()
    {
        var emailService = GetEmailService();
        emailService.Clear();
        await SeedUserAsync("resetvalid", "reset.valid@test.com", "StaraLozinka123!");

        await _client.PostAsJsonAsync("/api/Auth/forgot-password", new ForgotPasswordRequestDto
        {
            Email = "reset.valid@test.com"
        });

        var email = Assert.Single(emailService.PasswordResetEmails);
        var token = ExtractTokenFromLink(email.ResetLink);

        var response = await _client.PostAsJsonAsync("/api/Auth/reset-password", new ResetPasswordRequestDto
        {
            Token = token,
            NewPassword = "NovaLozinka123!",
            ConfirmPassword = "NovaLozinka123!"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal("Lozinka je uspješno promijenjena. Sada se možete prijaviti.", payload!.Message);

        var oldLoginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "resetvalid",
            Password = "StaraLozinka123!"
        });

        var newLoginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "resetvalid",
            Password = "NovaLozinka123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginResponse.StatusCode);
        newLoginResponse.EnsureSuccessStatusCode();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var storedToken = await context.PasswordResetTokens.SingleAsync(x => x.TokenHash == HashToken(token));
        Assert.NotNull(storedToken.UsedAtUtc);
    }

    [Fact]
    public async Task ResetPasswordEndpoint_WithExpiredToken_ReturnsBadRequestAndDoesNotChangePassword()
    {
        var userId = await SeedUserAsync("resetexpired", "reset.expired@test.com", "StaraLozinka123!");
        var rawToken = "expired-reset-token";

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            context.PasswordResetTokens.Add(new PasswordResetToken
            {
                KorisnikID = userId,
                TokenHash = HashToken(rawToken),
                CreatedAtUtc = DateTime.UtcNow.AddHours(-2),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-10)
            });
            await context.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/Auth/reset-password", new ResetPasswordRequestDto
        {
            Token = rawToken,
            NewPassword = "NovaLozinka123!",
            ConfirmPassword = "NovaLozinka123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal(InvalidResetTokenMessage, payload!.Message);

        var oldLoginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = "resetexpired",
            Password = "StaraLozinka123!"
        });

        oldLoginResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ResetPasswordEndpoint_WithUsedToken_ReturnsBadRequest()
    {
        var userId = await SeedUserAsync("resetused", "reset.used@test.com", "StaraLozinka123!");
        var rawToken = "used-reset-token";

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            context.PasswordResetTokens.Add(new PasswordResetToken
            {
                KorisnikID = userId,
                TokenHash = HashToken(rawToken),
                CreatedAtUtc = DateTime.UtcNow.AddMinutes(-15),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15),
                UsedAtUtc = DateTime.UtcNow.AddMinutes(-5)
            });
            await context.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/Auth/reset-password", new ResetPasswordRequestDto
        {
            Token = rawToken,
            NewPassword = "NovaLozinka123!",
            ConfirmPassword = "NovaLozinka123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal(InvalidResetTokenMessage, payload!.Message);
    }

    [Fact]
    public async Task ResetPasswordEndpoint_WithInvalidToken_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/reset-password", new ResetPasswordRequestDto
        {
            Token = "nepostojeci-token",
            NewPassword = "NovaLozinka123!",
            ConfirmPassword = "NovaLozinka123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal(InvalidResetTokenMessage, payload!.Message);
    }

    [Fact]
    public async Task ResetPasswordEndpoint_WithMismatchedPasswords_ReturnsBadRequestAndLeavesTokenUnused()
    {
        var userId = await SeedUserAsync("resetmismatch", "reset.mismatch@test.com", "StaraLozinka123!");
        var rawToken = "mismatch-reset-token";

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            context.PasswordResetTokens.Add(new PasswordResetToken
            {
                KorisnikID = userId,
                TokenHash = HashToken(rawToken),
                CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(25)
            });
            await context.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/Auth/reset-password", new ResetPasswordRequestDto
        {
            Token = rawToken,
            NewPassword = "NovaLozinka123!",
            ConfirmPassword = "DrugaLozinka123!"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MessageResponseDto>();
        Assert.Equal("Nova lozinka i potvrda se ne poklapaju.", payload!.Message);

        using var verificationScope = _factory.Services.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var token = await verificationContext.PasswordResetTokens.SingleAsync(x => x.KorisnikID == userId);
        Assert.Null(token.UsedAtUtc);
    }
}
