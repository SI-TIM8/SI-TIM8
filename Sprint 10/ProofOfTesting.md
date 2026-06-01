# Proof of Testing – Sprint 10

Ovaj izvještaj dokumentuje validaciju novih i proširenih modula sistema kroz unit i integracijske testove koji su dodani u Sprintu 10.

---

## 1. Modul: Autorizacija po modulima (ModuleAuthorizationIntegrationTests)

*Fokus: Verifikacija da su svi API endpointi ispravno zaštićeni prema korisničkoj ulozi — anonimni korisnici, studenti i ovlaštene uloge.*

### Integration Tests

| Test Case | Opis | Status | Trajanje |
| :--- | :--- | :--- | :--- |
| **AnonymousModuleGetEndpoin...** | Provjera da anonimni korisnik ne može pristupiti zaštićenim GET endpointima | ✅ PASS | 15 ms |
| **RoleBoundEndpoints_AllowExp...** | Provjera da korisnici sa odgovarajućom ulogom mogu pristupiti endpointima koji su im namijenjeni | ✅ PASS | 1,8 sec |
| **StudentMutationEndpoints_Ret...** | Provjera da student ne može izvršiti mutacijske operacije (POST/PUT/DELETE) koje su rezervisane za druge uloge | ✅ PASS | 3,7 sec |

---

## 2. Modul: Email Notifikacije (ResendEmailNotificationServiceTests)

*Fokus: Verifikacija da servis za slanje emailova ispravno poziva odgovarajuće metode za sve tipove notifikacija koje sistem šalje korisnicima.*

### Unit Tests

| Test Case | Opis | Status | Trajanje |
| :--- | :--- | :--- | :--- |
| **SendEmailVerificationEmailAsync** | Provjera da se verifikacioni email ispravno šalje nakon registracije ili promjene email adrese | ✅ PASS | 17 ms |
| **SendPasswordResetEmailAsync** | Provjera da se email za reset lozinke ispravno generišei šalje na korisničku adresu | ✅ PASS | 4 ms |
| **SendReservationDecisionEmail** | Provjera da student dobija email obavijest o odobrenju ili odbijanju rezervacijskog zahtjeva | ✅ PASS | 2 ms |
| **SendReservationReminderEmail** | Provjera da se podsjetnik pred termin ispravno šalje korisnicima sa verifikovanom email adresom | ✅ PASS | 6 ms |

---

## 3. Modul: Podsjetnici za termine (ReservationReminderServiceTests)

*Fokus: Verifikacija da servis za podsjetnike ispravno identifikuje nadolazeće termine i okida slanje notifikacija u pravom trenutku.*

### Unit Tests

| Test Case | Opis | Status | Trajanje |
| :--- | :--- | :--- | :--- |
| **ReservationReminderService_D...** | Provjera da servis ne šalje podsjetnike za termine koji su već prošli ili otkazani | ✅ PASS | 595 ms |
| **ReservationReminderService_Se...** | Provjera da servis ispravno šalje podsjetnike za aktivne buduće termine unutar definisanog vremenskog prozora | ✅ PASS | 642 ms |

---

## 4. Modul: Upravljanje sesijama (DatabaseRevokedTokenStoreTests)

*Fokus: Verifikacija da se opozvani refresh tokeni trajno bilježe u bazi i da se ne mogu ponovo koristiti.*

### Integration Tests

| Test Case | Opis | Status | Trajanje |
| :--- | :--- | :--- | :--- |
| **RevokeAsync_PersistsRevokedT...** | Provjera da se opoziv refresh tokena trajno sprema u tabelu i da zapis ostaje perzistentan nakon `SaveChangesAsync()` | ✅ PASS | 1,4 sec |

---

## 5. Modul: Inicijalizacija baze (LabSistemDbSeederTests)

*Fokus: Verifikacija da seed metode ispravno kreiraju početne podatke (kabinete, objekte, korisnike) u bazi bez grešaka i dupliranja.*

### Integration Tests

| Test Case | Opis | Status | Trajanje |
| :--- | :--- | :--- | :--- |
| **SeedDefaultKabinetiAsync_Wit...** | Provjera da se podrazumijevani kabineti ispravno kreiraju pri inicijalnom pokretanju sistema | ✅ PASS | 798 ms |
| **SeedDefaultObjektiAsync_With...** | Provjera da se podrazumijevani objekti (zgrade/lokacije) ispravno upisuju u bazu | ✅ PASS | 15 ms |
| **SeedDefaultUsersAsync_WithEx...** | Provjera da se podrazumijevani korisnici (admin, tehničar itd.) ispravno kreiraju, uključujući provjeru da ne dolazi do duplikata | ✅ PASS | 3,7 sec |

