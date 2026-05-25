# Proof of Testing – Sprint 9

Ovaj izvještaj dokumentuje validaciju sistema pod visokim opterećenjem i simulacijom stvarnih korisnika. Fokus testiranja je provjera **NFR-13 (Non-Functional Requirement)**: stabilnost sistema i odziv API-ja prilikom konkurentnog pristupa 50 korisnika istovremeno.

Također dokumentuje validaciju transakcijskog integriteta, izolacije konkurentnih zahtjeva i trajnosti zapisa unutar Modula za Rezervacije i Termine na integracijskom testnom okruženju.

Fokus testiranja je verifikacija **ACID svojstava baze podataka (Atomicity, Isolation, Durability)** kroz automatizovane integracijske testove (AcidTests.cs).

---

## 1. Load Testiranje (k6 Performance Engine)

**Fokus:** Evaluacija ponašanja backend servisa pod stresom, mjerenje brzine odziva ključnih endpointa (/api/Rezervacija/dostupni-studentima, /api/Rezervacija/zahtjev, /api/Oprema), identifikacija potencijalnih uskih grla (bottlenecks) i stabilnost baze podataka pri konkurentnim upisima.

---

### Profil Opterećenja (Load Profile & Stages)
Test je dizajniran da simulira realan scenario na fakultetu/laboratoriji (npr. momenat kada se otvara prijava za termine).

| Faza | Trajanje | Ciljani broj VU (Virtual Users) | Opis |
| :--- | :--- | :--- | :--- |
| **Ramp-Up** | 30 sekundi | 0 ➔ 25 | Postepeno podizanje opterećenja i zagrijavanje sistema |
| **Steady-State (Peak)** | 2 minute | 50 VU | Glavni test (NFR-13): Održavanje maksimalnog vršnog opterećenja |
| **Ramp-Down** | 30 sekundi | 50 ➔ 0 | Postepeno gašenje virtuelnih korisnika i smirivanje sistema |


---

### Verifikacija Testnih Scenarija (Mješavina Saobraćaja - Traffic Mix)
Unutar glavnog scenarija simulirano je ponašanje dvije ključne uloge u omjeru *70% studenti / 30% profesori*:

| Uloga / Akcija | Tip HTTP zahtjeva | Endpoint | Očekivano ponašanje pod opterećenjem |
| :--- | :--- | :--- | :--- |
| **Student (70%)** | GET | `/api/Rezervacija/dostupni-studentima` | Odziv unutar definisanog praga, uspješan dohvat |
| **Student (70%)** | POST | `/api/Rezervacija/zahtjev/{id}` | Konkurentne prijave na termine. Status 200/201 ili 409 (ako je već prijavljen) |
| **Profesor (30%)** | GET | `/api/Oprema` | Pregled opreme bez uticaja na studentske akcije |

---

### Tehnologije i metodologija

- **Grafana k6 (v1.x)** - izuzetno lagan, CLI baziran alat visokih performansi napisan u Go-u, izabran zbog minimalne potrošnje lokalnih resursa (CPU/RAM).
- **Lokalno izolovani Seed Podaci** - umjesto dinamičkog kreiranja korisnika tokom samog testa (što stvara overhead), korišteno je 50 unaprijed pripremljenih testnih studenata (loadstudent1 - loadstudent50)
- **Dinamičko planiranje termina** Setup faza kreira 10 svježih termina unaprijed pomjerenih u budućnost kako bi se izbjegli hardkodirani vremenski konflikti u bazi

---

### Rezultati Testiranja

| Metrika | Zahtjev (NFR) | Rezultat | Status |
| :--- | :--- | :--- |:--- |
| **Vršno opterećenje** | 50 konkurentnih korisnika | 50 VU održano tokom 2 min | ✅PROŠLO |
| **Vrijeme odziva (p95)** | http_req_duration < 1000ms | 420ms (95% zahtjeva završeno ispod 0.42s) | ✅100% PASS |
| **Procenat grešaka** | rate < 0.01 (manje od 1%) | 0.00% (Sve greške tipa 5xx su izbjegnute) | ✅100% PASS |
| **Rukovanje konfliktima** | Tolerancija na logičke greške | Status 409 Conflict ispravno prepoznat kao biznis pravilo, a ne sistemska greška |✅USPIJEŠNO|

---

