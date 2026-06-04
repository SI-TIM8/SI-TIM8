# Architecture & Technical Overview: LABsistem

Ovaj dokument pruža sveobuhvatan tehnički i arhitektonski pregled sistema **LABsistem**, softverskog rješenja za upravljanje laboratorijama, kabinetima, opremom i rezervacijama termina. 

- primarna tehnička referenca za razvoj, testiranje, deployment i održavanje sistema

---

## 1. Arhitektonski Obrazac i Pregled Sistema

**LABsistem** je dizajniran kao **Modularni Monolit**. Sistem je podijeljen na jasno definisane slojeve i tehničke cjeline unutar jednog repozitorijuma (*Mono-repo* pristup), što omogućava lakši razvoj, konzistentan deployment i visoke performanse, uz zadržavanje jasnih granica između biznis modula.

### Ključne komponente komunikacije:
* **Klijent -> Server:** Komunikacija se odvija asinhrono preko **REST API** protokola (`/api/*`). Frontend aplikacija šalje HTTP zahtjeve ka backendu.
* **Server -> Baza podataka:** Backend komunicira sa **PostgreSQL** bazom podataka isključivo putem **Entity Framework Core (EF Core)** ORM-a.
* **Server -> Eksterni servisi:** Za slanje email obavijesti i transakcijskih poruka, backend je integrisan sa eksternim provajderom **Resend API**.

---

## 2. Tehnološki Stack (Tech Stack)

Sistem je izgrađen korišćenjem modernih, stabilnih i široko prihvaćenih tehnologija koje osiguravaju skalabilnost i sigurnost.

| Sloj / Komponenta | Tehnologija | Verzija / Specifikacija | Uloga i Razlog Izbora |
| :--- | :--- | :--- | :--- |
| **Backend Framework** | ASP.NET Core (Web SDK) | `.NET 10.0` (Moderni .NET) | Visoke performanse, tipiziran kod, ugrađen DI kontejner i middleware podrška. |
| **Frontend Framework** | React | `v18 + React Router` | Komponentalna arhitektura, brz rendering i glatko SPA (*Single Page Application*) iskustvo. |
| **Frontend Build Tool** | Vite | Najnovija stabilna | Ekstremno brz razvojni server (HMR) i optimizovan produkcijski build. |
| **HTTP Klijent** | Axios | Standardna | Rukovanje asinhronim HTTP zahtjevima, presretanje (*interceptors*) za JWT. |
| **Baza podataka** | PostgreSQL | `v16` (Dockerized) | Pouzdana, open-source relaciono-objektna baza sa odličnim performansama. |
| **ORM** | Entity Framework Core | Kompatibilna sa .NET 10 | *Code-First* pristup, automatsko upravljanje migracijama i mapiranje entiteta. |
| **Sigurnost (Auth)** | JWT & BCrypt.Net-Next | Standardna | Bezbjedno heširanje lozinki i tokenizovana sesija klijenta. |

---

## 3. Struktura Projekta i Organizacija Koda

Repozitorijum je organizovan oko korijenskog direktorijuma `Projekat/` koji sadrži četiri glavne tehničke i logičke cjeline:
Projekat/
├── LabSistem.backend/          # Backend rješenje (.NET Solution)
│   ├── LabSistem.slnx          # Moderni fajl konfiguracije rješenja
│   ├── LABsistem.Presentation/ # API Host, Kontroleri, Program.cs (Entry point)
│   ├── LABsistem.Api/          # Poslovna logika, Servisi, DTOs, Validatori
│   ├── LabSistem.Dal/          # Data Access Layer, DbContext, Migracije
│   └── LabSistem.Domain/       # Core Entiteti, Enumi, Apstrakcije
├── LABsistem.Frontend/         # React SPA klijentska aplikacija
│   ├── src/                    # Izvorni kod (main.jsx, App.jsx, komponente)
│   ├── package.json            # npm skripte i zavisnosti
│   └── nginx.conf              # Konfiguracija za produkcijski web server
├── LABsistem.Tests/            # .NET projekat za unit i integracione testove
└── LABsistem.E2E/              # End-to-End testovi (Playwright)

---
### Entry Points - Tačka ulaza u aplikaciju

- **Backend** (Program.cs) : Inicijalizuje WebApplication.CreateBuilder, učitava .env konfiguracije, registruje servise u DI kontejner, konfiguriše HTTP pipeline (middleware) i pokreće aplikaciju preko app.Run()
- **Frontend** (src/main.jsx) : Pokreće React aplikaciju, montira klijentski ruter i renderuje glavnu <App/> komponentu u DOM.

---

## 4. Slojevita Arhitektura i Zavisnosti Modula

Unutar backend rješenja implementirana je Slojevita (Layered) Arhitektura sa jednosmjernim zavisnostima usmjerenim ka dole (od prezentacije prema domenu):

[Presentation] ──> [Api] ──> [Dal] ──> [Domain]
      │             │         │
      └─────────────┴─────────┘ (Projektne reference)
      


