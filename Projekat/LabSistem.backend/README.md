# LABsistem Backend API

**Verzija:** 1.0  
**Framework:** .NET 10.0  
**Baza podataka:** PostgreSQL

---

## Pokretanje Projekta

### 1. Provjera Preduvjeta

Trebaju ti:

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download)
- **PostgreSQL** - [Download](https://www.postgresql.org/download/)
- **git** (opciono)

### 2. Konfiguracija Baze Podataka

Kreiraj PostgreSQL bazu sa ovim kredencijaljima (ili ih promijeni u `appsettings.json`):

```
Host: localhost
Port: 5432
Database: labsistem
Username: labsistem
Password: labsistem
```

Ili sa psql komandom:

```sql
CREATE DATABASE labsistem;
CREATE USER labsistem WITH PASSWORD 'labsistem';
GRANT ALL PRIVILEGES ON DATABASE labsistem TO labsistem;
```

### 3. Pokretanje Backend-a

```bash
# Poziconiraj se u LABsistem.Presentation direktorij
cd Projekat/LabSistem.backend/LABsistem.Presentation

# Obavimo migraciju baze podataka (automatski se izvršava pri pokretanju)
dotnet run

# Ili ako želiš da pokreneš sa debuggingom
dotnet run --configuration Debug
```

Backend će biti dostupan na: **http://localhost:8080**

---

## API Dokumentacija

### 📚 Swagger UI

Otvori u pregledniku:  
**`http://localhost:8080/swagger`**

Ovdje možeš:

- ✅ Vidjeti sve dostupne endpointe
- ✅ Testirati API pozive direktno iz preglednika
- ✅ Vidjeti primjere zahtjeva i odgovora
- ✅ Koristiti JWT token za zaštićene endpointe

### 📖 Markdown Dokumentacija

Detaljna markdown dokumentacija je dostupna u:  
**`/docs/API.md`**

Dokumentacija sadrži:

- Sve endpointe grupirane po resursu
- Request/Response primjere
- Status kodove
- Primjere iz frontend-a (JavaScript)

---

## Testiranje API Endpointa

### 1. Health Check (bez autentifikacije)

```bash
curl http://localhost:8080/api/test/health
```

Odgovora:

```json
{
  "status": "healthy",
  "message": "LABsistem API je dostupan",
  "timestamp": "2026-04-28T12:30:00Z",
  "version": "v1.0"
}
```

### 2. Login i Dobijanje JWT Tokena

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'
```

Odgovora (primjer):

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "admin",
  "role": "Admin"
}
```

### 3. Zaštićeni Endpoint (sa JWT tokenom)

```bash
# Zamijeni <TOKEN> sa pravim JWT tokenom dobijenim iz login-a
curl -X GET http://localhost:8080/api/test/secure-test \
  -H "Authorization: Bearer <TOKEN>"
```

### 4. Korišćenje Swagger UI za Testiranje

1. Otvori **http://localhost:8080/swagger**
2. Klikni na **"Authorize"** dugme gore desno
3. Unesi JWT token (samo sam token, bez "Bearer ")
4. Klikni **"Authorize"**
5. Sada možeš testirati sve zaštićene endpointe

---

## Struktura Projekta

```
LabSistem.backend/
├── LABsistem.Presentation/          # Web API projekt (Program.cs, Kontroleri)
│   ├── Controllers/                 # API kontroleri (8 resursa)
│   ├── Program.cs                   # Startup konfiguracija
│   ├── appsettings.json            # Konfiguracija (DB, JWT)
│   └── LABsistem.Presentation.csproj
│
├── LABsistem.Api/                   # Business Logic (DTOs, Servisi)
│   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Auth/                    # Login, Register DTOs
│   │   ├── Korisnik/                # Korisnik DTOs
│   │   ├── Oprema/                  # Oprema DTOs
│   │   ├── Kabinet/                 # Kabinet DTOs
│   │   ├── Termin/                  # Termin DTOs
│   │   ├── Zahtjev/                 # Zahtjev DTOs
│   │   ├── Obavijest/               # Obavijest DTOs
│   │   ├── Evidencija/              # Evidencija DTOs
│   │   ├── Recenzija/               # Recenzija DTOs
│   │   └── Objekat/                 # Objekat DTOs
│   │
│   ├── Services/                    # Poslovni servis
│   │   ├── AuthService.cs           # Auth logika
│   │   ├── KorisnikService.cs       # TODO: Implementirati
│   │   ├── OpremaService.cs         # TODO: Implementirati
│   │   ├── TerminService.cs         # TODO: Implementirati
│   │   ├── ZahtjevService.cs        # TODO: Implementirati
│   │   ├── KabinetService.cs        # TODO: Implementirati
│   │   ├── ObavijestService.cs      # TODO: Implementirati
│   │   ├── EvidencijaService.cs     # TODO: Implementirati
│   │   └── RecenzijaService.cs      # TODO: Implementirati
│   │
│   └── Validators/                  # Validacijska pravila
│
├── LabSistem.Dal/                   # Data Access Layer
│   ├── Db/
│   │   ├── LabSistemDbContext.cs    # Entity Framework DbContext
│   │   └── Configurations/          # Entity konfiguracije
│   │
│   ├── Repositories/                # Repozitorij pattern
│   └── Migrations/                  # EF migracije
│
├── LabSistem.Domain/                # Domenske entitete
│   ├── Entities/                    # Sve entitete (Korisnik, Termin, itd.)
│   ├── Enums/                       # Enumeracije (Uloga, Status, itd.)
│   └── Abstractions/                # Interfejsi
│
├── docs/                            # Dokumentacija
│   └── API.md                       # Detaljni API katalog
│
└── docker-compose.yml               # Docker konfiguracija
```

---

## Konfiguracija (appsettings.json)

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=labsistem;Username=labsistem;Password=labsistem"
  },
  "Jwt": {
    "Key": "LABsistem_SuperSecretJwtKey_2026_ChangeInProduction",
    "Issuer": "LABsistem",
    "Audience": "LABsistem.Client",
    "ExpireMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### ⚠️ Važno: Promijeni tajne u produkciji!

Ne koristi default vrijednosti u produkciji:

- `Jwt.Key` - generiši novu nasumičnu vrijednost
- `ConnectionString` - koristi production bazu i kredencijale
- Koristi **environment variables** ili **Azure Key Vault** umjesto plain texta

---

## Autentifikacija i Autorizacija

### JWT Flow

1. **Korisnik se prijavljuje** → `POST /api/auth/login`
2. **Server vraća JWT token** sa information o korisniku
3. **Klijent sprema token** u `localStorage`
4. **Klijent šalje token** u `Authorization: Bearer <token>` headeru za sve zaštićene pozive
5. **Server validira token** pre nego što dozvoli pristup

### Uloge (Roles)

- **Admin** - Puni pristup, upravljanje sistemom
- **Profesor** - Upravljanje terminima, odobravanje zahtjeva
- **Tehnicar** - Upravljanje opremom i kabinetima
- **Student** - Kreiranje zahtjeva za rezervaciju

### Zaštita Ruta

```csharp
[Authorize]                           // Potreban bilo koji validan token
[Authorize(Roles = "Admin")]         // Potrebna je Admin uloga
[Authorize(Roles = "Profesor,Admin")] // Profesor ili Admin
[AllowAnonymous]                     // Javno dostupan endpoint
```

---

## Endpointi Pregled

### 🔐 Autentifikacija (`/api/auth`)

- `POST /login` - Prijava korisnika
- `POST /register` - Registracija studenta
- `POST /create-user` - Kreiranje korisnika (Admin)
- `GET /verify` - Verifikacija trenutnog tokena
- `POST /verify-token` - Verifikacija tokena iz body-ja

### 👥 Korisnici (`/api/korisnik`)

- `GET /` - Lista korisnika (Profesor, Admin)
- `GET /{id}` - Detalji korisnika
- `PUT /{id}` - Ažuriranje profila
- `POST /{id}/change-password` - Promjena lozinke
- `DELETE /{id}` - Brisanje korisnika (Admin)

### 🔧 Oprema (`/api/oprema`)

- `GET /` - Lista opreme
- `GET /{id}` - Detalji opreme
- `POST /` - Kreiranje opreme (Tehnicar, Admin)
- `PUT /{id}` - Ažuriranje opreme
- `POST /{id}/prijava-kvara` - Prijava kvara
- `DELETE /{id}` - Brisanje opreme

### 🏛️ Kabineti (`/api/kabinet`)

- `GET /` - Lista kabineta
- `GET /{id}` - Detalji kabineta
- `POST /` - Kreiranje kabineta (Admin)
- `PUT /{id}` - Ažuriranje kabineta
- `POST /{id}/block-period` - Blokiranje perioda
- `DELETE /{id}` - Brisanje kabineta

### ⏰ Termini (`/api/termin`)

- `GET /` - Lista termina
- `GET /{id}` - Detalji termina
- `POST /` - Kreiranje termina (Profesor, Tehnicar)
- `PUT /{id}` - Ažuriranje termina
- `DELETE /{id}` - Brisanje termina

### 📋 Zahtjevi (`/api/zahtjev`)

- `GET /` - Lista svih zahtjeva (Profesor, Admin)
- `GET /moji` - Moji zahtjevi
- `GET /{id}` - Detalji zahtjeva
- `POST /` - Kreiranje zahtjeva
- `PUT /{id}/approve` - Odobravanje zahtjeva
- `PUT /{id}/reject` - Odbijanje zahtjeva
- `PUT /{id}/cancel` - Otkazivanje zahtjeva

### 🔔 Obavijesti (`/api/obavijest`)

- `GET /` - Moje obavijesti
- `PUT /{id}/mark-as-read` - Označavanje kao pročitano
- `DELETE /{id}` - Brisanje obavijesti

### 📊 Evidencija (`/api/evidencija`)

- `GET /` - Audit log (Admin)
- `GET /user/{korisnikId}` - Evidencija korisnika

---

## Build i Deploy

### Lokalni Build

```bash
dotnet build
```

### Build za Release

```bash
dotnet publish -c Release -o ./publish
```

### Docker

```bash
# Build Docker image
docker build -f LABsistem.Presentation/Dockerfile -t labsistem-api .

# Pokreni kontejner
docker run -p 8080:8080 -e ConnectionStrings__Default="..." labsistem-api
```

---

## Česta Pitanja

### Q: Gdje se nalazi Swagger dokumentacija?

**A:** http://localhost:8080/swagger

### Q: Kako testiram zaštićene endpointe?

**A:** Koristi Swagger UI → klikni "Authorize" → unesi JWT token

### Q: Šta ako baza podataka ne postoji?

**A:** Migracija se izvršava automatski pri pokretanju (`context.Database.Migrate()` u Program.cs)

### Q: Kako promijenim JWT secret key?

**A:** Promijeni `Jwt.Key` u `appsettings.json`

### Q: Što trebam da uradim za produkciju?

**A:** Čitaj [⚠️ Važno: Promijeni tajne u produkciji!](#-važno-promijeni-tajne-u-produkciji) dio

---

## Status Implementacije

| Dio                  | Status     | Napomena                                |
| -------------------- | ---------- | --------------------------------------- |
| **Swagger/OpenAPI**  | ✅ Gotovo  | Dostupno na /swagger                    |
| **Test Endpointi**   | ✅ Gotovo  | /api/test/health, /api/test/secure-test |
| **Auth Kontroler**   | ✅ Gotovo  | Login, Register, Verify Token           |
| **Drugi Kontroleri** | 🟡 Skeleti | Struktura je gotova, logika trebame     |
| **DTOs**             | ✅ Gotovo  | Svi DTOs su kreirani                    |
| **Dokumentacija**    | ✅ Gotovo  | API.md sa svim endpointima              |
| **Servisi**          | 🟡 TODO    | AuthService je gotov, ostali trebame    |
| **Validacije**       | 🟡 TODO    | Trebaju se dodati validacijski atributi |

---

## Zadaci za Tim (M2 i ostali)

1. **Haris/M2** - Implementira logiku u **8 servisa** (Korisnik, Oprema, Kabinet, itd.)
2. **Aner** - Implementira **validacije** u DTOs-ima
3. **Merima** - Kreira **unit testove** za servise
4. **Emina** - Implementira **RBAC logiku** u servisima
5. **Hamza** - Implementira **logout logiku** i **session timeout**

---

## Kontakt i Podrška

- **API Dokumentacija:** `/docs/API.md`
- **Swagger UI:** `http://localhost:8080/swagger`
- **Sprint 5 Zadatak:** API setup i dokumentacija - **GOTOVO** ✅

---

**Zadnja ažuriranja:** 28.04.2026.  
**Autor:** Refik Mujčinović (API Setup & Documentation)
