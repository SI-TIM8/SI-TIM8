using System.Net.Http.Headers;
using System.Net.Http.Json;
using LABsistem.Application.DTOs;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LabSistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LABsistem.Tests.Integration
{
    public class RezervacijaIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public RezervacijaIntegrationTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<int> SeedUserAsync(
            string username,
            string email,
            string password,
            UlogaKorisnika role)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

            var existingUser = await context.Korisnici.FirstOrDefaultAsync(x => x.Username == username);
            if (existingUser is not null)
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(password);
                existingUser.Email = email;
                existingUser.Uloga = role;
                existingUser.DeactivatedAt = null;
                await context.SaveChangesAsync();
                return existingUser.ID;
            }

            var user = new Korisnik
            {
                ImePrezime = $"{role} Test User",
                Email = email,
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Uloga = role
            };

            context.Korisnici.Add(user);
            await context.SaveChangesAsync();
            return user.ID;
        }

        private async Task<string> LoginAsync(string username, string password)
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
            {
                Username = username,
                Password = password
            });

            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            return payload!.Token;
        }

        [Fact]
        public async Task DostupniStudentima_ReturnsOnlyReservedVisibleTerms()
        {
            var tehnicarId = await SeedUserAsync("rez-tech", "rez.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("rez-prof", "rez.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("rez-student", "rez.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Rez objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Rez kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                context.Termini.AddRange(
                    new Termin
                    {
                        Datum = DateTime.Today.AddDays(2),
                        VrijemePocetka = new TimeSpan(9, 0, 0),
                        VrijemeKraja = new TimeSpan(10, 0, 0),
                        KreatorID = tehnicarId,
                        KabinetID = kabinet.ID,
                        ProfesorID = profesorId,
                        StatusTermina = StatusTermina.Rezervisan,
                        VidljivoStudentima = true,
                        LimitOsoba = 10
                    },
                    new Termin
                    {
                        Datum = DateTime.Today.AddDays(2),
                        VrijemePocetka = new TimeSpan(11, 0, 0),
                        VrijemeKraja = new TimeSpan(12, 0, 0),
                        KreatorID = tehnicarId,
                        KabinetID = kabinet.ID,
                        ProfesorID = profesorId,
                        StatusTermina = StatusTermina.Rezervisan,
                        VidljivoStudentima = false,
                        LimitOsoba = 10
                    },
                    new Termin
                    {
                        Datum = DateTime.Today.AddDays(2),
                        VrijemePocetka = new TimeSpan(13, 0, 0),
                        VrijemeKraja = new TimeSpan(14, 0, 0),
                        KreatorID = tehnicarId,
                        KabinetID = kabinet.ID,
                        StatusTermina = StatusTermina.Slobodan,
                        VidljivoStudentima = true
                    });
                await context.SaveChangesAsync();

                var vidljivTerminId = await context.Termini
                    .Where(t => t.StatusTermina == StatusTermina.Rezervisan && t.VidljivoStudentima)
                    .Select(t => t.ID)
                    .SingleAsync();

                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = vidljivTerminId,
                    StatusZahtjeva = StatusZahtjeva.NaCekanju,
                    Komentar = string.Empty
                });
                await context.SaveChangesAsync();
            }

            var studentToken = await LoginAsync("rez-student", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Rezervacija/dostupni-studentima");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<List<TerminDTO>>();

            Assert.NotNull(payload);
            Assert.Single(payload);
            Assert.Equal("Rezervisan", payload[0].StatusTermina);
            Assert.True(payload[0].VidljivoStudentima);
            Assert.Equal("NaCekanju", payload[0].StatusPrijave);
        }
    }
}
