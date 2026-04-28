# LABsistem API Dokumentacija

**Verzija:** 1.0  
**Base URL (Development):** `http://localhost:8080`  
**Base URL (Docker):** `http://labsistem.api:8080`  
**Swagger UI:** `http://localhost:8080/swagger`

---

## Sadržaj

1. [Autentifikacija](#autentifikacija)
2. [Korisnici](#korisnici)
3. [Kabineti](#kabineti)
4. [Oprema](#oprema)
5. [Termini](#termini)
6. [Zahtjevi / Rezervacije](#zahtjevi--rezervacije)
7. [Obavijesti](#obavijesti)
8. [Evidencija](#evidencija)
9. [Kodovi Greške](#kodovi-greške)

---

## Autentifikacija

### JWT Bearer Token

Svi zaštićeni endpointi zahtijevaju JWT token u `Authorization` headeru:

```http
Authorization: Bearer <token>
```

Token se dobija nakon uspješne prijave i vrijedi prema konfiguraciji (DefaultValue: 60 minuta).

---

## Endpointi

### **AUTENTIFIKACIJA**

#### 1. Login korisnika

```http
POST /api/auth/login
```

**Opis:** Prijava korisnika i generisanje JWT tokena

**Auth:** ❌ Nije potrebna

**Request Body:**

```json
{
  "username": "student123",
  "password": "lozinka123"
}
```

**Response (200):**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "student123",
  "role": "Student"
}
```

**Status kodovi:**

- `200 OK` - Uspješna prijava
- `400 Bad Request` - Nedostaju username ili password
- `401 Unauthorized` - Pogrešni kredencijali

---

#### 2. Registracija novog korisnika (Student)

```http
POST /api/auth/register
```

**Opis:** Registracija novog studentskog naloga

**Auth:** ❌ Nije potrebna

**Request Body:**

```json
{
  "imePrezime": "Ajla Kovač",
  "email": "ajla.kovac@uni.ba",
  "username": "ajla123",
  "password": "sigurnaLozinka123"
}
```

**Response (200):**

```json
{
  "message": "Korisnik je uspješno registrovan"
}
```

**Status kodovi:**

- `200 OK` - Uspješna registracija
- `400 Bad Request` - Neispravni podaci ili korisnik već postoji
- `422 Unprocessable Entity` - Lozinka ne zadovoljava kriterije

---

#### 3. Kreiranje korisnika (Admin)

```http
POST /api/auth/create-user
```

**Opis:** Administrator kreira novog korisnika (Profesor ili Tehničar)

**Auth:** ✅ Potrebna - Admin uloga

**Query Parameters:**

- `uloga` (obavezno) - `Profesor` ili `Tehnicar`

**Request Body:**

```json
{
  "imePrezime": "Dr. Samir Subašić",
  "email": "s.subasic@uni.ba",
  "username": "prof_samir",
  "password": "sigurnaLozinka123"
}
```

**Response (200):**

```json
{
  "message": "Korisnik je uspješno kreiran kao Profesor"
}
```

**Status kodovi:**

- `200 OK` - Korisnik uspješno kreiran
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola (nije Admin)

---

#### 4. Verifikacija trenutnog korisnika

```http
GET /api/auth/verify
```

**Opis:** Provjera da li je JWT token validan i dohvatanje podataka o trenutnom korisniku

**Auth:** ✅ Potrebna - Bilo koja uloga

**Response (200):**

```json
{
  "valid": true,
  "userId": "1",
  "username": "student123",
  "role": "Student"
}
```

**Status kodovi:**

- `200 OK` - Token je validan
- `401 Unauthorized` - Token nije validan ili je istekao

---

#### 5. Verifikacija tokena (string token u body-ju)

```http
POST /api/auth/verify-token
```

**Opis:** Verifikacija JWT tokena koji se šalje u body-ju (ne u headeru)

**Auth:** ❌ Nije potrebna

**Request Body:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (200):**

```json
{
  "valid": true,
  "userId": "1",
  "username": "student123",
  "role": "Student"
}
```

**Response (401):**

```json
{
  "valid": false
}
```

**Status kodovi:**

- `200 OK` - Token je validan
- `401 Unauthorized` - Token nije validan

---

### **KORISNICI**

#### 1. Lista svih korisnika

```http
GET /api/korisnici
```

**Opis:** Dohvatanje liste svih korisnika sa mogućnošću filtriranja i paginacije

**Auth:** ✅ Potrebna - Admin ili Profesor

**Query Parameters (opciono):**

- `search` - Pretraga po imenu ili emailu
- `role` - Filter po ulozi (Admin, Profesor, Student, Tehnicar)
- `page` - Broj stranice (defaultValue: 1)
- `pageSize` - Broj stavki po stranici (defaultValue: 10)

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "imePrezime": "Ajla Kovač",
      "email": "ajla.kovac@uni.ba",
      "username": "ajla123",
      "uloga": "Student"
    }
  ],
  "totalCount": 47,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista korisnika
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola

---

#### 2. Detalji korisnika

```http
GET /api/korisnici/{id}
```

**Opis:** Dohvatanje detalja specificnog korisnika

**Auth:** ✅ Potrebna - Korisnik sam ili Admin

**Response (200):**

```json
{
  "id": 1,
  "imePrezime": "Ajla Kovač",
  "email": "ajla.kovac@uni.ba",
  "username": "ajla123",
  "uloga": "Student",
  "datumKreiranja": "2026-01-15T10:30:00Z"
}
```

**Status kodovi:**

- `200 OK` - Detalji korisnika
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Korisnik ne postoji

---

#### 3. Ažuriranje profila

```http
PUT /api/korisnici/{id}
```

**Opis:** Ažuriranje korisničkog profila (ime, email)

**Auth:** ✅ Potrebna - Korisnik sam ili Admin

**Request Body:**

```json
{
  "imePrezime": "Ajla Kovač-Nova",
  "email": "ajla.nova@uni.ba"
}
```

**Response (200):**

```json
{
  "message": "Profil je uspješno ažuriran"
}
```

**Status kodovi:**

- `200 OK` - Profil ažuriran
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Korisnik ne postoji

---

#### 4. Promjena lozinke

```http
POST /api/korisnici/{id}/change-password
```

**Opis:** Promjena lozinke korisnika

**Auth:** ✅ Potrebna - Korisnik sam ili Admin

**Request Body:**

```json
{
  "oldPassword": "stara_lozinka123",
  "newPassword": "nova_sigurna_lozinka456"
}
```

**Response (200):**

```json
{
  "message": "Lozinka je uspješno promijenjena"
}
```

**Status kodovi:**

- `200 OK` - Lozinka promijenjena
- `400 Bad Request` - Stara lozinka nije ispravna ili nova lozinka ne zadovoljava kriterije
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Korisnik ne postoji

---

#### 5. Brisanje korisnika

```http
DELETE /api/korisnici/{id}
```

**Opis:** Brisanje korisnika (samo admin)

**Auth:** ✅ Potrebna - Admin uloga

**Response (200):**

```json
{
  "message": "Korisnik je uspješno obrisan"
}
```

**Status kodovi:**

- `200 OK` - Korisnik obrisan
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola (samo admin)
- `404 Not Found` - Korisnik ne postoji

---

### **KABINETI**

#### 1. Lista kabineta

```http
GET /api/kabineti
```

**Opis:** Dohvatanje liste svih kabineta (laboratorija)

**Auth:** ✅ Potrebna - Bilo koja uloga

**Query Parameters (opciono):**

- `objektId` - Filter po objektu
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "naziv": "Kabinet 101",
      "objektId": 1,
      "objektNaziv": "Elektrotehnika",
      "kapacitet": 20,
      "odgovorniKorisnikId": 5,
      "odgovorniKorisnikIme": "Dr. Samir Subašić"
    }
  ],
  "totalCount": 12,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista kabineta
- `401 Unauthorized` - Nedostaje JWT token

---

#### 2. Detalji kabineta

```http
GET /api/kabineti/{id}
```

**Opis:** Dohvatanje detalja specificnog kabineta

**Auth:** ✅ Potrebna - Bilo koja uloga

**Response (200):**

```json
{
  "id": 1,
  "naziv": "Kabinet 101",
  "objektId": 1,
  "kapacitet": 20,
  "radnoVrijeme": "08:00 - 17:00",
  "odgovorniKorisnik": {
    "id": 5,
    "imePrezime": "Dr. Samir Subašić"
  },
  "opremaUKabinetu": 8
}
```

**Status kodovi:**

- `200 OK` - Detalji kabineta
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Kabinet ne postoji

---

#### 3. Kreiranje kabineta

```http
POST /api/kabineti
```

**Opis:** Kreiranje novog kabineta (samo admin)

**Auth:** ✅ Potrebna - Admin uloga

**Request Body:**

```json
{
  "naziv": "Kabinet 102",
  "objektId": 1,
  "kapacitet": 25,
  "odgovorniKorisnikId": 5
}
```

**Response (201):**

```json
{
  "id": 2,
  "naziv": "Kabinet 102",
  "message": "Kabinet je uspješno kreiran"
}
```

**Status kodovi:**

- `201 Created` - Kabinet kreiran
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola

---

#### 4. Ažuriranje kabineta

```http
PUT /api/kabineti/{id}
```

**Opis:** Ažuriranje detalja kabineta

**Auth:** ✅ Potrebna - Admin uloga

**Request Body:**

```json
{
  "naziv": "Kabinet 102 - Updated",
  "kapacitet": 30,
  "odgovorniKorisnikId": 6
}
```

**Response (200):**

```json
{
  "message": "Kabinet je uspješno ažuriran"
}
```

**Status kodovi:**

- `200 OK` - Kabinet ažuriran
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Kabinet ne postoji

---

#### 5. Brisanje kabineta

```http
DELETE /api/kabineti/{id}
```

**Opis:** Brisanje kabineta (samo admin)

**Auth:** ✅ Potrebna - Admin uloga

**Response (200):**

```json
{
  "message": "Kabinet je uspješno obrisan"
}
```

**Status kodovi:**

- `200 OK` - Kabinet obrisan
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Kabinet ne postoji

---

#### 6. Blokiranje perioda u kabinetu

```http
POST /api/kabineti/{id}/block-period
```

**Opis:** Blokiranje vremenskog perioda u kabinetu (npr. praznici, čišćenje)

**Auth:** ✅ Potrebna - Admin uloga

**Request Body:**

```json
{
  "datumPocetak": "2026-04-28",
  "datumKraj": "2026-04-30",
  "razlog": "Godišnji odmor"
}
```

**Response (200):**

```json
{
  "message": "Period je uspješno blokiran"
}
```

**Status kodovi:**

- `200 OK` - Period blokiran
- `400 Bad Request` - Neispravni datumi
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola

---

### **OPREMA**

#### 1. Lista opreme

```http
GET /api/oprema
```

**Opis:** Dohvatanje liste opreme sa mogućnošću filtriranja

**Auth:** ✅ Potrebna - Bilo koja uloga

**Query Parameters (opciono):**

- `kabinetId` - Filter po kabinetu
- `status` - Filter po statusu (Ispravno, UKvaru, NaServisu, Otpisano)
- `search` - Pretraga po nazivu
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "naziv": "Mikroskop XR-200",
      "serijskiBroj": "SN-4421",
      "kabinetId": 1,
      "status": "Ispravno",
      "kreiranDatum": "2026-01-10T14:20:00Z"
    }
  ],
  "totalCount": 38,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista opreme
- `401 Unauthorized` - Nedostaje JWT token

---

#### 2. Detalji opreme

```http
GET /api/oprema/{id}
```

**Opis:** Dohvatanje detalja specifične opreme

**Auth:** ✅ Potrebna - Bilo koja uloga

**Response (200):**

```json
{
  "id": 1,
  "naziv": "Mikroskop XR-200",
  "serijskiBroj": "SN-4421",
  "kabinetId": 1,
  "kabinetNaziv": "Kabinet 101",
  "status": "Ispravno",
  "kreiranOd": {
    "id": 2,
    "imePrezime": "Zlatan Mujić"
  },
  "kreiranDatum": "2026-01-10T14:20:00Z"
}
```

**Status kodovi:**

- `200 OK` - Detalji opreme
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Oprema ne postoji

---

#### 3. Kreiranje opreme

```http
POST /api/oprema
```

**Opis:** Kreiranje nove opreme (samo Tehničar ili Admin)

**Auth:** ✅ Potrebna - Tehnicar ili Admin uloga

**Request Body:**

```json
{
  "naziv": "Centrifuga C-10",
  "serijskiBroj": "SN-8834",
  "kabinetId": 1,
  "status": "Ispravno"
}
```

**Response (201):**

```json
{
  "id": 2,
  "naziv": "Centrifuga C-10",
  "message": "Oprema je uspješno kreiirana"
}
```

**Status kodovi:**

- `201 Created` - Oprema kreiirana
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola

---

#### 4. Ažuriranje statusa opreme

```http
PUT /api/oprema/{id}
```

**Opis:** Ažuriranje detalja i statusa opreme

**Auth:** ✅ Potrebna - Tehnicar ili Admin uloga

**Request Body:**

```json
{
  "naziv": "Centrifuga C-10 (Updated)",
  "status": "UKvaru"
}
```

**Response (200):**

```json
{
  "message": "Oprema je uspješno ažurirana"
}
```

**Status kodovi:**

- `200 OK` - Oprema ažurirana
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Oprema ne postoji

---

#### 5. Prijava kvara

```http
POST /api/oprema/{id}/prijava-kvara
```

**Opis:** Prijava kvara opreme i automatska promjena statusa

**Auth:** ✅ Potrebna - Bilo koja uloga

**Request Body:**

```json
{
  "opis": "Mikroskop ne fokusira leće pravilno",
  "hitnost": "Srednja"
}
```

**Response (200):**

```json
{
  "message": "Kvar je uspješno prijavljen",
  "statusOpreme": "UKvaru",
  "zahtjevId": 15
}
```

**Status kodovi:**

- `200 OK` - Kvar prijavljen
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Oprema ne postoji

---

#### 6. Brisanje opreme

```http
DELETE /api/oprema/{id}
```

**Opis:** Brisanje opreme iz sistema

**Auth:** ✅ Potrebna - Tehnicar ili Admin uloga

**Response (200):**

```json
{
  "message": "Oprema je uspješno obrisana"
}
```

**Status kodovi:**

- `200 OK` - Oprema obrisana
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Oprema ne postoji

---

### **TERMINI**

#### 1. Lista termina

```http
GET /api/termini
```

**Opis:** Dohvatanje liste termina sa mogućnošću filtriranja

**Auth:** ✅ Potrebna - Bilo koja uloga

**Query Parameters (opciono):**

- `kabinetId` - Filter po kabinetu
- `datumOd` - Početni datum (YYYY-MM-DD)
- `datumDo` - Završni datum
- `slobodni` - true/false - samo slobodni termini
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "kabinetId": 1,
      "kabinetNaziv": "Kabinet 101",
      "datum": "2026-05-01",
      "vrijemePocetka": "10:00",
      "vremeKraja": "12:00",
      "kreiranOd": "prof_samir",
      "status": "Slobodan"
    }
  ],
  "totalCount": 24,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista termina
- `401 Unauthorized` - Nedostaje JWT token

---

#### 2. Detalji termina

```http
GET /api/termini/{id}
```

**Opis:** Dohvatanje detalja specificnog termina

**Auth:** ✅ Potrebna - Bilo koja uloga

**Response (200):**

```json
{
  "id": 1,
  "kabinetId": 1,
  "kabinetNaziv": "Kabinet 101",
  "datum": "2026-05-01",
  "vrijemePocetka": "10:00",
  "vremeKraja": "12:00",
  "kapacitet": 20,
  "zauzetoMjesta": 0,
  "kreiranOd": {
    "id": 5,
    "imePrezime": "Dr. Samir Subašić"
  },
  "kreiranDatum": "2026-04-20T09:15:00Z"
}
```

**Status kodovi:**

- `200 OK` - Detalji termina
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Termin ne postoji

---

#### 3. Kreiranje termina

```http
POST /api/termini
```

**Opis:** Kreiranje novog termina (samo Profesor ili Tehničar)

**Auth:** ✅ Potrebna - Profesor ili Tehnicar uloga

**Request Body:**

```json
{
  "kabinetId": 1,
  "datum": "2026-05-05",
  "vrijemePocetka": "14:00",
  "vremeKraja": "16:00",
  "kapacitet": 20
}
```

**Response (201):**

```json
{
  "id": 5,
  "message": "Termin je uspješno kreiran"
}
```

**Status kodovi:**

- `201 Created` - Termin kreiran
- `400 Bad Request` - Neispravni podaci ili termin je van radnog vremena
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `409 Conflict` - Preklapanje sa postojećim terminom

---

#### 4. Ažuriranje termina

```http
PUT /api/termini/{id}
```

**Opis:** Ažuriranje vremena i detalja termina

**Auth:** ✅ Potrebna - Kreirator termina ili Admin

**Request Body:**

```json
{
  "datum": "2026-05-06",
  "vrijemePocetka": "15:00",
  "vremeKraja": "17:00",
  "kapacitet": 25
}
```

**Response (200):**

```json
{
  "message": "Termin je uspješno ažuriran"
}
```

**Status kodovi:**

- `200 OK` - Termin ažuriran
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Termin ne postoji
- `409 Conflict` - Preklapanje sa drugim terminom

---

#### 5. Brisanje termina

```http
DELETE /api/termini/{id}
```

**Opis:** Brisanje termina (samo kreirator ili admin)

**Auth:** ✅ Potrebna - Kreirator ili Admin

**Response (200):**

```json
{
  "message": "Termin je uspješno obrisan"
}
```

**Status kodovi:**

- `200 OK` - Termin obrisan
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Termin ne postoji

---

### **ZAHTJEVI / REZERVACIJE**

#### 1. Lista zahtjeva (za trenutnog korisnika)

```http
GET /api/zahtjevi/moji
```

**Opis:** Dohvatanje liste vlastitih zahtjeva/rezervacija

**Auth:** ✅ Potrebna - Bilo koja uloga

**Query Parameters (opciono):**

- `status` - Filter po statusu (NaCekanju, Odobren, Odbijen, Storniran)
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "terminId": 1,
      "kabinetNaziv": "Kabinet 101",
      "datum": "2026-05-01",
      "vrijeme": "10:00 - 12:00",
      "status": "Odobren",
      "komentar": "Potvrđeno",
      "kreiranDatum": "2026-04-25T10:30:00Z"
    }
  ],
  "totalCount": 3,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista zahtjeva
- `401 Unauthorized` - Nedostaje JWT token

---

#### 2. Lista svih zahtjeva (za Profesore/Admin)

```http
GET /api/zahtjevi
```

**Opis:** Dohvatanje liste svih zahtjeva u sistemu

**Auth:** ✅ Potrebna - Profesor ili Admin uloga

**Query Parameters (opciono):**

- `status` - Filter po statusu
- `kabinetId` - Filter po kabinetu
- `datum` - Filter po datumu
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "korisnik": "ajla123",
      "korisnikIme": "Ajla Kovač",
      "terminId": 1,
      "kabinetNaziv": "Kabinet 101",
      "datum": "2026-05-01",
      "vrijeme": "10:00 - 12:00",
      "status": "NaCekanju",
      "komentar": null,
      "kreiranDatum": "2026-04-25T10:30:00Z"
    }
  ],
  "totalCount": 12,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista zahtjeva
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola

---

#### 3. Kreiranje zahtjeva (Student podnosi zahtjev)

```http
POST /api/zahtjevi
```

**Opis:** Kreiranje zahtjeva za rezervaciju termina

**Auth:** ✅ Potrebna - Bilo koja uloga

**Request Body:**

```json
{
  "terminId": 1,
  "opremaSveZaTermin": true,
  "odabranaOpremaIds": [1, 3, 5],
  "napomena": "Trebam za seminarsku vježbu"
}
```

**Response (201):**

```json
{
  "id": 5,
  "status": "NaCekanju",
  "message": "Zahtjev je uspješno podnesen na odobrenje"
}
```

**Status kodovi:**

- `201 Created` - Zahtjev podnesen
- `400 Bad Request` - Neispravni podaci
- `401 Unauthorized` - Nedostaje JWT token
- `409 Conflict` - Termin je već zauzet ili student je dostigao limit

---

#### 4. Odobravanje zahtjeva

```http
PUT /api/zahtjevi/{id}/approve
```

**Opis:** Odobravanje zahtjeva (samo Profesor ili Admin)

**Auth:** ✅ Potrebna - Profesor ili Admin uloga

**Request Body:**

```json
{
  "komentar": "Odobreno - vidjeti se u navedenom vremenu"
}
```

**Response (200):**

```json
{
  "message": "Zahtjev je uspješno odobren",
  "obavijestIdPoslana": 1
}
```

**Status kodovi:**

- `200 OK` - Zahtjev odobren
- `400 Bad Request` - Zahtjev nije u statusu "Na čekanju"
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Zahtjev ne postoji

---

#### 5. Odbijanje zahtjeva

```http
PUT /api/zahtjevi/{id}/reject
```

**Opis:** Odbijanje zahtjeva (samo Profesor ili Admin)

**Auth:** ✅ Potrebna - Profesor ili Admin uloga

**Request Body:**

```json
{
  "komentar": "Kapacitet termina je već popunjen"
}
```

**Response (200):**

```json
{
  "message": "Zahtjev je odbijen",
  "obavijestIdPoslana": 2
}
```

**Status kodovi:**

- `200 OK` - Zahtjev odbijen
- `400 Bad Request` - Zahtjev nije u statusu "Na čekanju"
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Zahtjev ne postoji

---

#### 6. Otkazivanje zahtjeva

```http
PUT /api/zahtjevi/{id}/cancel
```

**Opis:** Otkazivanje odobrenog zahtjeva (Student ili Admin)

**Auth:** ✅ Potrebna - Korisnik sam ili Admin

**Request Body:**

```json
{
  "razlog": "Bolestan sam"
}
```

**Response (200):**

```json
{
  "message": "Zahtjev je otkazan",
  "obavijestIdPoslana": 3
}
```

**Status kodovi:**

- `200 OK` - Zahtjev otkazan
- `400 Bad Request` - Zahtjev nije u statusu "Odobren"
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola
- `404 Not Found` - Zahtjev ne postoji

---

### **OBAVIJESTI**

#### 1. Lista obavijesti korisnika

```http
GET /api/obavijesti
```

**Opis:** Dohvatanje liste obavijesti za trenutnog korisnika

**Auth:** ✅ Potrebna - Bilo koja uloga

**Query Parameters (opciono):**

- `pročitano` - true/false - filter po statusu čitanja
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "novosti": "Vaš zahtjev je odobren",
      "dostupnost": true,
      "procitano": false,
      "datumSlanja": "2026-04-26T14:30:00Z",
      "tipObavijesti": "ZahtjevOdobren"
    }
  ],
  "totalCount": 5,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Lista obavijesti
- `401 Unauthorized` - Nedostaje JWT token

---

#### 2. Označavanje obavijesti kao pročitane

```http
PUT /api/obavijesti/{id}/mark-as-read
```

**Opis:** Označavanje obavijesti kao pročitane

**Auth:** ✅ Potrebna - Bilo koja uloga

**Response (200):**

```json
{
  "message": "Obavijest je označena kao pročitana"
}
```

**Status kodovi:**

- `200 OK` - Obavijest označena
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Obavijest ne postoji

---

#### 3. Brisanje obavijesti

```http
DELETE /api/obavijesti/{id}
```

**Opis:** Brisanje obavijesti

**Auth:** ✅ Potrebna - Bilo koja uloga

**Response (200):**

```json
{
  "message": "Obavijest je obrisana"
}
```

**Status kodovi:**

- `200 OK` - Obavijest obrisana
- `401 Unauthorized` - Nedostaje JWT token
- `404 Not Found` - Obavijest ne postoji

---

### **EVIDENCIJA**

#### 1. Audit log - Istorija aktivnosti

```http
GET /api/evidencija
```

**Opis:** Dohvatanje log-a svih aktivnosti u sistemu (samo Admin)

**Auth:** ✅ Potrebna - Admin uloga

**Query Parameters (opciono):**

- `korisnikId` - Filter po korisniku
- `datumOd` - Početni datum
- `datumDo` - Završni datum
- `akcija` - Filter po akciji (Login, CreateTermin, ApproveZahtjev, itd.)
- `page` - Broj stranice
- `pageSize` - Broj stavki po stranici

**Response (200):**

```json
{
  "data": [
    {
      "id": 1,
      "korisnikId": 1,
      "korisnikUsername": "ajla123",
      "akcija": "Login",
      "opis": "Korisnik se prijavljivao",
      "resursDatum": "2026-04-27T08:30:00Z",
      "ipAdresa": "192.168.1.100"
    }
  ],
  "totalCount": 156,
  "pageNumber": 1,
  "pageSize": 10
}
```

**Status kodovi:**

- `200 OK` - Audit log
- `401 Unauthorized` - Nedostaje JWT token
- `403 Forbidden` - Nedovoljna dozvola (samo admin)

---

## Kodovi Greške

### Uobičajeni HTTP Status kodovi

| Kod   | Naziv                 | Opis                            |
| ----- | --------------------- | ------------------------------- |
| `200` | OK                    | Uspješan zahtjev                |
| `201` | Created               | Resurs je kreiran               |
| `204` | No Content            | Uspješan zahtjev bez sadržaja   |
| `400` | Bad Request           | Neispravni zahtjev              |
| `401` | Unauthorized          | Autentifikacija je obavezna     |
| `403` | Forbidden             | Pristup odbijen                 |
| `404` | Not Found             | Resurs nije pronađen            |
| `409` | Conflict              | Konflikt sa postojećim podacima |
| `422` | Unprocessable Entity  | Validacijska greška             |
| `500` | Internal Server Error | Greška na serveru               |

### Primjer greške

```json
{
  "errors": {
    "password": ["Lozinka mora biti duža od 8 karaktera"]
  },
  "message": "Validacijska greška",
  "statusCode": 400
}
```

---

## Primjeri Poziva iz Frontend-a (JavaScript)

### Login

```javascript
const response = await api.post("/auth/login", {
  username: "student123",
  password: "lozinka123",
});

const { token, userId, username, role } = response.data;
localStorage.setItem("token", token);
```

### Dohvati termine za kabinet

```javascript
const response = await api.get("/termini", {
  params: {
    kabinetId: 1,
    datumOd: "2026-05-01",
    slobodni: true,
  },
});

const termini = response.data.data;
```

### Kreiraj zahtjev za rezervaciju

```javascript
const response = await api.post("/zahtjevi", {
  terminId: 5,
  odabranaOpremaIds: [1, 3, 5],
  napomena: "Trebam za laboratorijsku vježbu",
});
```

---

## Napomene

- Svi vremeni su u UTC formatu
- Datumi se koriste u YYYY-MM-DD formatu
- Svi endpointi osim `/api/auth/login`, `/api/auth/register`, `/api/auth/verify-token` zahtijevaju JWT token
- JWT token se prosljeđuje kroz `Authorization: Bearer <token>` header
- Token ističe nakon vremenske periode definisane u konfiguraciji (DefaultValue: 60 minuta)

---

**Zadnja ažuriranja:** 28.04.2026.  
**Autor dokumentacije:** Tim za API
