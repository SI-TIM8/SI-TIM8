# LABsistem API Dokumentacija

## Pregled

Ovaj dokument opisuje trenutno implementirane API rute u projektu LABsistem.  
Dokument je uskladen sa stvarnim kontrolerima i servisima koji postoje u repozitoriju u ovom trenutku.

**Osnovni URL:** `http://localhost:8080`  
**Verzija:** 1.0  
**Status:** Trenutno implementirani API

---

## Sadrzaj

1. [Autentifikacija](#autentifikacija)
2. [Pregled ruta](#pregled-ruta)
3. [Detaljna dokumentacija](#detaljna-dokumentacija)
4. [Trenutno stanje projekta](#trenutno-stanje-projekta)
5. [Napomene](#napomene)

---

## Autentifikacija

API koristi JWT Bearer autentifikaciju.

Zasticeni endpointi zahtijevaju header:

```http
Authorization: Bearer <token>
```

Token se dobija nakon uspjesne prijave na endpointu za login.

---

## Pregled ruta

### Trenutno implementirane rute

| Metoda | Ruta                     | Opis                                     | Autentifikacija |
| ------ | ------------------------ | ---------------------------------------- | --------------- |
| POST   | `/api/auth/login`        | Prijava korisnika                        | Ne              |
| POST   | `/api/auth/register`     | Registracija studenta                    | Ne              |
| POST   | `/api/auth/create-user`  | Kreiranje korisnika od strane admina     | Da              |
| GET    | `/api/auth/verify`       | Provjera trenutno prijavljenog korisnika | Da              |
| POST   | `/api/auth/verify-token` | Verifikacija tokena iz body-ja           | Ne              |

---

## Detaljna dokumentacija

### 1. Prijava korisnika

#### Ruta

```http
POST /api/auth/login
```

#### Opis

Prijavljuje korisnika pomocu `username` i `password`, te vraca JWT token i osnovne korisnicke informacije.

#### Request body

```json
{
  "username": "student123",
  "password": "lozinka123"
}
```

#### Polja

| Polje      | Tip    | Obavezno | Opis           |
| ---------- | ------ | -------- | -------------- |
| `username` | string | Da       | Korisnicko ime |
| `password` | string | Da       | Lozinka        |

#### Success response

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": 1,
  "username": "student123",
  "role": "Student"
}
```

#### Status kodovi

- `200 OK` - Uspjesna prijava
- `401 Unauthorized` - Neispravni kredencijali
- `400 Bad Request` - Neispravan zahtjev

---

### 2. Registracija novog korisnika

#### Ruta

```http
POST /api/auth/register
```

#### Opis

Registruje novog korisnika kao `Student`.

#### Request body

```json
{
  "imePrezime": "Ajla Kovac",
  "email": "ajla.kovac@uni.ba",
  "username": "ajla123",
  "password": "sigurnaLozinka123"
}
```

#### Polja

| Polje        | Tip    | Obavezno | Opis                    |
| ------------ | ------ | -------- | ----------------------- |
| `imePrezime` | string | Da       | Ime i prezime korisnika |
| `email`      | string | Da       | Email adresa            |
| `username`   | string | Da       | Korisnicko ime          |
| `password`   | string | Da       | Lozinka                 |

#### Pravila za lozinku

Prema trenutnoj implementaciji lozinka mora imati:

- najmanje 8 znakova
- barem jedno veliko slovo
- barem jedan broj
- barem jedan specijalni znak

#### Success response

```json
{
  "message": "Registracija uspjesna."
}
```

#### Error responses

**400 Bad Request**

```json
{
  "message": "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak."
}
```

```json
{
  "message": "Username je vec zauzet."
}
```

#### Status kodovi

- `200 OK` - Uspjesna registracija
- `400 Bad Request` - Neispravni podaci ili username vec postoji

---

### 3. Kreiranje korisnika od strane administratora

#### Ruta

```http
POST /api/auth/create-user
```

#### Opis

Administrator kreira novog korisnika i dodjeljuje mu ulogu `Profesor` ili `Tehnicar`.

#### Autentifikacija

Potrebna je JWT autentifikacija i uloga `Admin`.

#### Query parametar

| Parametar | Tip           | Obavezno | Opis                    |
| --------- | ------------- | -------- | ----------------------- |
| `uloga`   | string / enum | Da       | Ciljana uloga korisnika |

Dozvoljene vrijednosti:

- `Profesor`
- `Tehnicar`

#### Request body

```json
{
  "imePrezime": "Dr. Samir Subasic",
  "email": "s.subasic@uni.ba",
  "username": "prof_samir",
  "password": "sigurnaLozinka123"
}
```

#### Success response

```json
{
  "message": "Korisnik uspjesno kreiran."
}
```

#### Error responses

**400 Bad Request**

```json
{
  "message": "Admin moze kreirati samo Profesore ili Tehnicare."
}
```

```json
{
  "message": "Lozinka mora imati najmanje 8 znakova, jedno veliko slovo, jedan broj i jedan specijalni znak."
}
```

```json
{
  "message": "Username je vec zauzet."
}
```

#### Status kodovi

- `200 OK` - Korisnik uspjesno kreiran
- `400 Bad Request` - Neispravni podaci ili nedozvoljena uloga
- `401 Unauthorized` - Nije poslan validan token
- `403 Forbidden` - Korisnik nema ulogu Admin

---

### 4. Verifikacija trenutnog korisnika

#### Ruta

```http
GET /api/auth/verify
```

#### Opis

Provjerava da li je trenutno poslani JWT token validan i vraca osnovne informacije o prijavljenom korisniku.

#### Autentifikacija

Da, potrebna je JWT autentifikacija.

#### Success response

```json
{
  "valid": true,
  "userId": "1",
  "username": "student123",
  "role": "Student"
}
```

#### Status kodovi

- `200 OK` - Token je validan
- `401 Unauthorized` - Token nije validan ili nedostaje

---

### 5. Verifikacija tokena iz body-ja

#### Ruta

```http
POST /api/auth/verify-token
```

#### Opis

Provjerava validnost tokena koji se salje u request body-ju.

#### Request body

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

#### Polja

| Polje   | Tip    | Obavezno | Opis                         |
| ------- | ------ | -------- | ---------------------------- |
| `token` | string | Da       | JWT token koji se provjerava |

#### Success response

```json
{
  "valid": true,
  "userId": "1",
  "username": "student123",
  "role": "Student"
}
```

#### Neuspjesan odgovor

```json
{
  "valid": false
}
```

#### Status kodovi

- `200 OK` - Token je validan
- `401 Unauthorized` - Token nije validan

---

## Trenutno stanje projekta

### Trenutno postoje

- `AuthController` sa implementiranim autentifikacijskim rutama
- `AuthService` sa logikom za login, registraciju, kreiranje korisnika i validaciju tokena
- DTO klase za auth:
  - `LoginRequestDto`
  - `LoginResponseDto`
  - `RegisterRequestDto`
  - `VerifyTokenRequestDto`

### Trenutno nisu implementirane API rute

Sljedeci kontroleri postoje u projektu, ali trenutno nemaju stvarne API endpoint rute, nego MVC skeletne akcije:

- `KorisnikController`
- `KabinetController`
- `OpremaController`
- `TerminController`
- `ZahtjevController`
- `ObavijestController`
- `EvidencijaController`
- `RecenzijaController`
- `ObjekatController`

Zbog toga se oni ne dokumentuju kao aktivni API endpointi dok se ne implementiraju.

---

## Napomene

- Login koristi `username` i `password`, ne email.
- Registracija trenutno kreira korisnika sa ulogom `Student`.
- Admin kreiranje korisnika je ograniceno na uloge `Profesor` i `Tehnicar`.
- Dokumentacija je uskladena sa trenutnim stanjem koda, bez dodavanja novih endpointa.
- Ako se kasnije implementiraju novi API kontroleri, ovaj dokument se moze prosiriti.

---

**Zadnje azuriranje:** 29.04.2026.
