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

    private async Task SeedUserAsync(string username, string email, string password)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

        if (await context.Korisnici.AnyAsync(x => x.Username == username))
        {
            return;
        }

        context.Korisnici.Add(new Korisnik
        {
            ImePrezime = "Test User",
            Email = email,
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Uloga = UlogaKorisnika.Student
        });

        await context.SaveChangesAsync();
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
    public async Task VerifyTokenEndpoint_WithInvalidToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/verify-token", new VerifyTokenRequestDto
        {
            Token = "neispravan.jwt.token"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
