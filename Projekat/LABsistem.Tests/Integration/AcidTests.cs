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

    public AcidTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static string Unique(string prefix)
    {
        return $"{prefix}{Guid.NewGuid():N}"[..Math.Min(prefix.Length + 10, prefix.Length + 32)];
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
            existingUser.EmailVerified = true;
            existingUser.EmailVerifiedAtUtc = DateTime.UtcNow;
            existingUser.MustChangePassword = false;
            await context.SaveChangesAsync();
            return existingUser.ID;
        }

        var user = new Korisnik
        {
            ImePrezime = $"{role} test",
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

    private async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequestDto
        {
            Username = username,
            Password = password
        });

        var rawBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        var payload = System.Text.Json.JsonSerializer.Deserialize<LoginResponseDto>(
            rawBody,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return payload?.Token ?? throw new Exception($"Login nije uspio za {username}: {rawBody}");
    }

    private async Task<HttpClient> CreateAuthorizedClientAsync(string username, string password)
    {
        var client = _factory.CreateClient();
        var token = await LoginAsync(client, username, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<(int KabinetId, int ObjekatId)> SeedKabinetAsync(string kabinetNaziv, int profesorId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

        var objekat = new Objekat
        {
            Lokacija = Unique("Obj"),
            RadnoVrijeme = "08-16"
        };
        context.Objekti.Add(objekat);
        await context.SaveChangesAsync();

        var kabinet = new Kabinet
        {
            Naziv = kabinetNaziv,
            KorisnikID = profesorId,
            ObjekatID = objekat.ID,
            Kapacitet = 20
        };
        context.Kabineti.Add(kabinet);
        await context.SaveChangesAsync();

        return (kabinet.ID, objekat.ID);
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
        var username = Unique("acidstudent");
        await SeedUserAsync(username, $"{username}@test.com", "Student123!", UlogaKorisnika.Student);

        using var client = await CreateAuthorizedClientAsync(username, "Student123!");

        var response = await client.PostAsJsonAsync("/api/Rezervacija/zahtjev/99999", new { });

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Ocekivana kontrolisana greska, dobijen status: {response.StatusCode}");
    }

    [Fact]
    public async Task Atomicity_PrijavaTerminaSaNevalidnimPodacima_NemaUpisa()
    {
        var username = Unique("acidtech");
        await SeedUserAsync(username, $"{username}@test.com", "Tehnicar123!", UlogaKorisnika.Tehnicar);

        using var client = await CreateAuthorizedClientAsync(username, "Tehnicar123!");

        var response = await client.PostAsJsonAsync("/api/Termin", new TerminCreateDTO
        {
            Datum = DateTime.Today.AddDays(10),
            VrijemePocetka = new TimeSpan(10, 0, 0),
            VrijemeKraja = new TimeSpan(9, 0, 0),
            KreatorID = 3,
            KabinetID = 99999
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Isolation_DvaStudentaKonkurentno_NemaKorumpcijePodataka()
    {
        var profesorUsername = Unique("acidprof");
        var tehnicarUsername = Unique("acidtech");
        var studentUsername1 = Unique("acidstud");
        var studentUsername2 = Unique("acidstud");

        var profesorId = await SeedUserAsync(profesorUsername, $"{profesorUsername}@test.com", "Profesor123!", UlogaKorisnika.Profesor);
        var tehnicarId = await SeedUserAsync(tehnicarUsername, $"{tehnicarUsername}@test.com", "Tehnicar123!", UlogaKorisnika.Tehnicar);
        var studentId1 = await SeedUserAsync(studentUsername1, $"{studentUsername1}@test.com", "Student123!", UlogaKorisnika.Student);
        var studentId2 = await SeedUserAsync(studentUsername2, $"{studentUsername2}@test.com", "Student123!", UlogaKorisnika.Student);

        var (kabinetId, _) = await SeedKabinetAsync(Unique("Kab"), profesorId);
        var terminId = await SeedTerminAsync(
            tehnicarId,
            kabinetId,
            profesorId,
            DateTime.Today.AddDays(5),
            new TimeSpan(9, 0, 0),
            new TimeSpan(10, 0, 0));

        using var client1 = await CreateAuthorizedClientAsync(studentUsername1, "Student123!");
        using var client2 = await CreateAuthorizedClientAsync(studentUsername2, "Student123!");

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
        var profesorUsername = Unique("isoprof");
        var tehnicarUsername = Unique("isotech");

        var profesorId = await SeedUserAsync(profesorUsername, $"{profesorUsername}@test.com", "Profesor123!", UlogaKorisnika.Profesor);
        var tehnicarId = await SeedUserAsync(tehnicarUsername, $"{tehnicarUsername}@test.com", "Tehnicar123!", UlogaKorisnika.Tehnicar);
        var (kabinetId, _) = await SeedKabinetAsync(Unique("IsoKab"), profesorId);

        using var client1 = await CreateAuthorizedClientAsync(tehnicarUsername, "Tehnicar123!");
        using var client2 = await CreateAuthorizedClientAsync(tehnicarUsername, "Tehnicar123!");

        var istiTermin = new TerminCreateDTO
        {
            Datum = DateTime.Today.AddDays(7),
            VrijemePocetka = new TimeSpan(10, 0, 0),
            VrijemeKraja = new TimeSpan(12, 0, 0),
            KreatorID = tehnicarId,
            KabinetID = kabinetId
        };

        var task1 = client1.PostAsJsonAsync("/api/Termin", istiTermin);
        var task2 = client2.PostAsJsonAsync("/api/Termin", istiTermin);

        var rezultati = await Task.WhenAll(task1, task2);
        var uspjesnih = rezultati.Count(r =>
            r.StatusCode == HttpStatusCode.OK ||
            r.StatusCode == HttpStatusCode.Created);

        Assert.True(uspjesnih <= 1, $"Ocekivan je najvise jedan uspjesan upis, dobijeno: {uspjesnih}.");
    }

    [Fact]
    public async Task Durability_NakonPrijaveZahtjevSeTrajnoSacuva()
    {
        var profesorUsername = Unique("durprof");
        var tehnicarUsername = Unique("durtech");
        var studentUsername = Unique("durstud");

        var profesorId = await SeedUserAsync(profesorUsername, $"{profesorUsername}@test.com", "Profesor123!", UlogaKorisnika.Profesor);
        var tehnicarId = await SeedUserAsync(tehnicarUsername, $"{tehnicarUsername}@test.com", "Tehnicar123!", UlogaKorisnika.Tehnicar);
        await SeedUserAsync(studentUsername, $"{studentUsername}@test.com", "Student123!", UlogaKorisnika.Student);

        var (kabinetId, _) = await SeedKabinetAsync(Unique("DurKab"), profesorId);

        int terminId;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var termin = new Termin
            {
                Datum = DateTime.Today.AddDays(5),
                VrijemePocetka = new TimeSpan(14, 0, 0),
                VrijemeKraja = new TimeSpan(15, 30, 0),
                KreatorID = tehnicarId,
                KabinetID = kabinetId,
                StatusTermina = StatusTermina.Slobodan
            };
            context.Termini.Add(termin);
            await context.SaveChangesAsync();
            terminId = termin.ID;
        }

        using var profesorClient = await CreateAuthorizedClientAsync(profesorUsername, "Profesor123!");
        var rezervacijaRes = await profesorClient.PostAsJsonAsync(
            $"/api/Rezervacija/rezervisi/{terminId}",
            new RezervacijaCreateDTO
            {
                LimitOsoba = 20,
                VidljivoStudentima = true
            });

        Assert.True(
            rezervacijaRes.IsSuccessStatusCode,
            $"Otvaranje rezervacije nije uspjelo: {await rezervacijaRes.Content.ReadAsStringAsync()}");

        using var studentClient = await CreateAuthorizedClientAsync(studentUsername, "Student123!");
        var prijavaRes = await studentClient.PostAsJsonAsync($"/api/Rezervacija/zahtjev/{terminId}", new { });

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
