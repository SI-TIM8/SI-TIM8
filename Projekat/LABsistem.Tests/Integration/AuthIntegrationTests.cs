using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LABsistem.Presentation.DTOs.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LABsistem.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    
                    services.AddDbContext<LabSistemDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IntegrationTestDatabase");
                    });

                    // Seed the database
                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
                    db.Database.EnsureCreated();

                    if (!db.Korisnici.Any())
                    {
                        db.Korisnici.Add(new Korisnik
                        {
                            ImePrezime = "Integration User",
                            Email = "integration@test.com",
                            Username = "integration_user",
                            Password = "integration_password",
                            Uloga = UlogaKorisnika.Student
                        });
                        db.SaveChanges();
                    }
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task PostLogin_WithValidCredentials_ReturnsSuccessStatusCodeAndToken()
        {
            // Arrange
            var loginData = new LoginRequestDto
            {
                Username = "integration_user",
                Password = "integration_password"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal("integration_user", result.Username);
        }

        [Fact]
        public async Task PostLogin_WithWrongPassword_ReturnsUnauthorizedStatusCode()
        {
            // Arrange
            var loginData = new LoginRequestDto
            {
                Username = "integration_user",
                Password = "wrong_password_123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}