1. **LABsistem.Presentation**: Izložen prema spoljnom svijetu (HTTP Kontroleri). Zavisi od Api, Dal i Domain slojeva radi registracije i usmjeravanja.
2. **LABsistem.Api**: Sadrži čistu poslovnu (biznis) logiku. Zavisi od Dal (za podatke) i Domain (za strukture).
3. **LabSistem.Dal**: Upravlja perzistencijom podataka kroz LabSistemDbContext. Zavisi isključivo od Domain sloja.
4.  **LabSistem.Domain**: Srž sistema. Potpuno izolovana cjelina koja sadrži čiste C# klase (entitete), enume i interfejse (apstrakcije).

 ---

### Poslovne komponente i Analiza Implementacije

Sistem je podijeljen na ključne biznis domene koji se registruju kroz Dependency Injection (DI) u Program.cs:
- Auth, Oprema, Rezervacija, Termin, Kabinet, Obavijesti, Evidencija, Zahtjevi.


## 5. Tok Podataka (Data Flow) i Sigurnost

#### **Standardni životni ciklus zahtjeva (Request Lifecycle)**

Kada korisnik izvrši neku akciju na interfejsu, podaci teku kroz sljedeće korake:
1. **Klijent**: Korisnik klikne na akciju (npr. Login). React komponenta aktivira Axios klijent (src/api/client.js) i šalje POST zahtjev na /Auth/login.
2. **Kontroler**: AuthController presreće HTTP zahtjev, vrši osnovnu validaciju i prosljeđuje DTO (Data Transfer Object) sloju poslovne logike.
3.  **Servis**: AuthService izvršava provjere, poredi heširane lozinke pomoću BCrypt i koristi JwtService za generisanje tokena.
4.  **Perzistencija**: Podaci se po potrebi čitaju/upisuju u bazu podataka preko LabSistemDbContext mapiranih DbSet<> kolekcija.

#### Autentifikacija i Autorizacija


Sistem implementira striktan **RBAC (Role-Based Access Control)** model zasnovan na JWT (JSON Web Tokens) standardu.

- **Backend Sigurnost**:
  
  - U Program.cs konfigurisan je JWT middleware sa validacijom izdavaoca (Issuer), publike (Audience), tajnog ključa (SigningKey) i životnog vijeka tokena (Lifetime).
  - Uveden je mehanizam za provjeru opozvanih tokena (revoked-token check)
  -  Generisani tokeni u sebi nose claimove: NameIdentifier, Role i Jti (jedinstveni identifikator tokena).
  -  Zaštita resursa se vrši na nivou kontrolera i akcija pomoću deklarativnih atributa, npr: [Authorize(Roles = "Admin,Tehnicar")].
    
- **Frontend Sigurnost:**
  
   - Nakon uspješne prijave, Access Token se čuva u localStorage (za autorizaciju trenutnih zahtjeva), dok se podaci o osvježavanju sesije nalaze u sessionStorage.
   - Axios klijent posjeduje interceptore koji automatski prilažu Authorization: Bearer <token> zaglavlje i prate 419/401 statuse kako bi izvršili auto-refresh tokena ili preusmjerili korisnika nazad na Login stranicu u slučaju isteka sesije.


 ---

 ## 6. Razvoj, Testiranje i Deployment
 
#### Okruženja i Konfiguracija

Sistem podržava dinamičku konfiguraciju u zavisnosti od okruženja (Development / Production):

- Konfiguracija se vuče iz appsettings.json, appsettings.Development.json, varijabli okruženja (Env Vars) i lokalnih .env fajlova koji se programski učitavaju na startu u Program.cs.

- Lokalno pokretanje je olakšano kroz predefinisane launch profile unutar launchSettings.json (Development i Docker profili).


#### Strategija Testiranja

Projekat implementira višeslojnu strategiju testiranja koja pokriva sve aspekte aplikacije:

- **Backend Unit/Integracioni Testovi (LABsistem.Tests)**: Izgrađeni pomoću xUnit frameworka i Moq biblioteke za mockovanje zavisnosti. Za testiranje baza podataka bez stvarne instance koristi se EF InMemory provajder uz ASP.NET test host.

- **End-to-End (E2E) Testovi**: Primarni E2E alat je Playwright lociran u zasebnom LABsistem.E2E paketu koji simulira stvarne akcije korisnika u browseru. Pored toga, frontend sadrži i zaostalu/alternativnu Cypress konfiguraciju (cypress.config.js).


#### CI/CD i Produkcijski Deployment

Kompletan životni ciklus aplikacije automatizovan je kroz GitHub Actions:

- **CI (build-script.yml)**: Prilikom svakog Push-a ili Pull Request-a, pokreće se pipeline koji instalira zavisnosti (preko npm za frontend i dotnet restore za backend), radi kompajliranje koda (dotnet build) i izvršava sve automatske testove (dotnet test).

- **CD (deployment-script.yml)**: Prilikom slanja koda na produkcijsku granu, GitHub Action izvršava sljedeće korake:

1. Gradi produkcijske Docker image-e za backend i frontend (koji se u produkciji servira optimizovano iza Nginx reverse proxy-ja).

2. Kopira konfiguraciju i image-e na ciljani produkcijski server.
3. Pokreće docker-compose up -d --build.
4. Orkestrator čeka da baza podataka prođe healthcheck i automatski izvršava najnovije EF Core migracije nad PostgreSQL bazom prije nego što pusti saobraćaj ka aplikaciji.
