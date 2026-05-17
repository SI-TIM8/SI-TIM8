# Sprint Backlog – Sprint 6

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a| Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|
| 1 | Korisnici CRUD – frontend, backend i baza uključujući deaktivaciju i aktivaciju | US01, US02, US35, US36 | Hamza Hadžić, Merima Glušac | Završeno | Implementirano upravljanje korisnicima |
| 2 | Oprema CRUD – frontend; Status opreme – backend logika | US06, US07, US08 | Aner Atović, Haris Sadiković | Završeno | Upravljanje opremom i statusima |
| 3 | Radno vrijeme laboratorije; Kabineti + blokiranje perioda; Termini – definisanje, izmjena i brisanje | US17, US18, US19, US20, US22, US23 | Refik Mujčinović, Haris Macić | Završeno | Implementirano upravljanje terminima i kabinetima |
| 4 | Integracijski testovi za sve CRUD operacije | US01, US02, US06, US07, US08, US17, US18, US19, US20, US22, US23, US35, US36 | Emina Hamamdžić, Alma Jusufbegović | Završeno | Testirane CRUD funkcionalnosti |

---

# Detaljni User Stories (US)

---

### US01 – Kreiranje korisnika

*Kao administrator, želim kreirati korisnike, kako bi se mogli prijaviti na sistem.*

**Acceptance Criteria:**

* Administrator ima pristup formi za unos korisnika.
* Obavezna polja se validiraju.
* Novi korisnik se uspješno sprema u bazu.
* Korisnik dobija odgovarajuću ulogu.

---

### US02 – Deaktivacija i aktivacija korisnika

*Kao administrator, želim deaktivirati i ponovo aktivirati korisnike, kako bih održavao ažurnu bazu korisnika.*

**Acceptance Criteria:**

* Administrator vidi listu svih korisnika.
* Dostupna je opcija deaktivacije i aktivacije.
* Deaktivirani korisnik se ne može prijaviti.
* Podaci korisnika ostaju sačuvani.

---

### US35 – Uređivanje korisnika

*Kao administrator, želim uređivati korisničke podatke, kako bih održavao tačne informacije u sistemu.*

**Acceptance Criteria:**

* Administrator može otvoriti postojeći korisnički nalog.
* Prikazuju se trenutni podaci korisnika.
* Moguće je izmijeniti osnovne podatke i ulogu.
* Promjene se uspješno spremaju u bazu.

---

### US36 – Pretraga i filtriranje korisnika

*Kao administrator, želim pretraživati i filtrirati korisnike, kako bih lakše upravljao korisničkim nalozima.*

**Acceptance Criteria:**

* Moguće je pretraživanje po imenu, emailu ili korisničkom imenu.
* Dostupni su filteri po ulozi i statusu.
* Više filtera može biti aktivno istovremeno.
* Postoji opcija resetovanja filtera.

---

### US06 – Kreiranje nove opreme

*Kao laboratorijski tehničar, želim dodati novu opremu u sistem, kako bi bila dostupna za rezervaciju.*

**Acceptance Criteria:**

* Dostupna je forma za unos opreme.
* Validiraju se obavezni podaci.
* Oprema se uspješno dodaje u bazu.
* Oprema je odmah vidljiva u sistemu.

---

### US07 – Brisanje opreme

*Kao laboratorijski tehničar, želim ukloniti opremu koja se više ne koristi.*

**Acceptance Criteria:**

* Prikazana je lista opreme.
* Dostupna je opcija brisanja.
* Korisnik potvrđuje akciju brisanja.
* Oprema se uklanja iz sistema.

---

### US08 – Uređivanje statusa opreme

*Kao laboratorijski tehničar, želim promijeniti status opreme, kako bi podaci bili ažurni.*

**Acceptance Criteria:**

* Prikazuje se trenutni status opreme.
* Status se može izmijeniti.
* Promjene se odmah reflektuju u sistemu.

---

### US17 – Kreiranje radnog vremena

*Kao administrator, želim definisati radno vrijeme laboratorije.*

**Acceptance Criteria:**

* Administrator može definisati radno vrijeme.
* Validiraju se uneseni podaci.
* Radno vrijeme je vidljivo korisnicima.

---

### US18 – Definisanje termina

*Kao laboratorijski tehničar, želim definisati termine korištenja laboratorije.*

**Acceptance Criteria:**

* Dostupna je forma za kreiranje termina.
* Termin sadrži trajanje i povezanu opremu.
* Novi termin je odmah vidljiv korisnicima.

---

### US19 – Mijenjanje termina

*Kao laboratorijski tehničar, želim izmijeniti postojeće termine.*

**Acceptance Criteria:**

* Postojeći termin se može otvoriti za izmjene.
* Moguće je izmijeniti sve podatke termina.
* Izmjene se odmah prikazuju u sistemu.

---

### US20 – Brisanje termina

*Kao laboratorijski tehničar, želim ukloniti termin iz sistema.*

**Acceptance Criteria:**

* Termin se može obrisati.
* Promjena se odmah reflektuje u kalendaru.

---

### US22 – Dodavanje i uređivanje kabineta

*Kao administrator, želim upravljati kabinetima, kako bih organizovao resurse laboratorije.*

**Acceptance Criteria:**

* Administrator može dodati novi kabinet.
* Administrator može uređivati postojeće kabinete.
* Dostupna je lista svih kabineta.

---

### US23 – Blokiranje korištenja kabineta

*Kao administrator, želim blokirati određene periode u kabinetu, kako bi se spriječile rezervacije tokom neradnih termina.*

**Acceptance Criteria:**

* Administrator može blokirati vremenski period.
* Rezervacije nisu moguće u blokiranom periodu.
* Blokirani period je vizuelno označen.

---