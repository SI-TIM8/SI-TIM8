using System.Net.Http.Headers;
using System.Net.Http.Json;
using LABsistem.Api.Services;
using LABsistem.Api.Options;
using LABsistem.Api.Validators;
using LABsistem.Application.DTOs;
using LABsistem.Application.DTOs.Auth;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LabSistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

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
                existingUser.EmailVerified = true;
                existingUser.EmailVerifiedAtUtc = DateTime.UtcNow;
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

        private TestEmailNotificationService GetEmailService()
        {
            using var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<TestEmailNotificationService>();
        }

        private IReservationReminderService CreateReminderService(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();

            return new ReservationReminderService(
                context,
                emailService,
                Options.Create(new ReservationReminderOptions
                {
                    Enabled = true,
                    DeliveryWindowMinutes = 5,
                    OffsetsMinutes = [1440, 60]
                }),
                NullLogger<ReservationReminderService>.Instance);
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

                var vidljiviTermin = new Termin
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
                };
                var skriveniTermin = new Termin
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
                };
                var slobodanTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(2),
                    VrijemePocetka = new TimeSpan(13, 0, 0),
                    VrijemeKraja = new TimeSpan(14, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    StatusTermina = StatusTermina.Slobodan,
                    VidljivoStudentima = true
                };

                context.Termini.AddRange(vidljiviTermin, skriveniTermin, slobodanTermin);
                await context.SaveChangesAsync();

                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = vidljiviTermin.ID,
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
            var matchingTerms = payload.Where(t => t.KabinetNaziv == "Rez kabinet").ToList();

            Assert.Single(matchingTerms);
            Assert.Equal("Rezervisan", matchingTerms[0].StatusTermina);
            Assert.True(matchingTerms[0].VidljivoStudentima);
            Assert.Equal("NaCekanju", matchingTerms[0].StatusPrijave);
        }

        [Fact]
        public async Task GetMoje_ForStudent_ReturnsOnlyApprovedReservations()
        {
            var tehnicarId = await SeedUserAsync("mine-tech", "mine.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("mine-prof", "mine.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("mine-student", "mine.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Moje objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Moje kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var approvedTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(3),
                    VrijemePocetka = new TimeSpan(9, 0, 0),
                    VrijemeKraja = new TimeSpan(10, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };
                var pendingTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(4),
                    VrijemePocetka = new TimeSpan(11, 0, 0),
                    VrijemeKraja = new TimeSpan(12, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };

                context.Termini.AddRange(approvedTermin, pendingTermin);
                await context.SaveChangesAsync();

                context.Zahtjevi.AddRange(
                    new Zahtjev
                    {
                        StudentID = studentId,
                        TerminID = approvedTermin.ID,
                        StatusZahtjeva = StatusZahtjeva.Odobren,
                        Komentar = string.Empty
                    },
                    new Zahtjev
                    {
                        StudentID = studentId,
                        TerminID = pendingTermin.ID,
                        StatusZahtjeva = StatusZahtjeva.NaCekanju,
                        Komentar = string.Empty
                    });
                await context.SaveChangesAsync();
            }

            var studentToken = await LoginAsync("mine-student", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Rezervacija/moje");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<List<TerminDTO>>();

            Assert.NotNull(payload);
            Assert.Single(payload);
            Assert.Equal("Moje kabinet", payload[0].KabinetNaziv);
            Assert.Equal(DateTime.Today.AddDays(3), payload[0].Datum.Date);
        }

        [Fact]
        public async Task GetMojiZahtjevi_ForStudent_ReturnsNonApprovedRequests()
        {
            var tehnicarId = await SeedUserAsync("requests-tech", "requests.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("requests-prof", "requests.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("requests-student", "requests.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Zahtjevi objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Zahtjevi kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var pendingTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(4),
                    VrijemePocetka = new TimeSpan(9, 0, 0),
                    VrijemeKraja = new TimeSpan(10, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };
                var rejectedTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(5),
                    VrijemePocetka = new TimeSpan(11, 0, 0),
                    VrijemeKraja = new TimeSpan(12, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };
                var approvedTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(6),
                    VrijemePocetka = new TimeSpan(13, 0, 0),
                    VrijemeKraja = new TimeSpan(14, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };

                context.Termini.AddRange(pendingTermin, rejectedTermin, approvedTermin);
                await context.SaveChangesAsync();

                context.Zahtjevi.AddRange(
                    new Zahtjev
                    {
                        StudentID = studentId,
                        TerminID = pendingTermin.ID,
                        StatusZahtjeva = StatusZahtjeva.NaCekanju,
                        Komentar = string.Empty
                    },
                    new Zahtjev
                    {
                        StudentID = studentId,
                        TerminID = rejectedTermin.ID,
                        StatusZahtjeva = StatusZahtjeva.Odbijen,
                        Komentar = string.Empty
                    },
                    new Zahtjev
                    {
                        StudentID = studentId,
                        TerminID = approvedTermin.ID,
                        StatusZahtjeva = StatusZahtjeva.Odobren,
                        Komentar = string.Empty
                    });
                await context.SaveChangesAsync();
            }

            var studentToken = await LoginAsync("requests-student", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Rezervacija/moji-zahtjevi");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<List<StudentZahtjevDTO>>();

            Assert.NotNull(payload);
            Assert.Equal(2, payload.Count);
            Assert.Contains(payload, z => z.StatusZahtjeva == "NaCekanju" && z.MozeOtkazati);
            Assert.Contains(payload, z => z.StatusZahtjeva == "Odbijen" && !z.MozeOtkazati);
            Assert.DoesNotContain(payload, z => z.StatusZahtjeva == "Odobren");
        }

        [Fact]
        public async Task OdgovoriNaZahtjev_WhenProfesorApproves_UpdatesStatusAndRemovesPendingEntry()
        {
            var tehnicarId = await SeedUserAsync("approve-tech", "approve.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("approve-prof", "approve.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("approve-student", "approve.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int zahtjevId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Approve objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Approve kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var termin = new Termin
                {
                    Datum = DateTime.Today.AddDays(5),
                    VrijemePocetka = new TimeSpan(8, 0, 0),
                    VrijemeKraja = new TimeSpan(9, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();

                var zahtjev = new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = termin.ID,
                    StatusZahtjeva = StatusZahtjeva.NaCekanju,
                    Komentar = string.Empty
                };
                context.Zahtjevi.Add(zahtjev);
                await context.SaveChangesAsync();
                zahtjevId = zahtjev.ID;
            }

            var profesorToken = await LoginAsync("approve-prof", "ProfesorPassword123!");
            using var odgovorRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/odgovor/{zahtjevId}?odobri=true");
            odgovorRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            var odgovorResponse = await _client.SendAsync(odgovorRequest);
            odgovorResponse.EnsureSuccessStatusCode();

            using var dolazniRequest = new HttpRequestMessage(HttpMethod.Get, "/api/Rezervacija/dolazni-zahtjevi");
            dolazniRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            var dolazniResponse = await _client.SendAsync(dolazniRequest);
            dolazniResponse.EnsureSuccessStatusCode();
            var dolazniPayload = await dolazniResponse.Content.ReadFromJsonAsync<List<ZahtjevDTO>>();

            Assert.NotNull(dolazniPayload);
            Assert.Empty(dolazniPayload);

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var updatedZahtjev = await verificationContext.Zahtjevi.FindAsync(zahtjevId);

            Assert.NotNull(updatedZahtjev);
            Assert.Equal(StatusZahtjeva.Odobren, updatedZahtjev!.StatusZahtjeva);
            var obavijest = await verificationContext.Obavijesti
                .FirstOrDefaultAsync(o => o.KorisnikID == studentId && o.TerminID == updatedZahtjev.TerminID);
            Assert.NotNull(obavijest);
            Assert.Contains("odobren", obavijest!.Novosti, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task OtkaziTermin_ResetsSlotAndCancelsLinkedRequests()
        {
            var tehnicarId = await SeedUserAsync("cancel-tech", "cancel.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("cancel-prof", "cancel.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("cancel-student", "cancel.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int terminId;
            int zahtjevId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Cancel objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Cancel kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var termin = new Termin
                {
                    Datum = DateTime.Today.AddDays(6),
                    VrijemePocetka = new TimeSpan(14, 0, 0),
                    VrijemeKraja = new TimeSpan(15, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();
                terminId = termin.ID;

                var zahtjev = new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = terminId,
                    StatusZahtjeva = StatusZahtjeva.NaCekanju,
                    Komentar = string.Empty
                };
                context.Zahtjevi.Add(zahtjev);
                await context.SaveChangesAsync();
                zahtjevId = zahtjev.ID;
            }

            var profesorToken = await LoginAsync("cancel-prof", "ProfesorPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/otkazi/{terminId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var updatedTermin = await verificationContext.Termini.FindAsync(terminId);
            var updatedZahtjev = await verificationContext.Zahtjevi.FindAsync(zahtjevId);

            Assert.NotNull(updatedTermin);
            Assert.Equal(StatusTermina.Slobodan, updatedTermin!.StatusTermina);
            Assert.Null(updatedTermin.ProfesorID);
            Assert.Null(updatedTermin.LimitOsoba);
            Assert.False(updatedTermin.VidljivoStudentima);

            Assert.NotNull(updatedZahtjev);
            Assert.Equal(StatusZahtjeva.Otkazan, updatedZahtjev!.StatusZahtjeva);
        }

        [Fact]
        public async Task OtkaziTermin_WhenStudentCancelsApprovedReservation_CancelsOnlyStudentsReservation()
        {
            var tehnicarId = await SeedUserAsync("student-cancel-tech", "student.cancel.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("student-cancel-prof", "student.cancel.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("student-cancel-user", "student.cancel.user@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int terminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Student cancel objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Student cancel kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var termin = new Termin
                {
                    Datum = DateTime.Today.AddDays(3),
                    VrijemePocetka = new TimeSpan(15, 0, 0),
                    VrijemeKraja = new TimeSpan(16, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 5
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();
                terminId = termin.ID;

                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = terminId,
                    StatusZahtjeva = StatusZahtjeva.Odobren,
                    Komentar = string.Empty
                });
                await context.SaveChangesAsync();
            }

            var studentToken = await LoginAsync("student-cancel-user", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/otkazi/{terminId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var updatedZahtjev = await verificationContext.Zahtjevi
                .FirstOrDefaultAsync(z => z.StudentID == studentId && z.TerminID == terminId);
            var profesorObavijest = await verificationContext.Obavijesti
                .FirstOrDefaultAsync(o => o.KorisnikID == profesorId && o.TerminID == terminId);

            Assert.NotNull(updatedZahtjev);
            Assert.Equal(StatusZahtjeva.Otkazan, updatedZahtjev!.StatusZahtjeva);
            Assert.NotNull(profesorObavijest);
            Assert.Contains("otkazao dolazak", profesorObavijest!.Novosti, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task OtkaziStudentovZahtjev_StudentCancelsPendingRequest()
        {
            var tehnicarId = await SeedUserAsync("pending-cancel-tech", "pending.cancel.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("pending-cancel-prof", "pending.cancel.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("pending-cancel-student", "pending.cancel.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int zahtjevId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Povlačenje zahtjeva objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Povlačenje zahtjeva kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var termin = new Termin
                {
                    Datum = DateTime.Today.AddDays(4),
                    VrijemePocetka = new TimeSpan(10, 0, 0),
                    VrijemeKraja = new TimeSpan(11, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 5
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();

                var zahtjev = new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = termin.ID,
                    StatusZahtjeva = StatusZahtjeva.NaCekanju,
                    Komentar = string.Empty
                };
                context.Zahtjevi.Add(zahtjev);
                await context.SaveChangesAsync();
                zahtjevId = zahtjev.ID;
            }

            var studentToken = await LoginAsync("pending-cancel-student", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/otkazi-zahtjev/{zahtjevId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var updatedZahtjev = await verificationContext.Zahtjevi.FindAsync(zahtjevId);

            Assert.NotNull(updatedZahtjev);
            Assert.Equal(StatusZahtjeva.Otkazan, updatedZahtjev!.StatusZahtjeva);
        }

        [Fact]
        public async Task ReservationReminderService_SendsInAppAndEmailReminder_ForVerifiedStudent()
        {
            var emailService = GetEmailService();
            emailService.Clear();

            var tehnicarId = await SeedUserAsync("reminder-tech", "reminder.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("reminder-prof", "reminder.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("reminder-student", "reminder.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            DateTime currentTime;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Reminder objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Reminder kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                currentTime = new DateTime(2099, 1, 10, 9, 0, 0);
                var termin = new Termin
                {
                    Datum = currentTime.Date,
                    VrijemePocetka = currentTime.AddMinutes(58).TimeOfDay,
                    VrijemeKraja = currentTime.AddHours(1).TimeOfDay,
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 5
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();

                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = termin.ID,
                    StatusZahtjeva = StatusZahtjeva.Odobren,
                    Komentar = string.Empty
                });
                await context.SaveChangesAsync();
            }

            using (var scope = _factory.Services.CreateScope())
            {
                var reminderService = CreateReminderService(scope);
                var sent = await reminderService.SendDueRemindersAsync(currentTime);
                Assert.Equal(1, sent);

                var sentAgain = await reminderService.SendDueRemindersAsync(currentTime);
                Assert.Equal(0, sentAgain);
            }

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var obavijest = await verificationContext.Obavijesti
                .FirstOrDefaultAsync(o => o.KorisnikID == studentId && o.Novosti.Contains("Podsjetnik:", StringComparison.Ordinal));
            var dispatch = await verificationContext.ReservationReminderDispatches.SingleAsync();

            Assert.NotNull(obavijest);
            Assert.Equal(60, dispatch.ReminderOffsetMinutes);

            var sentEmail = Assert.Single(emailService.ReservationReminderEmails);
            Assert.Equal("reminder.student@test.com", sentEmail.RecipientEmail);
            Assert.Equal("Reminder kabinet", sentEmail.KabinetNaziv);
            Assert.Equal("za 1 sat", sentEmail.ReminderLeadTimeText);
        }

        [Fact]
        public async Task ReservationReminderService_DoesNotSendEmail_ForUnverifiedStudent()
        {
            var emailService = GetEmailService();
            emailService.Clear();

            var tehnicarId = await SeedUserAsync("unverified-reminder-tech", "unverified.reminder.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("unverified-reminder-prof", "unverified.reminder.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("unverified-reminder-student", "unverified.reminder.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
                var student = await context.Korisnici.FirstAsync(x => x.ID == studentId);
                student.EmailVerified = false;
                student.EmailVerifiedAtUtc = null;

                var objekat = new Objekat
                {
                    Lokacija = "Unverified reminder objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Unverified reminder kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var currentTime = new DateTime(2099, 1, 12, 9, 0, 0);
                var termin = new Termin
                {
                    Datum = currentTime.Date,
                    VrijemePocetka = currentTime.AddMinutes(58).TimeOfDay,
                    VrijemeKraja = currentTime.AddHours(1).TimeOfDay,
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 5
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();

                context.Zahtjevi.Add(new Zahtjev
                {
                    StudentID = studentId,
                    TerminID = termin.ID,
                    StatusZahtjeva = StatusZahtjeva.Odobren,
                    Komentar = string.Empty
                });
                await context.SaveChangesAsync();

                var reminderService = CreateReminderService(scope);
                var sent = await reminderService.SendDueRemindersAsync(currentTime);
                Assert.Equal(1, sent);
            }

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var obavijest = await verificationContext.Obavijesti
                .FirstOrDefaultAsync(o => o.KorisnikID == studentId && o.Novosti.Contains("Podsjetnik:", StringComparison.Ordinal));

            Assert.NotNull(obavijest);
            Assert.Empty(emailService.ReservationReminderEmails);
        }

        [Fact]
        public async Task RezervisiTermin_ProfessorCanReserveSlot()
        {
            var tehnicarId = await SeedUserAsync("reserve-tech", "reserve.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("reserve-prof", "reserve.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);

            int terminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Reserve objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Reserve kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 30
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var termin = new Termin
                {
                    Datum = DateTime.Today.AddDays(7),
                    VrijemePocetka = new TimeSpan(9, 0, 0),
                    VrijemeKraja = new TimeSpan(10, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    StatusTermina = StatusTermina.Slobodan,
                    VidljivoStudentima = false
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();
                terminId = termin.ID;
            }

            var profesorToken = await LoginAsync("reserve-prof", "ProfesorPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/rezervisi/{terminId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", profesorToken);
            request.Content = JsonContent.Create(new RezervacijaCreateDTO
            {
                LimitOsoba = 10,
                VidljivoStudentima = true
            });

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var updatedTermin = await verificationContext.Termini.FindAsync(terminId);

            Assert.NotNull(updatedTermin);
            Assert.Equal(profesorId, updatedTermin!.ProfesorID);
            Assert.Equal(StatusTermina.Rezervisan, updatedTermin.StatusTermina);
            Assert.Equal(10, updatedTermin.LimitOsoba);
            Assert.True(updatedTermin.VidljivoStudentima);
        }

        [Fact]
        public async Task PosaljiZahtjev_StudentCreatesRequest()
        {
            var tehnicarId = await SeedUserAsync("request-tech", "request.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("request-prof", "request.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("request-student", "request.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int terminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Request objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Request kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 10
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                var termin = new Termin
                {
                    Datum = DateTime.Today.AddDays(8),
                    VrijemePocetka = new TimeSpan(11, 0, 0),
                    VrijemeKraja = new TimeSpan(12, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 5
                };
                context.Termini.Add(termin);
                await context.SaveChangesAsync();
                terminId = termin.ID;
            }

            var studentToken = await LoginAsync("request-student", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/zahtjev/{terminId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var verificationScope = _factory.Services.CreateScope();
            var verificationContext = verificationScope.ServiceProvider.GetRequiredService<LabSistemDbContext>();
            var zahtjev = await verificationContext.Zahtjevi.FirstOrDefaultAsync(z => z.TerminID == terminId && z.StudentID == studentId);

            Assert.NotNull(zahtjev);
            Assert.Equal(StatusZahtjeva.NaCekanju, zahtjev!.StatusZahtjeva);
        }

        [Fact]
        public async Task PosaljiZahtjev_ReturnsBadRequest_WhenMaxActiveRequestsReached()
        {
            var tehnicarId = await SeedUserAsync("limit-tech", "limit.tech@test.com", "TehnicarPassword123!", UlogaKorisnika.Tehnicar);
            var profesorId = await SeedUserAsync("limit-prof", "limit.prof@test.com", "ProfesorPassword123!", UlogaKorisnika.Profesor);
            var studentId = await SeedUserAsync("limit-student", "limit.student@test.com", "StudentPassword123!", UlogaKorisnika.Student);

            int terminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

                var objekat = new Objekat
                {
                    Lokacija = "Limit objekat",
                    RadnoVrijeme = "08-16"
                };
                context.Objekti.Add(objekat);
                await context.SaveChangesAsync();

                var kabinet = new Kabinet
                {
                    Naziv = "Limit kabinet",
                    KorisnikID = profesorId,
                    ObjekatID = objekat.ID,
                    Kapacitet = 20
                };
                context.Kabineti.Add(kabinet);
                await context.SaveChangesAsync();

                for (var i = 0; i < RezervacijaValidator.MaxAktivnihZahtjevaPoStudentu; i++)
                {
                    var activeTermin = new Termin
                    {
                        Datum = DateTime.Today.AddDays(20 + i),
                        VrijemePocetka = new TimeSpan(8, 0, 0),
                        VrijemeKraja = new TimeSpan(9, 0, 0),
                        KreatorID = tehnicarId,
                        KabinetID = kabinet.ID,
                        ProfesorID = profesorId,
                        StatusTermina = StatusTermina.Rezervisan,
                        VidljivoStudentima = true,
                        LimitOsoba = 10
                    };
                    context.Termini.Add(activeTermin);
                    await context.SaveChangesAsync();

                    context.Zahtjevi.Add(new Zahtjev
                    {
                        StudentID = studentId,
                        TerminID = activeTermin.ID,
                        StatusZahtjeva = i % 2 == 0 ? StatusZahtjeva.NaCekanju : StatusZahtjeva.Odobren,
                        Komentar = string.Empty
                    });
                    await context.SaveChangesAsync();
                }

                var newTermin = new Termin
                {
                    Datum = DateTime.Today.AddDays(40),
                    VrijemePocetka = new TimeSpan(10, 0, 0),
                    VrijemeKraja = new TimeSpan(11, 0, 0),
                    KreatorID = tehnicarId,
                    KabinetID = kabinet.ID,
                    ProfesorID = profesorId,
                    StatusTermina = StatusTermina.Rezervisan,
                    VidljivoStudentima = true,
                    LimitOsoba = 10
                };
                context.Termini.Add(newTermin);
                await context.SaveChangesAsync();
                terminId = newTermin.ID;
            }

            var studentToken = await LoginAsync("limit-student", "StudentPassword123!");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/Rezervacija/zahtjev/{terminId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);

            var response = await _client.SendAsync(request);

            Assert.Equal(400, (int)response.StatusCode);
            var payload = await response.Content.ReadAsStringAsync();
            Assert.Contains("Dostignut je maksimalan broj aktivnih zahtjeva", payload);
        }
    }
}
