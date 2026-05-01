using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<LabSistemDbContext>>();
            services.RemoveAll<LabSistemDbContext>();

            services.AddDbContext<LabSistemDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}

public class AuthIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
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
        bool isActive = true)
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
            await context.SaveChangesAsync();
            return existingUser.ID;
        }

        var user = new Korisnik
        {
            ImePrezime = "Test User",
            Email = email,
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Uloga = role,
            DeactivatedAt = isActive ? null : DateTime.UtcNow
        };

        context.Korisnici.Add(user);
        await context.SaveChangesAsync();
        return user.ID;
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
        Assert.Contains("Profil je uspjesno azuriran.", payload);
        Assert.Contains("profileupdated", payload);
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
}
