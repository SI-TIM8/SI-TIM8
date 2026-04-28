using System.Net;
using System.Net.Http.Json;
using LABsistem.Bll.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Ovdje se po potrebi u postavkama factoryja može mockati baza ili koristiti in-memory baza
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginEndpoint_WithEmptyBody_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "", Password = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);

        // Assert
        // Očekujemo Unauthorized (401) jer AuthService vraća null za prazna polja, 
        // a AuthController na null vraća Unauthorized.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VerifyTokenEndpoint_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new VerifyTokenRequestDto { Token = "neispravan.jwt.token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/verify-token", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}