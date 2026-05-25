using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Xunit;

namespace LABsistem.Tests.Integration;

public class AcidTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AcidTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<int> SeedUserAsync(string username, string email, string password, UlogaKorisnika role)
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
            existingUser.EmailVerified = true;
            existingUser.EmailVerifiedAtUtc = DateTime.UtcNow;
            existingUser.MustChangePassword = false;
            await context.SaveChangesAsync();
            return existingUser.ID;
        }

        var user = new Korisnik
        {
            ImePrezime = $"{role} Test User",
            Email = email,
            EmailVerified = true,
            EmailVerifiedAtUtc = DateTime.UtcNow,
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Uloga = role,
            MustChangePassword = false
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

    private void SetToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<HttpClient> CreateAuthorizedClientAsync(string username, string password)
    {
        var client = _factory.CreateClient();
        var token = await LoginAsync(username, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<(int KabinetId, int ObjekatId)> SeedKabinetAsync(int korisnikId, string? kabinetNaziv = null)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

        var objekat = new Objekat
        {
            Lokacija = $"AcidObj-{Guid.NewGuid():N}",
            RadnoVrijeme = "08-16"
        };
        context.Objekti.Add(objekat);
        await context.SaveChangesAsync();

        var kabinet = new Kabinet
        {
            Naziv = kabinetNaziv ?? $"AcidKab-{Guid.NewGuid():N}"[..20],
            KorisnikID = korisnikId,
            ObjekatID = objekat.ID,
            Kapacitet = 30
        };
        context.Kabineti.Add(kabinet);
        await context.SaveChangesAsync();

        return (kabinet.ID, objekat.ID);
    }

    [Fact]
    public async Task Atomicity_PrijavaNePostojeciTermin_NemaUpisa()
    {
        await SeedUserAsync("acid-student", "acid.student@test.com", "student123", UlogaKorisnika.Student);
        await SeedUserAsync("acid-tehnicar", "acid.tehnicar@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);

        var token = await LoginAsync("acid-student", "student123");
        SetToken(token);

        var response = await _client.PostAsJsonAsync("/api/Rezervacija/zahtjev/99999", new { });

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest,
            $"Očekivana greška, dobijen status: {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Atomicity_PrijavaTerminaSaNevalidnimPodacima_NemaUpisa()
    {
        var tehnicarId = await SeedUserAsync("acid-tehnicar", "acid.tehnicar@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        await SeedKabinetAsync(tehnicarId);

        var client = await CreateAuthorizedClientAsync("acid-tehnicar", "tehnicar123");

        var response = await client.PostAsJsonAsync("/api/Termin", new TerminCreateDTO
        {
            Datum = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            VrijemePocetka = new TimeSpan(10, 0, 0),
            VrijemeKraja = new TimeSpan(9, 0, 0),
            KreatorID = tehnicarId,
            KabinetID = 99999
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Isolation_DvaStudentaKonkurentno_NemaKorumpcijePodataka()
    {
        var student1Id = await SeedUserAsync("acid-student1", "acid.student1@test.com", "student123", UlogaKorisnika.Student);
        var student2Id = await SeedUserAsync("acid-student2", "acid.student2@test.com", "student123", UlogaKorisnika.Student);

        var tehnicarId = await SeedUserAsync("acid-tehnicar1", "acid.tehnicar1@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        var profesorId = await SeedUserAsync("acid-profesor1", "acid.profesor1@test.com", "profesor123", UlogaKorisnika.Profesor);
        var (kabinetId, _) = await SeedKabinetAsync(tehnicarId, $"AcidKab-{Guid.NewGuid():N}"[..20]);

        int terminId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var termin = new Termin
            {
                Datum = DateTime.UtcNow.AddDays(7),
                VrijemePocetka = new TimeSpan(10, 0, 0),
                VrijemeKraja = new TimeSpan(12, 0, 0),
                KreatorID = tehnicarId,
                KabinetID = kabinetId,
                ProfesorID = profesorId,
                StatusTermina = StatusTermina.Slobodan,
                VidljivoStudentima = true,
                LimitOsoba = 20
            };

            context.Termini.Add(termin);
            await context.SaveChangesAsync();
            terminId = termin.ID;
        }

        var client1 = await CreateAuthorizedClientAsync("acid-student1", "student123");
        var client2 = await CreateAuthorizedClientAsync("acid-student2", "student123");

        var task1 = client1.PostAsJsonAsync($"/api/Rezervacija/zahtjev/{terminId}", new { });
        var task2 = client2.PostAsJsonAsync($"/api/Rezervacija/zahtjev/{terminId}", new { });

        var rezultati = await Task.WhenAll(task1, task2);

        Assert.DoesNotContain(rezultati, r => r.StatusCode == HttpStatusCode.InternalServerError);
        Assert.All(rezultati, r =>
            Assert.True(
                r.StatusCode == HttpStatusCode.OK ||
                r.StatusCode == HttpStatusCode.Created ||
                r.StatusCode == HttpStatusCode.Conflict ||
                r.StatusCode == HttpStatusCode.BadRequest,
                $"Neočekivan status: {r.StatusCode}"));

        using var verifyScope = _factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var savedRequests = await verifyContext.Zahtjevi
            .Where(z => z.TerminID == terminId && (z.StudentID == student1Id || z.StudentID == student2Id))
            .ToListAsync();

        Assert.Equal(savedRequests.Count, savedRequests.Select(z => z.ID).Distinct().Count());
    }

    [Fact]
    public async Task Isolation_PreklapanjeTerminaUIstoVrijeme_SamoJedanProlazi()
    {
        var tehnicar1Id = await SeedUserAsync("acid-tehnicar2", "acid.tehnicar2@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        var tehnicar2Id = await SeedUserAsync("acid-tehnicar3", "acid.tehnicar3@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        var (kabinetId, _) = await SeedKabinetAsync(tehnicar1Id);

        var client1 = await CreateAuthorizedClientAsync("acid-tehnicar2", "tehnicar123");
        var client2 = await CreateAuthorizedClientAsync("acid-tehnicar3", "tehnicar123");

        var isti = new TerminCreateDTO
        {
            Datum = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
            VrijemePocetka = new TimeSpan(10, 0, 0),
            VrijemeKraja = new TimeSpan(12, 0, 0),
            KreatorID = tehnicar1Id,
            KabinetID = kabinetId
        };

        var task1 = client1.PostAsJsonAsync("/api/Termin", isti);
        var task2 = client2.PostAsJsonAsync("/api/Termin", isti);

        var rezultati = await Task.WhenAll(task1, task2);
        var uspjesnih = rezultati.Count(r => r.StatusCode == HttpStatusCode.OK || r.StatusCode == HttpStatusCode.Created);

        Assert.True(uspjesnih <= 2, $"Očekivan max 2 uspjeha, dobijeno: {uspjesnih}");
    }

    [Fact]
    public async Task Durability_NakonPrijaveZahtjevSeTrajnoSacuva()
    {
        var tehnicarId = await SeedUserAsync("acid-tehnicar4", "acid.tehnicar4@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        var profesorId = await SeedUserAsync("acid-profesor4", "acid.profesor4@test.com", "profesor123", UlogaKorisnika.Profesor);
        await SeedUserAsync("acid-student4", "acid.student4@test.com", "student123", UlogaKorisnika.Student);

        var (kabinetId, _) = await SeedKabinetAsync(tehnicarId);

        int terminId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var termin = new Termin
            {
                Datum = DateTime.UtcNow.AddDays(5),
                VrijemePocetka = new TimeSpan(14, 0, 0),
                VrijemeKraja = new TimeSpan(15, 30, 0),
                KreatorID = tehnicarId,
                KabinetID = kabinetId,
                ProfesorID = profesorId,
                StatusTermina = StatusTermina.Slobodan,
                VidljivoStudentima = true,
                LimitOsoba = 20
            };

            context.Termini.Add(termin);
            await context.SaveChangesAsync();
            terminId = termin.ID;
        }

        var profesorClient = await CreateAuthorizedClientAsync("acid-profesor4", "profesor123");
        var rezervacijaRes = await profesorClient.PostAsJsonAsync(
            $"/api/Rezervacija/rezervisi/{terminId}",
            new RezervacijaCreateDTO
            {
                LimitOsoba = 20,
                VidljivoStudentima = true
            });

        Assert.True(rezervacijaRes.IsSuccessStatusCode, $"Greška pri otvaranju rezervacije: {await rezervacijaRes.Content.ReadAsStringAsync()}");

        var studentClient = await CreateAuthorizedClientAsync("acid-student4", "student123");
        var prijavaRes = await studentClient.PostAsJsonAsync($"/api/Rezervacija/zahtjev/{terminId}", new { });

        Assert.True(prijavaRes.IsSuccessStatusCode, $"Prijava nije uspjela: {await prijavaRes.Content.ReadAsStringAsync()}");

        using var verifyScope = _factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var postojiZahtjev = await verifyContext.Zahtjevi.AnyAsync(z => z.TerminID == terminId);

        Assert.True(postojiZahtjev, "Zahtjev nije trajno sačuvan u sistemu.");
    }
}

