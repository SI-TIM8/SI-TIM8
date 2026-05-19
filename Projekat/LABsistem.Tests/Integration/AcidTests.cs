using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace LABsistem.Tests.Integration;

public class AcidTests
{
    private readonly HttpClient _client;
    private readonly string _baseUrl = "https://labsistem.duckdns.org/";

    public AcidTests()
    {
        _client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    // ─── Pomoćna metoda za login ──────────────────────────────────────────
    private async Task<string> LoginAsync(string username, string password)
    {
        var res = await _client.PostAsJsonAsync("/api/Auth/login", new { username, password });
        var rawBody = await res.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<Dictionary<string, object>>(rawBody);

        if (body == null || !body.ContainsKey("token"))
            throw new Exception($"Login neuspješan za {username}: {rawBody}");

        return body["token"]?.ToString() ?? string.Empty;
    }

    private void SetToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }


    // =========================================================================
    // ATOMICITY
    // =========================================================================

    [Fact]
    public async Task Atomicity_PrijavaNePostojeciTermin_NemaUpisa()
    {
        var token = await LoginAsync("student", "student123");
        SetToken(token);

        var response = await _client.PostAsJsonAsync("/api/Rezervacija/zahtjev/99999", new { });

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Očekivana greška, dobijen: {response.StatusCode}"
        );
    }

    [Fact]
    public async Task Atomicity_PrijavaTerminaSaNevalidnimPodacima_NemaUpisa()
    {
        var token = await LoginAsync("tehnicar", "tehnicar123");
        SetToken(token);

        // Pokušaj kreiranja termina sa nepostojećim kabinetom
        var response = await _client.PostAsJsonAsync("/api/Termin", new
        {
            datum = "2026-08-01T00:00:00.000Z",
            vrijemePocetka = "10:00:00",
            vrijemeKraja = "09:00:00", // kraj prije početka — nevalidno
            kreatorID = 3,
            kabinetID = 99999,      // nepostoji
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    // =========================================================================
    // ISOLATION
    // =========================================================================

    [Fact]
    public async Task Isolation_DvaStudentaKonkurentno_NemaKorumpcijePodataka()
    {
        // Dva odvojena klijenta — simulira dva različita korisnika
        var client1 = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        var client2 = new HttpClient { BaseAddress = new Uri(_baseUrl) };

        var token1 = await LoginAsync("student1", "student123");
        var token2 = await LoginAsync("student2", "student123");

        client1.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
        client2.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);

        // Oba studenta se prijavljuju na isti termin istovremeno
        var task1 = client1.PostAsJsonAsync("/api/Rezervacija/zahtjev/1", new { });
        var task2 = client2.PostAsJsonAsync("/api/Rezervacija/zahtjev/1", new { });

        var rezultati = await Task.WhenAll(task1, task2);

        // Provjeri da nema 500 grešaka — sistem mora ostati stabilan
        Assert.DoesNotContain(rezultati, r => r.StatusCode == HttpStatusCode.InternalServerError);

        // Svaki rezultat mora biti ili uspjeh ili kontrolirana greška (409 conflict)
        Assert.All(rezultati, r =>
            Assert.True(
                r.StatusCode == HttpStatusCode.OK ||
                r.StatusCode == HttpStatusCode.Created ||
                r.StatusCode == HttpStatusCode.Conflict ||
                r.StatusCode == HttpStatusCode.BadRequest,
                $"Neočekivan status: {r.StatusCode}"
            )
        );
    }

    [Fact]
    public async Task Isolation_PreklapanjeTerminaUIstoVrijeme_SamoJedanProlazi()
    {
        var client1 = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        var client2 = new HttpClient { BaseAddress = new Uri(_baseUrl) };

        var tehnicarToken1 = await LoginAsync("tehnicar", "tehnicar123");
        var tehnicarToken2 = await LoginAsync("tehnicar", "tehnicar123");

        client1.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tehnicarToken1);
        client2.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tehnicarToken2);

        // Isti termin u isto vrijeme, isti kabinet — samo jedan smije proći
        var isti = new
        {
            datum = "2026-08-10T00:00:00.000Z",
            vrijemePocetka = "10:00:00",
            vrijemeKraja = "12:00:00",
            kreatorID = 3,
            kabinetID = 3,
        };

        var task1 = client1.PostAsJsonAsync("/api/Termin", isti);
        var task2 = client2.PostAsJsonAsync("/api/Termin", isti);

        var rezultati = await Task.WhenAll(task1, task2);

        int uspjesnih = rezultati.Count(r =>
            r.StatusCode == HttpStatusCode.OK ||
            r.StatusCode == HttpStatusCode.Created);

        // Maksimalno jedan smije proći
        Assert.True(uspjesnih <= 1, $"Očekivan max 1 uspjeh, dobijeno: {uspjesnih}");
    }


    // =========================================================================
    // DURABILITY
    // =========================================================================
    [Fact]
    public async Task Durability_NakonPrijaveZahtjevSeTrajnoSacuva()
    {
        
        // 1. Login kao tehnicar
        
        var tehnicarToken = await LoginAsync("tehnicar", "tehnicar123");
        SetToken(tehnicarToken);

        var datumObj = DateTime.UtcNow.AddDays(5);

        var datumString = datumObj.ToString("yyyy-MM-ddT00:00:00.000Z");

        var vrijemePocetka = "11:00:00";
        var vrijemeKraja = "12:30:00";

        
        // 2. Kreiranje termina
        
        var terminRes = await _client.PostAsJsonAsync("/api/Termin", new
        {
            datum = datumString,
            vrijemePocetka,
            vrijemeKraja,
            kreatorID = 4,
            kabinetID = 1
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
        
        var profesorToken = await LoginAsync("profesor", "profesor123");
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
        
        var studentToken = await LoginAsync("student", "student123");
        SetToken(studentToken);

        
        // 8. Student šalje zahtjev
        
        var prijavaRes =
            await _client.PostAsJsonAsync(
                $"/api/Rezervacija/zahtjev/{terminId}",
                new { });

        Assert.True(
            prijavaRes.IsSuccessStatusCode,
            $"Prijava nije uspjela: {await prijavaRes.Content.ReadAsStringAsync()}"
        );

        
        // 9. Profesor provjerava zahtjeve
        
        SetToken(profesorToken);

        var zahtjevi =
            await _client.GetFromJsonAsync<List<Dictionary<string, object>>>(
                "/api/Rezervacija/dolazni-zahtjevi");

        Assert.NotNull(zahtjevi);

        
        // 10. Provjera perzistencije
        
        var postojiZahtjev = zahtjevi.Any(z =>
        {
            if (!z.ContainsKey("terminID"))
                return false;

            return z["terminID"]?.ToString() == terminId;
        });

        Assert.True(
            postojiZahtjev,
            "Zahtjev nije trajno sačuvan u sistemu."
        );
    }
}