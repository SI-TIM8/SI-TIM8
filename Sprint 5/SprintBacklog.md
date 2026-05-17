# Sprint Backlog – Sprint 5

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a| Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|
| 1 | Login UI + validacija forme (frontend) | US03 | Aner Atović | Završeno | Implementirana validacija login forme i prikaz poruka greške |
| 2 | JWT generisanje i verifikacija (backend) | US29 | Haris Sadiković | Završeno | Implementiran JWT auth mehanizam |
| 3 | RBAC logika – uloge i zaštita ruta | US30 | Emina Hamamdžić | Završeno | Zaštićene rute prema ulozi korisnika |
| 4 | Sigurna odjava + redirect na login stranicu | US31 | Hamza Hadžić | Završeno | Implementirano uništavanje sesije i redirect |
| 5 | Automatski istek sesije | US32 | Alma Jusufbegović | Završeno | Timeout neaktivne sesije |
| 6 | Testiranje auth toka (unit + integration testovi) | US03, US29, US30, US31, US32 | Merima Glušac | Završeno | Pokriveni ključni auth scenariji |
| 7 | CI/CD pipeline + Docker konfiguracija | - | Haris Macić | Završeno | Dockerizacija i CI/CD pipeline |
| 8 | API setup i dokumentacija endpointa | US03, US29 | Refik Mujčinović | Završeno | Dokumentovani auth endpointi |

---

# Detaljni User Stories (US)

---

### US03 – Prijava na sistem

*Kao registrovani korisnik, želim da se prijavim na sistem, kako bih pristupio resursima.*

**Acceptance Criteria:**

* Korisnik može unijeti email i lozinku.
* Sistem validira unesene kredencijale.
* U slučaju uspješne prijave korisnik dobija pristup sistemu.
* U slučaju greške prikazuje se odgovarajuća poruka.

---

### US29 – Sigurna prijava sa tokenima

*Kao korisnik, želim da sistem generiše siguran JWT token prilikom prijave, kako bih ostao prijavljen bez ponovnog unosa lozinke.*

**Acceptance Criteria:**

* Sistem generiše JWT token nakon uspješne prijave.
* Token se koristi za autentifikaciju API zahtjeva.
* Token sadrži podatke o ulozi korisnika.
* Nevalidan ili istekao token vraća 401 Unauthorized.

---

### US30 – Ograničenje pristupa prema ulogama

*Kao administrator, želim da sistem prepozna korisničku ulogu, kako bi svaki korisnik imao pristup samo dozvoljenim funkcionalnostima.*

**Acceptance Criteria:**

* Student nema pristup administratorskim funkcijama.
* Samo ovlaštene uloge mogu pristupiti CRUD operacijama.
* Neovlašten pristup vraća 403 Forbidden.
* UI prikazuje samo opcije dostupne trenutnoj ulozi.

---

### US31 – Sigurna odjava

*Kao korisnik, želim da se odjavim sa sistema, kako bih zaštitio svoj nalog na dijeljenim računarima.*

**Acceptance Criteria:**

* Dostupno je Logout dugme.
* JWT token se uklanja nakon odjave.
* Korisnik se preusmjerava na login stranicu.
* Zaštićene stranice nisu dostupne nakon odjave.

---

### US32 – Istek sesije

*Kao administrator, želim da sistem automatski odjavi korisnika nakon perioda neaktivnosti, kako bih povećao sigurnost sistema.*

**Acceptance Criteria:**

* Sesija ističe nakon definisanog perioda neaktivnosti.
* Korisnik dobija obavijest o isteku sesije.
* Nakon isteka potrebna je nova prijava.

---