# Sprint Backlog – Sprint 8

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a| Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|
| 1 | Automatska validacija konflikta termina i opreme   | US26 | Aner Atović, Haris Sadiković | Završeno  | Sprječavanje duplih rezervacija |
| 2 | In-app i email notifikacije o statusu zahtjeva     | US27 | Emina Hamamdžić, Alma Jusufbegović | Završeno  | Obavijesti studentima i osoblju |
| 3 | Automatska promjena statusa opreme po rezervaciji  | - | Hamza Hadžić, Merima Glušac | Završeno  | Automatsko zauzimanje i oslobađanje opreme |
| 4 | E2E i integracijsko testiranje rezervacijskog toka | US11, US15, US26, US27 | Refik Mujčinović, Haris Macić | Završeno  | Testiranje kompletnog workflowa rezervacija |

---

# Detaljni User Stories (US)

---

### US26 – Sprječavanje preklapanja termina

*Kao sistem, želim spriječiti potvrdu rezervacije ukoliko postoji konflikt termina ili zauzeta oprema.*

**Acceptance Criteria:**

* Sistem provjerava dostupnost prije potvrde rezervacije.
* Duple rezervacije nisu moguće.
* Korisnik dobija poruku ako je termin zauzet.
* Validacija se izvršava u realnom vremenu.

---

### US27 – Obavještenje o odobrenju ili odbijanju zahtjeva

*Kao student, želim primiti notifikaciju o statusu zahtjeva, kako bih znao da li je rezervacija prihvaćena.*

**Acceptance Criteria:**

* Student prima email ili in-app notifikaciju.
* Notifikacija sadrži status zahtjeva.
* Komentar profesora ili asistenta je uključen u obavijest.
* Profesor dobija obavijest o novim zahtjevima.

---