### Ključni business rules i edge scenariji

- Eliminacija preklapanja termina
- Zaključavanje resursa (Concurrency)
- Notifikacije korisnicima u svim edge-cases (odobrenje/odbijanje/otkazivanje/kvar)
- Ne može doći do prekoračenja limita ili kasnog otkazivanja (pravilo 24h unaprijed)
- Svi root-case scenariji su pokriveni od strane QA i dev tima

---

### Validacija spremnosti sistema

✅ **SVE PERFORMANSNE METRIKE USPJEŠNO VALIDIRANE** — Modul rezervacija pokazao je izuzetnu stabilnost i brzinu odziva pod definisanim vršnim opterećenjem od 50 konkurentnih korisnika. Aplikacija u potpunosti zadovoljava definisane ne-funkcionalne zahtjeve (NFR-13).

**Datum:** 2026-05-19 | **Sprint:** Sprint 9 | **Status:** ✅ PERFORMANCE READY

---

## 2. Transakcijska Pouzdanost (ACID Integration Core)

**Fokus:** Osiguravanje da aplikacija i baza podataka ispravno reaguju na nevalidne unose (Atomicity), sprečavanje korupcije podataka i preklapanja resursa prilikom istovremenih klikova (Isolation), te garancija da jednom upisan zahtjev ostaje trajno zapisan u sistemu (Durability).

---

### Verifikacija Testnih Scenarija (ACID Matrix)

| Klasifikacija (ACID) | Testni Scenario | Metoda / Endpoint | Očekivano Ponašanje | Status |
| :--- | :--- | :--- | :--- |:--- |
| **ATOMICITY** | Prijava na nepostojeći termin | `POST /api/Rezervacija/zahtjev/99999` | Sistem odbija zahtjev sa 404 NotFound ili 400 BadRequest. Nema djelomičnih ili fantomskih upisa | ✅ |
| **ATOMICITY** | Kreiranje termina s nevalidnim podacima | `POST /api/Termin` | Kraj termina prije početka + nepostojeći kabinetID: 99999 kaskadno odbija cijelu transakciju sa 400 BadRequest | ✅ |
| **ISOLATION** | Dva studenta konkurentno na isti termin | `POST /api/Rezervacija/zahtjev/1` | Race Condition Validation: Istovremeni zahtjevi ne smiju uzrokovati 500 Internal Error. Jedan prolazi, drugi dobija kontrolisani 409 Conflict/BadRequest | ✅ |
| **ISOLATION** | Preklapanje termina u isto vrijeme i kabinet | `POST /api/Termin` | Dva tehničara istovremeno unose isti slot. Sistem kroz unutrašnje zaključavanje (Locking) propušta maksimalno 1 uspješan unos | ✅ |
| **ATOMICITY** | Perzistencija end-to-end toka rezervacije | `POST ➔ GET (Više uloga)` | Nakon što tehničar kreira termin, profesor otvori rezervaciju, a student pošalje zahtjev — podaci uspješno prolaze kroz faze i ostaju trajno čitljivi | ✅ |

---

### Tehnologije i metodologija

- **xUnit Test Framework** - Korišten za izolaciju, strukturiranje i izvršavanje integracijskih assertation-a nad aktivnim API klijentom
- **Konkurentno Asinhrono Izvršavanje (Task.WhenAll)** - Korišteno unutar Isolation faza kako bi se simulirao stvarni mrežni nalet i simultani klikovi (mili-sekundni razmaci) preko odvojenih instanci HttpClient-a sa zasebnim JWT autorizacijskim tokenima
- **End-to-End State Machine** Durability test dinamički simulira kompletan poslovni proces kroz tri različite uloge (Tehničar ➔ Profesor ➔ Student), provjeravajući perzistenciju na svakom koraku preko realnih pretraga baze podataka

---

### Validacija spremnosti sistema

✅ **SVA ACID SVOJSTVA SISTEMA USPJEŠNO VALIDIRANA** — Modul rezervacije i pripadajući kontroleri demonstriraju potpunu transakcijsku sigurnost, otpornost na konkurentne sukobe (Race Conditions) i konzistentno upravljanje stanjima entiteta unutar baze podataka.

**Datum:** 2026-05-19 | **Sprint:** Sprint 9 | **Status:** ✅ ACID & TRANSACTION SIGNED-OFF

---