---

## 6. Modul: Rezervacije — novi scenariji (RezervacijaIntegrationTests — proširenje)

*Fokus: Dodatni integracijski testovi koji pokrivaju edge-case scenarije upravljanja zahtjevima i otkazivanjem koji nisu bili dokumentovani u prethodnim sprintovima.*

### Integration Tests

| Test Case | Opis | Status | Trajanje |
| :--- | :--- | :--- | :--- |
| **GetMojiZahtjevi_ForStudent_Re...** | Provjera da `/api/Rezervacija/moji-zahtjevi` vraća ispravno filtrirane zahtjeve za prijavljenog studenta | ✅ PASS | 857 ms |
| **OtkaziStudentovZahtjev_Studen...** | Provjera da student može poništiti vlastiti zahtjev koji je još na čekanju, uz ispravnu promjenu statusa | ✅ PASS | 665 ms |
| **OtkaziTermin_WhenStudentCan...** | Provjera da student ne može otkazati termin koji ne ispunjava uvjete (npr. unutar 24h ili već otkazan) | ✅ PASS | 654 ms |
| **PosaljiZahtjev_ReturnsBadRequ...** | Provjera da sistem vraća `400 BadRequest` pri slanju zahtjeva koji krši validacijska pravila (popunjen termin, nevidljiv termin i sl.) | ✅ PASS | 670 ms |

---

## Korištene tehnologije i metodologija

- **XUnit** — test runner i assertion framework
- **EF Core In-Memory Database** — brzo izvršavanje integracijskih testova bez side-efekata na produkcijsku bazu
- **TestWebApplicationFactory** — simulacija web aplikacije sa autentifikacijom i dependency injection-om
- **Moq** — mockanje vanjskih servisa (email, notifikacije) u unit testovima
- **Arrange-Act-Assert Pattern** — standardna struktura za čitljivost testova

---

## Rezultati Testiranja

| Metrika | Rezultat | Status |
| :--- | :--- | :--- |
| **Novi unit testovi (Sprint 10)** | 6 | ✅ 100% PASS |
| **Novi integracijski testovi (Sprint 10)** | 11 | ✅ 100% PASS |
| **Ukupno novih testova** | 17 | ✅ 100% PASS |
| **Ukupno testova u projektu** | 136 | ✅ 100% PASS |
| **Ukupno trajanje cijelog test runa** | 1,2 min | ✅ Prihvatljivo |
| **Business Rules pokriveni** | Svi novi scenariji iz Sprinta 10 | ✅ Kompletan |
| **Authorization coverage** | 100% — anonimni, student, ovlaštene uloge | ✅ Sigurno |

---

## Novi business rules pokriveni u Sprintu 10

| BR ID | Opis | Test | Status |
| :--- | :--- | :--- | :--- |
| **BR-AUTH-010** | Anonimni korisnik ne može pristupiti zaštićenim endpointima | ModuleAuth TC1 | ✅ PASS |
| **BR-AUTH-011** | Student ne može izvršavati mutacije rezervisane za druge uloge | ModuleAuth TC3 | ✅ PASS |
| **BR-NOT-001** | Verifikacioni email se šalje nakon promjene email adrese | ResendEmail TC1 | ✅ PASS |
| **BR-NOT-002** | Email podsjetnik se šalje samo korisnicima sa verifikovanom adresom | ResendEmail TC4, Reminder TC2 | ✅ PASS |
| **BR-NOT-003** | Otkazani termini ne dobijaju podsjetnike | Reminder TC1 | ✅ PASS |
| **BR-RES-011** | Student može poništiti vlastiti zahtjev na čekanju | RezervacijaInt — OtkaziStudent | ✅ PASS |
| **BR-RES-012** | Zahtjev koji krši validacijska pravila vraća 400 | RezervacijaInt — PosaljiZahtjev | ✅ PASS |
| **BR-SEC-005** | Opozvan refresh token ostaje trajno perzistentan u bazi | DatabaseRevokedToken TC1 | ✅ PASS |
| **BR-SEED-001** | Inicijalni podaci se kreiraju bez dupliranja | DbSeeder TC1-3 | ✅ PASS |

---

## Zaključak

✅ **SVE TESTNE SEKCIJE SPRINT 10 USPJEŠNO VALIDIRANE** — Novi moduli (email notifikacije, podsjetnici, autorizacija po modulima, upravljanje sesijama i inicijalizacija baze) prošli su sve faze testiranja. Projekat u cjelini broji **136 testova sa 100% prolaznosti** u finalnom test runu.

**Datum:** 2026-06-01 | **Sprint:** Sprint 10 | **Status:** ✅ PRODUCTION READY