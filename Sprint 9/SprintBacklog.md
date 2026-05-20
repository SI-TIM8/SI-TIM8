
#### Sprint 9 | TBD

**Cilj sprinta:** Prijava kvarova, filtriranje opreme i sistemsko testiranje 
**Kapacitet tima:** ~65%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US09 / US28 | Prijava kvara + automatsko otkazivanje budućih rezervacija | Profesor prijavljuje kvar opreme; sistem mijenja status na "neispravna" i otkazuje sve Pending/Approved rezervacije; pogođeni korisnici primaju obavijest | Feature | 3+3 | Medium | To Do |
| US16 | Ograničenje broja aktivnih rezervacija po studentu | Profesor postavlja limit; sistem prikazuje upozorenje kada se limit dostigne | Feature | 2 | Medium | To Do |
| – | Sistemsko i performansno testiranje | E2E tok s kvarom opreme; load testiranje sa 50 concurrent korisnika; provjera ACID transakcija i backup mehanizma | Testing | 4 | High | To Do |



| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Forma za prijavu kvara – frontend (US09/US28) |
| M2 | Dodavanje općenitih obavještenja za sve korisnike |
| M3-M4 | Ograničenje broja rezervacija po studentu (US16)|
| M5 | E2E testiranje- ostatak sistema|
| M6 |Load testiranje – 50 concurrent korisnika (NFR-13); ACID i backup provjera (NFR-17, NFR-19) |
| M7-M8 | Regresijsko testiranje svih funkcionalnosti do kraja S9 |


---

#### Sprint 10 | TBD

**Cilj sprinta:** Rješavanje problema kvarova, ocjenjivanje studenata
**Kapacitet tima:** ~85%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US44 | Rješavanje kvarova opreme  | Nakon forme za kvar opreme koja je poslata od strane profesora/asistenta, tehničar riješava  problem, dolazi mu na mail obavijest o kvaru, i stavlja je u to do sekciju| 2 | Low | To Do |
| US45| Nakon obavijesti da je došlo do kvara opreme, u sekciji to do se riješava problem | Tehničar  riješava problem u sekciji To-Do, tako što isključuje datu opremu iz sistema privremeno ili odmah je popravi | Feature | 3 | Medium | To Do |
| US29 | Ocjena studentima | Profesor  daje ocjene studentima, može ih brisati dodavati ili uređivati| Feature | 2 | Medium | To Do |



| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Prosljeđivanje problema u ( To-Do)|
| M2 | Rješavanje problema |
| M3 | Ocjenjivanje studenata |
| M4 |Unit testiranje |
