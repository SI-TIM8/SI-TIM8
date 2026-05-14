# Sprint Backlog – Sprint 7

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a| Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|
| 1 | Kalendar UI – prikaz i color-coding | US24 | Hamza Hadžić, Merima Glušac | Završeno | Implementiran kalendarski prikaz termina |
| 2 | Lista slobodnih termina | US21 | Emina Hamamdžić | Završeno | Backend logika za dostupne termine |
| 3 | Rezervacije CRUD – frontend i backend | US11, US25 | Refik Mujčinović, Haris Macić | Završeno | Kreiranje i upravljanje rezervacijama |
| 4 | Odobravanje/odbijanje zahtjeva i pregled vlastitih zahtjeva | US12, US15 | Emina Hamamdžić, Alma Jusufbegović | Završeno | Workflow za odobravanje zahtjeva |
| 5 | Testiranje operacija rezervacija | US11, US12, US15, US21, US24, US25 | Aner Atović, Haris Sadiković | Završeno | Testirani rezervacijski tokovi |

---

# Detaljni User Stories (US)

---

### US21 – Pregled slobodnih termina

*Kao student, želim pregledati slobodne termine, kako bih mogao planirati rezervacije.*

**Acceptance Criteria:**

* Prikazana je lista dostupnih termina.
* Korisnik vidi samo slobodne termine.
* Podaci se ažuriraju u realnom vremenu.

---

### US24 – Pregled termina putem kalendara

*Kao student, želim vidjeti termine u formi kalendara, kako bih imao pregled rasporeda laboratorije.*

**Acceptance Criteria:**

* Dostupan je kalendarski prikaz.
* Termini su color-coded prema statusu.
* Moguće je filtriranje po kabinetu.

---

### US11 – Kreiranje rezervacije

*Kao student, želim rezervisati termin i opremu, kako bih mogao koristiti laboratorijske resurse.*

**Acceptance Criteria:**

* Prikazuju se samo slobodni termini i dostupna oprema.
* Sistem sprječava rezervacije u prošlosti.
* Sistem sprječava preklapanje rezervacija.
* Zahtjev se šalje na odobrenje.

---

### US25 – Odabir opreme unutar rezervacije

*Kao student, želim odabrati specifičnu opremu prilikom rezervacije.*

**Acceptance Criteria:**

* Lista opreme zavisi od odabranog kabineta.
* Korisnik može odabrati više stavki opreme.
* Sistem evidentira povezanu opremu uz rezervaciju.

---

### US12 – Pregled vlastitih zahtjeva

*Kao student, želim pregledati svoje zahtjeve i rezervacije.*

**Acceptance Criteria:**

* Prikazuje se lista korisnikovih zahtjeva.
* Vidljiv je status svakog zahtjeva.
* Korisnik može pregledati detalje rezervacije.

---

### US15 – Odobravanje zahtjeva

*Kao profesor ili asistent, želim odobriti ili odbiti rezervacijske zahtjeve.*

**Acceptance Criteria:**

* Prikazana je lista pending zahtjeva.
* Dostupne su opcije Approve i Reject.
* Moguće je ostaviti komentar.
* Status se ažurira u realnom vremenu.

---