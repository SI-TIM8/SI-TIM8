# Sprint Backlog – Sprint 8

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a| Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|
| 1 | Oporavak lozinke putem emaila  | US33 | Aner Atović, Haris Sadiković | Završeno  | -|
| 2 | In-app i email notifikacije o statusu zahtjeva     | US27 | Emina Hamamdžić, Alma Jusufbegović | Završeno  | Obavijesti studentima i osoblju |
| 3 | Uvođenje novih tipova opreme   | - | Hamza Hadžić, Merima Glušac | Završeno  | Automatsko zauzimanje i oslobađanje opreme |
| 4 | E2E i integracijsko testiranje rezervacijskog toka | US11, US15, US26, US27 | Refik Mujčinović, Haris Macić | Završeno  | Testiranje kompletnog workflowa rezervacija |




## **Cilj sprinta:** Validacija konfliktnih rezervacija i workflow odobravanja  

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US33 | Oporavak lozinke putem emaila | Korisnik resetuje zaboravljenu lozinku putem unikatnog privremenog linka poslanog na email | Feature | 3 | Medium | To Do |
| US27 | In-app/email notifikacija o zahtjevu | Student prima email ili in-app notifikaciju o odobrenju/odbijanju, uključujući komentar profesora/asistenta; Profesor prima notifikacije o pristiglom zahtjevu.| Feature | 3 | Medium | To Do |
| US28 | In-app notifikacija o zahtjevu | Profesor  dobija zahtjev od studenata za prihvatanje na termin ili odbijanje, prilikom odbijanja ili prihvatanja ima mogućnost komenatara.| Feature | 3 | Medium | To Do |
| US34 | Upravljanje tipovima opreme | Kao tehničar, želim dodavati, uređivati i brisati tipove opreme kako bi sistem sadržavao ažurne kategorije laboratorijske opreme.| Feature | 3 | Medium | To Do |



# Detaljni User Stories (US)

---
### US33 – Oporavak lozinke putem emaila

*Kao student/profesor, želim  resetovati svoju lozinku, kako bih mogla pristupiti sistemu.*

**Acceptance Criteria:**

* Korisnik može kliknuti na opciju „Zaboravljena lozinka“.
* Korisnik unosi registrovani email.
* Sistem šalje jedinstveni privremeni link za reset lozinke na email.
* Korisnik putem linka može unijeti novu lozinku i potvrdu lozinke.
* Sistem validira ispravnost nove lozinke i podudaranje potvrde.
* Nakon uspješnog resetovanja, korisnik dobija potvrdu o uspješnoj promjeni lozinke.

---



### US27 – Odobravanje zahtjeva

*Kao profesor ili asistent, želim odobriti ili odbiti rezervacijske zahtjeve.*

**Acceptance Criteria:**

* Prikazana je lista pending zahtjeva.
* Dostupne su opcije Odobri i Odbij.
* Moguće je ostaviti komentar prilikom odobravanja ili odbijanja zahtjeva.
* Status zahtjeva se ažurira u realnom vremenu.
* Student dobija email i in-app notifikaciju o statusu zahtjeva.
* Notifikacija sadrži informaciju da li je zahtjev prihvaćen ili odbijen.
* Ako je ostavljen komentar, student ga može vidjeti u notifikaciji/emailu.
---



### US28 – Obavještenje o odobrenju ili odbijanju zahtjeva

*Kao student, želim primiti notifikaciju o statusu zahtjeva, kako bih znao da li je rezervacija prihvaćena.*

**Acceptance Criteria:**

* Student prima email ili in-app notifikaciju.
* Notifikacija sadrži status zahtjeva.
* Komentar profesora ili asistenta je uključen u obavijest.
* Profesor dobija obavijest o novim zahtjevima.

---

### US34 – Upravljanje tipovima opreme

Kao tehničar, želim dodavati, uređivati i brisati tipove opreme kako bi sistem sadržavao ažurne kategorije laboratorijske opreme.

Acceptance Criteria:

* Tehničar može dodati novi tip opreme.
* Tehničar  može izmijeniti postojeći tip opreme.
* Tehničar  može obrisati tip opreme koji nije u upotrebi.
* Prikazuje se lista svih tipova opreme.
* Sistem validira da naziv tipa opreme nije prazan.
* Nakon izmjena, lista tipova opreme se automatski ažurira.

---
