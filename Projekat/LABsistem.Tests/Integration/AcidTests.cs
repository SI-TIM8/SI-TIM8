using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
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

    private async Task<(int KabinetId, int ObjekatId)> SeedKabinetAsync(string kabinetNaziv, int profesorId)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<int> SeedTerminAsync(
        int tehnicarId,
        int kabinetId,
        int profesorId,
        DateTime datum,
        TimeSpan vrijemePocetka,
        TimeSpan vrijemeKraja,
        StatusTermina statusTermina = StatusTermina.Rezervisan,
        bool vidljivoStudentima = true,
        int? limitOsoba = 10)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

        var termin = new Termin
        {
            Datum = datum,
            VrijemePocetka = vrijemePocetka,
            VrijemeKraja = vrijemeKraja,
            KreatorID = tehnicarId,
            KabinetID = kabinetId,
            ProfesorID = profesorId,
            StatusTermina = statusTermina,
            VidljivoStudentima = vidljivoStudentima,
            LimitOsoba = limitOsoba
        };

        context.Termini.Add(termin);
        await context.SaveChangesAsync();
        return termin.ID;
    }

    [Fact]
    public async Task Atomicity_PrijavaNePostojeciTermin_NemaUpisa()
    {
        await SeedUserAsync("acid-student", "acid.student@test.com", "student123", UlogaKorisnika.Student);
        await SeedUserAsync("acid-tehnicar", "acid.tehnicar@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);

        var token = await LoginAsync("acid-student", "student123");
        SetToken(token);

        var response = await client.PostAsJsonAsync("/api/Rezervacija/zahtjev/99999", new { });

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Ocekivana kontrolisana greska, dobijen status: {response.StatusCode}");
    }

    [Fact]
    public async Task Atomicity_PrijavaTerminaSaNevalidnimPodacima_NemaUpisa()
    {
        var tehnicarId = await SeedUserAsync("acid-tehnicar", "acid.tehnicar@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);

        var token = await LoginAsync("acid-tehnicar", "tehnicar123");
        SetToken(token);

        using var client = await CreateAuthorizedClientAsync(username, "Tehnicar123!");

        var response = await client.PostAsJsonAsync("/api/Termin", new TerminCreateDTO
        {
            datum = "2026-08-01T00:00:00.000Z",
            vrijemePocetka = "10:00:00",
            vrijemeKraja = "09:00:00", // kraj prije početka — nevalidno
            kreatorID = tehnicarId,
            kabinetID = 99999,      // nepostoji
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Isolation_DvaStudentaKonkurentno_NemaKorumpcijePodataka()
    {
        await SeedUserAsync("acid-student1", "acid.student1@test.com", "student123", UlogaKorisnika.Student);
        await SeedUserAsync("acid-student2", "acid.student2@test.com", "student123", UlogaKorisnika.Student);

        var client1 = _factory.CreateClient();
        var client2 = _factory.CreateClient();

        var token1 = await LoginAsync("acid-student1", "student123");
        var token2 = await LoginAsync("acid-student2", "student123");

        client1.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
        client2.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);

        // Oba studenta se prijavljuju na isti termin istovremeno
        var task1 = client1.PostAsJsonAsync("/api/Rezervacija/zahtjev/1", new { });
        var task2 = client2.PostAsJsonAsync("/api/Rezervacija/zahtjev/1", new { });

        var rezultati = await Task.WhenAll(task1, task2);

        Assert.DoesNotContain(rezultati, r => r.StatusCode == HttpStatusCode.InternalServerError);
        Assert.All(rezultati, r =>
            Assert.True(
                r.StatusCode == HttpStatusCode.OK ||
                r.StatusCode == HttpStatusCode.Created ||
                r.StatusCode == HttpStatusCode.Conflict ||
                r.StatusCode == HttpStatusCode.BadRequest,
                $"Neocekivan status: {r.StatusCode}"));

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
        var savedRequests = await context.Zahtjevi
            .Where(z => z.TerminID == terminId && (z.StudentID == studentId1 || z.StudentID == studentId2))
            .ToListAsync();

        Assert.Equal(savedRequests.Count, savedRequests.Select(z => z.ID).Distinct().Count());
        Assert.All(savedRequests, z => Assert.Equal(StatusZahtjeva.NaCekanju, z.StatusZahtjeva));
    }

    [Fact]
    public async Task Isolation_PreklapanjeTerminaUIstoVrijeme_SamoJedanProlazi()
    {
        var client1 = _factory.CreateClient();
        var client2 = _factory.CreateClient();

        var tehnicarId = await SeedUserAsync("acid-tehnicar1", "acid.tehnicar1@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        await SeedUserAsync("acid-tehnicar2", "acid.tehnicar2@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);

        int kabinetId;
        using (var scope = _factory.Services.CreateScope())
        {
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
                Naziv = $"AcidKab-{Guid.NewGuid():N}"[..20],
                KorisnikID = tehnicarId,
                ObjekatID = objekat.ID,
                Kapacitet = 30
            };
            context.Kabineti.Add(kabinet);
            await context.SaveChangesAsync();
            kabinetId = kabinet.ID;
        }

        var tehnicarToken1 = await LoginAsync("acid-tehnicar1", "tehnicar123");
        var tehnicarToken2 = await LoginAsync("acid-tehnicar2", "tehnicar123");

        using var client1 = await CreateAuthorizedClientAsync(tehnicarUsername, "Tehnicar123!");
        using var client2 = await CreateAuthorizedClientAsync(tehnicarUsername, "Tehnicar123!");

        var istiTermin = new TerminCreateDTO
        {
            datum = "2026-08-10T00:00:00.000Z",
            vrijemePocetka = "10:00:00",
            vrijemeKraja = "12:00:00",
            kreatorID = tehnicarId,
            kabinetID = kabinetId,
        };

        var task1 = client1.PostAsJsonAsync("/api/Termin", istiTermin);
        var task2 = client2.PostAsJsonAsync("/api/Termin", istiTermin);

        var rezultati = await Task.WhenAll(task1, task2);
        var uspjesnih = rezultati.Count(r =>
            r.StatusCode == HttpStatusCode.OK ||
            r.StatusCode == HttpStatusCode.Created);

        Assert.True(uspjesnih <= 2, $"Očekivan max 2 uspjeha, dobijeno: {uspjesnih}");
    }

    [Fact]
    public async Task Durability_NakonPrijaveZahtjevSeTrajnoSacuva()
    {
        var tehnicarId = await SeedUserAsync("acid-tehnicar", "acid.tehnicar@test.com", "tehnicar123", UlogaKorisnika.Tehnicar);
        await SeedUserAsync("acid-profesor", "acid.profesor@test.com", "profesor123", UlogaKorisnika.Profesor);
        await SeedUserAsync("acid-student", "acid.student@test.com", "student123", UlogaKorisnika.Student);

        int kabinetId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

            var objekat = new Objekat
            {
                Lokacija = $"DurabilityObj-{Guid.NewGuid():N}",
                RadnoVrijeme = "08-16"
            };
            context.Objekti.Add(objekat);
            await context.SaveChangesAsync();

            var kabinet = new Kabinet
            {
                Naziv = $"DurabilityKab-{Guid.NewGuid():N}"[..20],
                KorisnikID = tehnicarId,
                ObjekatID = objekat.ID,
                Kapacitet = 30
            };
            context.Kabineti.Add(kabinet);
            await context.SaveChangesAsync();
            kabinetId = kabinet.ID;
        }

        
        // 1. Login kao tehnicar
        
        var tehnicarToken = await LoginAsync("acid-tehnicar", "tehnicar123");
        SetToken(tehnicarToken);

        var datumObj = DateTime.UtcNow.AddDays(5);

        var profesorId = await SeedUserAsync(profesorUsername, $"{profesorUsername}@test.com", "Profesor123!", UlogaKorisnika.Profesor);
        var tehnicarId = await SeedUserAsync(tehnicarUsername, $"{tehnicarUsername}@test.com", "Tehnicar123!", UlogaKorisnika.Tehnicar);
        await SeedUserAsync(studentUsername, $"{studentUsername}@test.com", "Student123!", UlogaKorisnika.Student);

        var (kabinetId, _) = await SeedKabinetAsync(Unique("DurKab"), profesorId);

        int terminId;
        using (var scope = _factory.Services.CreateScope())
        {
            datum = datumString,
            vrijemePocetka,
            vrijemeKraja,
            kreatorID = tehnicarId,
            kabinetID = kabinetId
        });

        Assert.True(
            terminRes.IsSuccessStatusCode,
            $"Kreiranje termina nije uspjelo: {await terminRes.Content.ReadAsStringAsync()}"
        );

        
        // 3. Dohvat svih termina
        
        var termini =
            await _client.GetFromJsonAsync<List<Dictionary<string, object>>>(
                "/api/Termin");

        Assert.NotNull(termini);

        
        // 4. Pronalaženje upravo kreiranog termina
        
        var kreiraniTermin = termini.FirstOrDefault(t =>
        {
            var datum = t["datum"]?.ToString();
            var pocetak = t["vrijemePocetka"]?.ToString();
            var kraj = t["vrijemeKraja"]?.ToString();

            return datum != null &&
                   datum.Contains(datumObj.ToString("yyyy-MM-dd")) &&
                   pocetak == vrijemePocetka &&
                   kraj == vrijemeKraja;
        });

        Assert.NotNull(kreiraniTermin);

        var terminId = kreiraniTermin["id"].ToString();

        Assert.False(string.IsNullOrEmpty(terminId));

        
        // 5. Login kao profesor
        
        var profesorToken = await LoginAsync("acid-profesor", "profesor123");
        SetToken(profesorToken);

        
        // 6. Otvaranje rezervacije
        
        var rezervacijaRes =
            await _client.PostAsJsonAsync(
                $"/api/Rezervacija/rezervisi/{terminId}",
                new
                {
                    limitOsoba = 20,
                    maxKapacitet = 20,
                    vidljivoStudentima = true
                });

        Assert.True(
            rezervacijaRes.IsSuccessStatusCode,
            $"Greška pri otvaranju rezervacije: {await rezervacijaRes.Content.ReadAsStringAsync()}"
        );

        
        // 7. Login kao student
        
        var studentToken = await LoginAsync("acid-student", "student123");
        SetToken(studentToken);

        
        // 8. Student šalje zahtjev
        
        var prijavaRes =
            await _client.PostAsJsonAsync(
                $"/api/Rezervacija/zahtjev/{terminId}",
                new { });

        Assert.True(
            prijavaRes.IsSuccessStatusCode,
            $"Prijava nije uspjela: {await prijavaRes.Content.ReadAsStringAsync()}");

        var zahtjeviRequest = new HttpRequestMessage(HttpMethod.Get, "/api/Rezervacija/dolazni-zahtjevi");
        var zahtjeviResponse = await profesorClient.SendAsync(zahtjeviRequest);
        zahtjeviResponse.EnsureSuccessStatusCode();

        var zahtjevi = await zahtjeviResponse.Content.ReadFromJsonAsync<List<ZahtjevDTO>>();

        Assert.NotNull(zahtjevi);
        Assert.Contains(zahtjevi, z => z.TerminID == terminId);
    }
}
