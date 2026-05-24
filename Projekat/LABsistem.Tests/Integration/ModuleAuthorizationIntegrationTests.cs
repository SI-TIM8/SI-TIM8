using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LABsistem.Application.DTOs;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LABsistem.Tests.Integration
{
    public class ModuleAuthorizationIntegrationTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ModuleAuthorizationIntegrationTests(TestWebApplicationFactory factory)
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

        private async Task<(int objekatId, int kabinetId, int opremaId)> SeedModuleDependenciesAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

            var tehnicar = await context.Korisnici.SingleAsync(x => x.Username == "module-tehnicar");

            var objekat = new Objekat
            {
                Lokacija = "Test objekat",
                RadnoVrijeme = "08-16"
            };
            context.Objekti.Add(objekat);
            await context.SaveChangesAsync();

            var kabinet = new Kabinet
            {
                Naziv = "KabT1",
                KorisnikID = tehnicar.ID,
                ObjekatID = objekat.ID
            };
            context.Kabineti.Add(kabinet);
            await context.SaveChangesAsync();

            var oprema = new Oprema
            {
                Naziv = "Test oprema",
                Kategorija = "Test kategorija",
                SerijskiBroj = 42,
                stanje = StatusOpreme.Ispravno,
                KreatorID = tehnicar.ID,
                KabinetID = kabinet.ID
            };
            context.Oprema.Add(oprema);
            await context.SaveChangesAsync();

            return (objekat.ID, kabinet.ID, oprema.ID);
        }

        [Fact]
        public async Task AnonymousModuleGetEndpoints_ReturnUnauthorized()
        {
            foreach (var path in new[]
                     {
                         "/api/Objekat",
                         "/api/Kabinet",
                         "/api/Termin",
                         "/api/Oprema",
                         "/api/Evidencija"
                     })
            {
                var response = await _client.GetAsync(path);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task StudentMutationEndpoints_ReturnForbidden()
        {
            await SeedUserAsync("module-admin", "module.admin@test.com", "AdminPassword123!", UlogaKorisnika.Admin);
            await SeedUserAsync("module-student", "module.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);
            await SeedUserAsync("module-tehnicar", "module.tehnicar@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);

            var (_, kabinetId, opremaId) = await SeedModuleDependenciesAsync();
            var studentToken = await LoginAsync("module-student", "StudentPassword123!");

            using var kabinetRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Kabinet")
            {
                Content = JsonContent.Create(new KabinetCreateDTO
                {
                    Naziv = "KabS1",
                    KorisnikID = 1,
                    ObjekatID = 1
                })
            };
            kabinetRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var opremaForm = new MultipartFormDataContent
            {
                { new StringContent("OprS1"), "Naziv" },
                { new StringContent("Studentska kategorija"), "Kategorija" },
                { new StringContent("10"), "SerijskiBroj" },
                { new StringContent(((int)StatusOpreme.Ispravno).ToString()), "Stanje" },
                { new StringContent(kabinetId.ToString()), "KabinetID" },
                { new StringContent("1"), "KreatorID" }
            };
            using var opremaRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Oprema")
            {
                Content = opremaForm
            };
            opremaRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            using var terminRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Termin")
            {
                Content = JsonContent.Create(new TerminCreateDTO
                {
                    Datum = new DateTime(2026, 6, 1),
                    VrijemePocetka = new TimeSpan(9, 0, 0),
                    VrijemeKraja = new TimeSpan(10, 0, 0),
                    KreatorID = 1,
                    KabinetID = kabinetId
                })
            };
            terminRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            using var evidencijaRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Evidencija")
            {
                Content = JsonContent.Create(new EvidencijaCreateDTO
                {
                    OpremaID = opremaId,
                    KorisnikID = 1,
                    Status = "Kvar",
                    Komentar = "Test kvar"
                })
            };
            evidencijaRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(kabinetRequest)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(opremaRequest)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(terminRequest)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(evidencijaRequest)).StatusCode);
        }

        [Fact]
        public async Task RoleBoundEndpoints_AllowExpectedRolesOnly()
        {
            await SeedUserAsync("module2-admin", "module2.admin@test.com", "AdminPassword123!", UlogaKorisnika.Admin);
            var profesorId = await SeedUserAsync("module2-profesor", "module2.profesor@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var tehnicarId = await SeedUserAsync("module2-tehnicar", "module2.tehnicar@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            await SeedUserAsync("module2-student", "module2.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int objekatId;
            int kabinetId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat { Lokacija = "ObjP1", RadnoVrijeme = "08-16" };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();
                objekatId = objekat.ID;

                var kabinet = new Kabinet
                {
                    Naziv = "KabP1",
                    KorisnikID = tehnicarId,
                    ObjekatID = objekat.ID
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();
                kabinetId = kabinet.ID;
            }

            var adminToken = await LoginAsync("module2-admin", "AdminPassword123!");
            var profesorToken = await LoginAsync("module2-profesor", "ProfesorPassword123!");
            var tehnicarToken = await LoginAsync("module2-tehnicar", "TehnicarPassword123!");
            var studentToken = await LoginAsync("module2-student", "StudentPassword123!");

            using var profesorObjekatPost = new HttpRequestMessage(HttpMethod.Post, "/api/Objekat")
            {
                Content = JsonContent.Create(new ObjekatCreateDTO
                {
                    Lokacija = "ObjP2",
                    RadnoVrijeme = "08-16"
                })
            };
            profesorObjekatPost.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            using var profesorOpremaGet = new HttpRequestMessage(HttpMethod.Get, "/api/Oprema");
            profesorOpremaGet.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            using var profesorTerminGet = new HttpRequestMessage(HttpMethod.Get, "/api/Termin");
            profesorTerminGet.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            using var studentTerminGet = new HttpRequestMessage(HttpMethod.Get, "/api/Termin");
            studentTerminGet.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            using var profesorTerminPost = new HttpRequestMessage(HttpMethod.Post, "/api/Termin")
            {
                Content = JsonContent.Create(new TerminCreateDTO
                {
                    Datum = new DateTime(2026, 6, 2),
                    VrijemePocetka = new TimeSpan(9, 0, 0),
                    VrijemeKraja = new TimeSpan(10, 0, 0),
                    KreatorID = profesorId,
                    KabinetID = kabinetId
                })
            };
            profesorTerminPost.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            var tehnicarOpremaForm = new MultipartFormDataContent
            {
                { new StringContent("OprT1"), "Naziv" },
                { new StringContent("Tehnicka kategorija"), "Kategorija" },
                { new StringContent("11"), "SerijskiBroj" },
                { new StringContent(((int)StatusOpreme.Ispravno).ToString()), "Stanje" },
                { new StringContent(kabinetId.ToString()), "KabinetID" },
                { new StringContent(tehnicarId.ToString()), "KreatorID" }
            };
            using var tehnicarOpremaPost = new HttpRequestMessage(HttpMethod.Post, "/api/Oprema")
            {
                Content = tehnicarOpremaForm
            };
            tehnicarOpremaPost.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tehnicarToken);

            using var tehnicarKabinetPost = new HttpRequestMessage(HttpMethod.Post, "/api/Kabinet")
            {
                Content = JsonContent.Create(new KabinetCreateDTO
                {
                    Naziv = "KabT2",
                    KorisnikID = tehnicarId,
                    ObjekatID = objekatId,
                    Kapacitet = 20
                })
            };
            tehnicarKabinetPost.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tehnicarToken);

            using var adminKabinetPost = new HttpRequestMessage(HttpMethod.Post, "/api/Kabinet")
            {
                Content = JsonContent.Create(new KabinetCreateDTO
                {
                    Naziv = "KabA1",
                    KorisnikID = tehnicarId,
                    ObjekatID = objekatId,
                    Kapacitet = 30
                })
            };
            adminKabinetPost.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(profesorObjekatPost)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(profesorOpremaGet)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(profesorTerminGet)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(studentTerminGet)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(profesorTerminPost)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(tehnicarOpremaPost)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await _client.SendAsync(tehnicarKabinetPost)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(adminKabinetPost)).StatusCode);
        }
    }
}
