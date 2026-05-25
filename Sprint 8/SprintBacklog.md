# Sprint Backlog – Sprint 8

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a | Opis | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|-|
| 1 | Oporavak lozinke putem emaila | Implementiran je kompletan tok za zaboravljenu lozinku, uključujući slanje reset linka na email, validaciju tokena i postavljanje nove lozinke kroz sigurnu formu. | US33 | Aner Atović, Hamza Hadžić | Završeno | - |
| 2 | In-app i email notifikacije o statusu zahtjeva | Student nakon obrade zahtjeva dobija obavijest unutar aplikacije i email sa statusom zahtjeva, dok profesor ili asistent može ostaviti komentar koji se prikazuje studentu uz odobrenje ili odbijanje. | US27 | Emina Hamamdžić, Alma Jusufbegović | Završeno | Obavijesti studentima i osoblju |
| 3 | Uvođenje novih tipova opreme | Sistem je proširen novom klasifikacijom opreme kroz tip ili kategoriju, uz podršku za unos, prikaz i filtriranje tog podatka u inventaru. | US34 | Haris Macić, Refik Mujčinović | Završeno | Automatsko zauzimanje i oslobađanje opreme |
| 4 | E2E i integracijsko testiranje rezervacijskog toka | Testiran je kompletan workflow rezervacija kroz više slojeva sistema, uključujući slanje zahtjeva, obradu zahtjeva, slanje obavijesti i provjeru da se sve promjene ispravno čuvaju i prikazuju. | US11, US15, US26, US27 | Merima Glušac, Haris Sadiković | Završeno | Testiranje kompletnog workflowa rezervacija |

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

### US37 – Upravljanje tipovima opreme

*Kao tehničar, želim dodavati, uređivati i brisati tipove opreme kako bi sistem sadržavao ažurne kategorije laboratorijske opreme.*

**Acceptance Criteria:**

* Tehničar može dodati novi tip opreme.
* Tehničar  može izmijeniti postojeći tip opreme.
* Tehničar  može obrisati tip opreme koji nije u upotrebi.
* Prikazuje se lista svih tipova opreme.
* Sistem validira da naziv tipa opreme nije prazan.
* Nakon izmjena, lista tipova opreme se automatski ažurira.

---

### US26 – Automatska validacija konflikta termina i opreme

*Kao sistem, želim provjeriti da li su termin i resursi i dalje dostupni prije potvrde rezervacije, kako bi se spriječile konfliktne ili duple rezervacije.*

**Acceptance Criteria:**

* Sistem prije potvrde rezervacije provjerava da li je termin još uvijek dostupan.
* Sistem ne dozvoljava potvrdu rezervacije ako je termin u međuvremenu zauzet.
* Korisnik dobija jasnu poruku kada rezervacija nije moguća zbog konflikta.
* Validacija se izvršava u sklopu rezervacijskog toka prije konačne potvrde zahtjeva.

---
