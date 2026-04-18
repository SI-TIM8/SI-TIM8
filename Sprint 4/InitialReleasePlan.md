# INITIAL RELEASE PLAN

**Sistem za upravljanje laboratorijskom opremom**  
Tim 8 – ETF Sarajevo | Sprint 3 – April 2026

---

## 1. Uvod i svrha dokumenta

Ovaj dokument predstavlja Inicijalni Plan Objavljivanja (Initial Release Plan) sistema za upravljanje laboratorijskom opremom, koji razvija Tim 8 u okviru kolegija na ETF-u Sarajevo. Dokument definira raspored svih planiranih releasea, mapira user storyje i stavke product backloga na konkretne sprintove, te opisuje šta svaki inkrement sistema donosi kao upotrebljivu vrijednost krajnjim korisnicima.

Plan je zasnovan na svim dokumentima kreiranim tijekom Sprintova 1–3: Product Backlog (44 stavke), User Stories (US01–US34), Nefunkcionalni zahtjevi (NFR-01–NFR-20), Architecture Overview, Domain Model, Use Case Model i ERD. Ukupni vremenski okvir projekta je 13 sedmica, a razvoj traje od Sprinta 4 (inicijalizacija) do Sprinta 10 (finalne funkcionalnosti).

Tim broji **8 članova** koji rade paralelno, što omogućava istovremenu realizaciju više user storija unutar jednog sprinta uz jasnu podjelu odgovornosti po oblastima.

---

## 2. Pregled svih releasea

Sistem je podijeljen na 5 releasea koji odgovaraju logičkim skupinama funkcionalnosti. Svaki release zatvara jedan inkrement sistema koji je upotrebljiv i testabilan. Nije moguće prijeći na sljedeći release bez završetka prethodnog, jer postoje čvrste zavisnosti između slojeva.

| Release | Naziv | Sprintovi | Kapacitet tima | Ključni isporučaj |
|---------|-------|-----------|----------------|-------------------|
| **R0** | Projektna dokumentacija i inicijalizacija | Sprint 1, 2, 3 i 4 | – | Kompletna projektna dokumentacija i postavljeni tehnički temelji |
| **R1** | Autentifikacija i jezgro sistema | Sprint 5 | ~62% | Funkcionalan login sistem sa JWT tokenima i role-based pristupom |
| **R2** | Administracija i inventar | Sprint 6 | ~95% | Admin panel za upravljanje korisnicima, kabinetima i opremom |
| **R3** | Rezervacijski sistem | Sprint 7 i Sprint 8 | ~65% / ~80% | Funkcionalan end-to-end tok rezervacije termina i opreme |
| **R4** | Upravljanje opremom i napredne funkcionalnosti | Sprint 9 i Sprint 10 | ~40% / ~85% | Pregled i otkazivanje rezervacija, kvarovi, profil i sekundarne funkcionalnosti |

---

## 3. Detalji po releaseu i sprintu

---

### Release 0 – Projektna dokumentacija i inicijalizacija

**Obuhvaćeni sprintovi:** Sprint 1, Sprint 2, Sprint 3 i Sprint 4  
**Ključni isporučaj:** Kompletna projektna dokumentacija i postavljeni tehnički temelji  
**Zavisnosti:** Nema – ovo je polazišna tačka projekta.  
**Glavni rizici:** Nejasna vizija projekta može uzrokovati pogrešne pretpostavke u kasnijim fazama; kašnjenje tehničkog setu može blokirati Sprint 5.

---

#### Sprint 1 | 25.03.2026 – 28.03.2026

**Cilj sprinta:** Definisanje vizije projekta i identifikacija stakeholdera

| ID | Naziv stavke | Tip | Story Pts | Prioritet | Status |
|----|-------------|-----|-----------|-----------|--------|
| US– | Team Charter | Dokumentacija | – | High | Done |
| PB02 | Product Vision | Dokumentacija | 5 | High | Done |
| PB03/04 | Stakeholder Map | Dokumentacija/Research | 6 | High | Done |
| PB01 | Definisanje problema i ciljeva sistema | Research | 3 | High | Done |
| PB05 | Početni Product Backlog (35 stavki) | Dokumentacija | 5 | High | Done |

---

#### Sprint 2 | 31.03.2026 – 05.04.2026

**Cilj sprinta:** Razrada korisničkih priča, kriterija prihvatljivosti i nefunkcionalnih zahtjeva

| ID | Naziv stavke | Tip | Story Pts | Prioritet | Status |
|----|-------------|-----|-----------|-----------|--------|
| PB06 | User Stories (US01–US34) | Dokumentacija | 5 | High | Done |
| PB07 | Acceptance Criteria za sve user storyje | Dokumentacija | 5 | High | Done |
| PB08 | Prioritizacija backloga (MoSCoW) | Research | 3 | High | Done |
| PB09 | NFR zahtjevi (NFR-01 do NFR-20) | Dokumentacija | 3 | Medium | Done |
| PB– | Ažurirani Product Backlog | Dokumentacija | – | High | Done |

---

#### Sprint 3 | 06.04.2026 – 12.04.2026

**Cilj sprinta:** Modelovanje sistema, arhitektura, strategija testiranja i upravljanje rizicima

| ID | Naziv stavke | Tip | Story Pts | Prioritet | Status |
|----|-------------|-----|-----------|-----------|--------|
| PB10 | Risk Register (30 rizika) | Dokumentacija | 3 | Medium | Done |
| PB11a | Domain Model (7 entiteta, 24 poslovna pravila) | Dokumentacija | 3 | High | Done |
| PB11b | Use Case Model (UC01–UC19) | Dokumentacija | 3 | High | Done |
| PB12 | Architecture Overview (4-slojna arhitektura) | Dokumentacija | 5 | High | Done |
| PB13 | Test Strategy (8 nivoa testiranja) | Dokumentacija | 3 | Medium | Done |
| PB16 | ERD – Finalni model podataka (13 tabela) | Dokumentacija | 3 | Medium | Done |
| PB– | Tehnološki Setup Sistema (ASP.NET/React/PostgreSQL/Docker) | Dokumentacija | – | High | Done |

---

#### Sprint 4 | TBD

**Cilj sprinta:** Definisanje kriterija završetka, plana objave i uspostava tehničkog skeleta

| ID | Naziv stavke | Tip | Story Pts | Prioritet | Status |
|----|-------------|-----|-----------|-----------|--------|
| PB14 | Definition of Done (DoD) | Dokumentacija | 2 | Medium | To Do |
| PB15 | Initial Release Plan | Dokumentacija | 3 | Medium | To Do |
| PB17 | Postavljanje tehničkog skeletona (repo, CI/CD, Docker Compose, skeleton projekta) | Technical Task | 5 | High | To Do |

> **Sažetak releasea:** Ovaj release obuhvata svu dokumentarnu i analitičku fazu projekta. Na kraju Sprinta 4 tim ima potpuno definisanu viziju, arhitekturu, model podataka i funkcionalan razvojni ambijent spreman za implementaciju.

---

### Release 1 – Autentifikacija i jezgro sistema

**Obuhvaćeni sprintovi:** Sprint 5  
**Ključni isporučaj:** Funkcionalan login sistem sa JWT tokenima i role-based pristupom  
**Kapacitet tima:** ~62% (namjerno – auth mora biti temeljno istestiran)  
**Zavisnosti:** Release 0 mora biti završen. Tehnički skelet (Docker, repo, CI/CD) mora biti postavljen prije prvog commita poslovne logike.  
**Glavni rizici:** Nedovoljno iskustvo tima sa JWT implementacijom može uzrokovati sigurnosne propuste (R028); neautorizirani pristup admin panelu ako RBAC nije ispravno implementiran (R001).

---

#### Sprint 5 | TBD

**Cilj sprinta:** Kompletna implementacija autentifikacije i autorizacije korisnika

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US03 / US29 | Prijava na sistem + JWT generisanje | Prijava korisnika uz generisanje sigurnog JWT tokena koji osigurava stateless sesiju | Feature | 3+3 | High | To Do |
| US30 | Role-based access control – RBAC | Ograničenje pristupa prema ulozi (Admin, Profesor, Student, Tehničar); 403 Forbidden za neovlaštene rute | Feature | 3 | High | To Do |
| US31 | Sigurna odjava | JWT token se uništava na klijentskoj strani; back button ne vraća na zaštićene stranice | Feature | 2 | High | To Do |
| US32 | Automatski istek sesije | Sistem odjavljuje korisnika nakon 30 min neaktivnosti uz obavještenje | Feature | 2 | Medium | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Login UI + validacija forme (frontend) |
| M2 | JWT generisanje i verifikacija (backend) |
| M3 | RBAC logika – uloge i zaštita ruta |
| M4 | Sigurna odjava + redirect na login stranicu |
| M5 | Automatski istek sesije (US32) |
| M6 | Testiranje auth toka (unit + integration testovi) |
| M7 | CI/CD pipeline + Docker konfiguracija |
| M8 | API setup, dokumentacija endpointa |

> **Sažetak releasea:** Na kraju ovog releasea svaki korisnik može se prijaviti i odjaviti. Sistem prepoznaje ulogu svakog korisnika i restriktira pristup samo dozvoljenoj sadržini. JWT token osigurava stateless sigurnost, a sesija automatski ističe radi zaštite od zloupotrebe na dijeljenim računarima. Ovo je preduslov za sve naredne releasee.

---

### Release 2 – Administracija i inventar

**Obuhvaćeni sprintovi:** Sprint 6  
**Ključni isporučaj:** Admin panel za upravljanje korisnicima, kabinetima i opremom  
**Kapacitet tima:** ~95% (kritičan sprint – svi rade paralelno na jasno odvojenim CRUD oblastima)  
**Zavisnosti:** Release 1 mora biti završen. CRUD operacije zahtijevaju funkcionalan JWT i RBAC sistem – bez autentifikacije nije moguće razlikovati administratora od studenta.  
**Glavni rizici:** Preopterećenost tima u Sprintu 6 zbog najvećeg broja paralelnih featura (R–); neslaganje oko tehničkih odluka pri integraciji komponenti (R027).

---

#### Sprint 6 | TBD

**Cilj sprinta:** Kreiranje CRUD operacija za osnovne resurse sistema

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US01 / US02 | Kreiranje i brisanje korisnika | Administrator kreira i briše korisničke naloge; brisanje zahtijeva confirmation dialog | Feature | 5 | High | To Do |
| US22 | Dodavanje i uređivanje kabineta | Administrator dodaje i uređuje prostorije; prikaz liste svih kabineta | Feature | 3 | High | To Do |
| US23 | Blokiranje perioda u kabinetu | Administrator blokira termine u kabinetu; blokiran period vizuelno označen u kalendaru | Feature | 3 | Medium | To Do |
| US06 / US07 | Dodavanje i brisanje opreme | Tehničar unosi novu opremu u bazu i briše opremu koja se više ne koristi | Feature | 5 | High | To Do |
| US08 | Uređivanje statusa opreme | Tehničar mijenja status opreme; promjena se ažurira u realnom vremenu | Feature | 4 | Medium | To Do |
| US17 | Kreiranje radnog vremena laboratorije | Administrator definira radno vrijeme za specifičnu prostoriju | Feature | 3 | High | To Do |
| US18 / US19 / US20 | Definisanje, mijenjanje i brisanje termina | Tehničar kreira, uređuje i briše termine; promjene se odmah reflektuju u kalendaru | Feature | 5 | High | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Korisnici CRUD – frontend (US01/02) |
| M2 | Korisnici CRUD – backend i baza |
| M3 | Kabineti + blokiranje perioda (US22/23) |
| M4 | Oprema CRUD – frontend (US06/07) |
| M5 | Status opreme – backend logika (US08) |
| M6 | Radno vrijeme laboratorije (US17) |
| M7 | Termini – definisanje, izmjena, brisanje (US18/19/20) |
| M8 | Integracijski testovi za sve CRUD operacije |

> **Sažetak releasea:** Sprint 6 je najopterećeniji u cijelom projektu. Ključ uspjeha je da svaki član ima strogo odvojenu oblast. M8 je dedikovan za testiranje dok ostali razvijaju. Na kraju sprinta administrator može upravljati korisnicima, kabinetima i opremom, a tehničar može definisati termine i radno vrijeme. Ovo je preduslov za funkcionisanje rezervacijskog sistema.

---

### Release 3 – Rezervacijski sistem

**Obuhvaćeni sprintovi:** Sprint 7 i Sprint 8  
**Ključni isporučaj:** Funkcionalan end-to-end tok rezervacije termina i opreme  
**Zavisnosti:** Release 2 mora biti završen. Rezervacijski sistem ne može funkcionisati bez prethodno kreiranog inventara – kabineti, oprema, termini i korisnici moraju postojati u bazi.  
**Glavni rizici:** Race condition – dupla rezervacija ako database locks nisu ispravno implementirani (R004; NFR-16); iznenadni novi zahtjevi koji remete scope (R011).

---

#### Sprint 7 | TBD

**Cilj sprinta:** Prikaz dostupnosti i podnošenje zahtjeva za rezervaciju  
**Kapacitet tima:** ~65%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US21 / US24 | Pregled i kalendarski prikaz slobodnih termina | Student pregledava termine u kalendarskom prikazu; color-coded status (slobodan/zauzet/blokiran); filtriranje po kabinetu | Feature | 5 | High | To Do |
| US11 | Kreiranje zahtjeva za rezervaciju | Student rezerviše slobodan termin i opremu; sistem sprječava preklapanja i rezervacije u prošlosti; zahtjev se šalje asistentu na odobrenje | Feature | 5 | High | To Do |
| US25 | Odabir specifične opreme unutar kabineta | Student odabire tačno određenu opremu iz kabineta pri rezervaciji; moguće označiti više stavki | Feature | 2 | Medium | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Kalendar UI – prikaz i color-coding (US24) |
| M2 | Lista slobodnih termina – backend (US21) |
| M3 | Kreiranje rezervacije – frontend flow (US11) |
| M4 | Kreiranje rezervacije – backend + validacija |
| M5 | Odabir opreme unutar kabineta (US25) |
| M6 | API integracija frontend–backend |
| M7 | Testiranje UI i rezervacijskog toka |
| M8 | Buffer – podrška, code review, dokumentacija endpointa |

---

#### Sprint 8 | TBD

**Cilj sprinta:** Validacija konfliktnih rezervacija i workflow odobravanja  
**Kapacitet tima:** ~80%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US26 | Automatska validacija konflikta termina i opreme | Sistem provjerava bazu podataka u milisekundama prije potvrde rezervacije; poruka "Termin je u međuvremenu zauzet" | Technical Task | 5 | High | To Do |
| US14 | Pregled svih rezervacija (asistent/profesor) | Profesor/asistent pregledava listu svih zauzetih i slobodnih termina, historiju zauzeća; opcija eksporta podataka | Feature | 3 | High | To Do |
| US15 | Odobravanje i odbijanje zahtjeva | Profesor/asistent pregleda Pending zahtjeve i može ih odobriti ili odbiti uz komentar; status se ažurira u realnom vremenu | Feature | 5 | High | To Do |
| US27 | In-app notifikacija o odluci | Student prima email ili in-app notifikaciju o odobrenju/odbijanju, uključujući komentar asistenta | Feature | 3 | Medium | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Database Locks + konflikt validacija (US26) |
| M2 | Pregled svih rezervacija – backend (US14) |
| M3 | Odobravanje/odbijanje zahtjeva – backend (US15) |
| M4 | Odobravanje/odbijanje zahtjeva – frontend |
| M5 | In-app notifikacije (US27) |
| M6 | Pregled rezervacija – frontend (US14) |
| M7 | API integracija i validacija statusa |
| M8 | E2E testiranje cijelog rezervacijskog toka |

> **Sažetak releasea:** Ovo je srž sistema. Student vidi kalendar, bira termin i opremu, šalje zahtjev. Sistem automatski blokira konfliktne rezervacije (database locks, NFR-16). Profesor/asistent dobija listu zahtjeva i odobrava ili odbija ih. Student prima in-app obavijest o odluci. Nakon ovog releasea sistem je funkcionalno upotrebljiv za osnovni laboratorijski rad.

---

### Release 4 – Upravljanje opremom i napredne funkcionalnosti

**Obuhvaćeni sprintovi:** Sprint 9 i Sprint 10  
**Ključni isporučaj:** Pregled i otkazivanje rezervacija, upravljanje kvarovima, profil korisnika i sekundarne funkcionalnosti  
**Zavisnosti:** Release 3 mora biti završen. Upravljanje kvarovima i automatska promjena statusa direktno ovise o rezervacijskom toku koji mora biti testiran i stabilan.  
**Glavni rizici:** Nedostupnost developera u finalnim sprintovima može ugroziti demo pripremu (R009); Sprint 10 je zahtjevan po broju stavki – potrebno je pažljivo planiranje kapaciteta.

---

#### Sprint 9 | TBD

**Cilj sprinta:** Pregled, otkazivanje i ograničavanje rezervacija od strane korisnika  
**Kapacitet tima:** ~40% (namjerno – buffer za refactoring i bugfixing iz prethodnih sprintova)

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US12 | Pregled vlastitih zahtjeva | Student pregledava sve podnesene i odobrene zahtjeve | Feature | 2 | Medium | To Do |
| US13 | Otkazivanje rezervacije | Student otkazuje aktivan termin uz trenutno oslobađanje resursa | Feature | 3 | Medium | To Do |
| US16 | Ograničenje broja aktivnih rezervacija po studentu | Profesor postavlja limit; sistem prikazuje upozorenje kada se limit dostigne | Feature | 2 | Medium | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Pregled vlastitih zahtjeva – frontend (US12) |
| M2 | Pregled vlastitih zahtjeva – backend |
| M3 | Otkazivanje rezervacije – frontend + backend (US13) |
| M4 | Limitiranje rezervacija po studentu (US16) |
| M5–M8 | Refactoring, bugfixing iz prethodnih sprintova, performance testiranje |

---

#### Sprint 10 | TBD

**Cilj sprinta:** Prijava kvarova, filtriranje opreme, upravljanje profilom i sigurnosne funkcionalnosti  
**Kapacitet tima:** ~85%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US09 / US28 | Prijava kvara + automatsko otkazivanje budućih rezervacija | Profesor prijavljuje kvar opreme; sistem mijenja status na "neispravna" i otkazuje sve Pending/Approved rezervacije; korisnici primaju obavijest | Feature | 3+3 | Medium | To Do |
| US10 | Filtriranje i pretraga opreme | Korisnik filtrira opremu po kategoriji, dostupnosti i nazivu; mogućnost korištenja više filtera; opcija resetovanja | Feature | 3 | Medium | To Do |
| US04 / US05 | Promjena lozinke i uređivanje profila | Korisnik mijenja lozinku (uz validaciju minimalne dužine) i ažurira vlastite podatke na profilu | Feature | 6 | Medium | To Do |
| US33 | Oporavak lozinke putem emaila | Korisnik resetuje zaboravljenu lozinku putem unikatnog privremenog linka poslanog na email | Feature | 3 | Medium | To Do |
| US34 | Evidentiranje aktivnosti prijava (audit log) | Sistem bilježi korisničko ime i timestamp svake uspješne i neuspješne prijave; administrator može pregledati listu zapisa | Feature | 2 | Low | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Forma za prijavu kvara – frontend (US09) |
| M2 | Automatsko otkazivanje rezervacija pri kvaru (US28) |
| M3 | Filtriranje i pretraga opreme (US10) |
| M4 | Promjena lozinke i profila – frontend (US04/05) |
| M5 | Promjena lozinke – backend + validacija |
| M6 | Oporavak lozinke putem emaila (US33) |
| M7 | Audit log prijava (US34) |
| M8 | Finalno testiranje, dokumentacija, demo priprema |

> **Sažetak releasea:** Ovaj release zaokružuje funkcionalni sistem. Sprint 9 je namjerno rasterećen kao buffer za stabilizaciju i bugfixing. Sprint 10 donosi preostale funkcionalnosti: kvarovi se prijavljuju direktno kroz aplikaciju i automatski povlače buduće rezervacije, korisnici filtriraju opremu, upravljaju profilom i resetuju lozinku. Slobodan kapacitet u S9 (M5–M8) služi kao planirani buffer za pripremu finalne demo verzije sistema.

---

## 4. Zavisnosti između releasea

| Release | Ovisi o | Razlog zavisnosti |
|---------|---------|-------------------|
| **Release 1 (Sprint 5)** | Release 0 (Sprintovi 1–4) | Tehnički skelet i razvojni ambijent moraju biti postavljeni prije prvog commita poslovne logike. |
| **Release 2 (Sprint 6)** | Release 1 (Sprint 5) | CRUD operacije zahtijevaju funkcionalan JWT i RBAC – bez autentifikacije nije moguće razlikovati administratora od studenta. |
| **Release 3 (Sprintovi 7–8)** | Release 2 (Sprint 6) | Rezervacijski sistem ne može funkcionisati bez prethodno kreiranog inventara (kabineti, oprema, termini, korisnici). |
| **Release 4 (Sprintovi 9–10)** | Release 3 (Sprintovi 7–8) | Upravljanje kvarovima i automatska promjena statusa direktno ovise o rezervacijskom toku koji mora biti testiran i stabilan. |

---

## 5. Mapiranje ključnih NFR zahtjeva na releasee

| NFR ID | Opis (skraćeno) | Kategorija | Release |
|--------|----------------|------------|---------|
| NFR-05/06/07 | JWT 401 u 100ms; 403 za studenta na admin rutama; BCrypt heširanje | Sigurnost | R1 |
| NFR-11 | Logika razdvojena po slojevima (layered arch.) | Održivost | R1 |
| NFR-18 | Svi tekstovi na BHS jezicima | Lokalizacija | R2 |
| NFR-01 | Rezervacija u max 4 klika | Upotrebljivost | R3 |
| NFR-02 | Dashboard profesora < 1 sekunda | Upotrebljivost | R3 |
| NFR-04 | Responzivnost 360px–1920px | Upotrebljivost | R3 |
| NFR-16 | Database Locks – spriječiti duplu rezervaciju | Pouzdanost | R3 |
| NFR-17 | ACID transakcije – oporavak < 500ms | Pouzdanost | R3 |
| NFR-14/15 | API lista opreme < 500ms; upis rezervacije < 300ms | Performanse | R3/R4 |
| NFR-12 | 80% unit test pokrivenost kritične logike | Održivost | R3/R4 |
| NFR-03 | Promjena statusa opreme < 10 sekundi | Upotrebljivost | R4 |
| NFR-08 | JMBG vidljiv samo u Approved zahtjevu | Privatnost | R4 |
| NFR-13 | 50 concurrent korisnika, odziv < 1s | Skalabilnost | R4 |
| NFR-19 | Automatski incremental backup svakih 24h | Backup | R4 |

---

## 6. Kriteriji završetka (Draft Definition of Done)

Svaka stavka se smatra završenom kada su ispunjeni sljedeći kriteriji (bit će finalizirani u Sprintu 4):

- Kod je pregledan od strane najmanje jednog kolege (code review na GitHub-u).
- Unit testovi su napisani i prolaze; pokrivenost kritične logike ≥ 80% (NFR-12).
- Funkcionalnost je integracijski testirana i radi ispravno s bazom podataka.
- API endpoint (ako postoji) vraća ispravne HTTP status kodove (200, 201, 401, 403, 404).
- UI je testiran na Chrome v110+, Firefox v105+ i Safari v15+ (NFR-10).
- UI je responzivan na rezolucijama od 360px do 1920px (NFR-04).
- Svi tekstovi su na BHS jeziku (NFR-18).
- Dokumentacija je ažurirana (API spec, komentari u kodu).
- Product Owner je pregledao i odobrio funkcionalnost.

---

## 7. Ključni rizici plana i mitigacijske mjere

| ID | Rizik | Vjerovatnoća | Uticaj | Mitigacija u planu |
|----|-------|--------------|--------|--------------------|
| R009 | Nedostupnost developera – kašnjenje isporuke | Srednja | Visok | GitHub uvid u rad; zamjenski resursi unutar tima; buffer kapacitet u S9 |
| R028 | Nedovoljno iskustvo tima za zahtjevane tehnologije | Srednja | Visok | Redovni sastanci; parni rad; istraživanje unaprijed |
| R027 | Neslaganje u timu oko tehničkih odluka | Srednja | Visok | Glasanje prostom većinom; eskalacija asistentu |
| R011 | Iznenadno dodavanje novih zahtjeva | Visoka | Srednji | Potpisan scope; svaki novi zahtjev = produženje roka |
| R001 | Neautorizirani pristup admin panelu | Niska | Visok | JWT + RBAC implementacija u Sprintu 5 (Release 1) |
| R004 | Race condition – dupla rezervacija | Srednja | Srednji | Database Locks (NFR-16) implementacija u Sprintu 8 |
| R– | Preopterećenost tima u Sprintu 6 | Srednja | Visok | Jasna podjela oblasti po članu; M8 dedikovan isključivo za testiranje |

---

## 8. Vizualni pregled timeline-a

| Release | S1 | S2 | S3 | S4 | S5 | S6 | S7 | S8 | S9 | S10 |
|---------|----|----|----|----|----|----|----|----|----|----|
| **R0 – Dokumentacija** | ✓ | ✓ | ✓ | ↻ | – | – | – | – | – | – |
| **R1 – Autentifikacija** | – | – | – | – | ◉ | – | – | – | – | – |
| **R2 – Administracija** | – | – | – | – | – | ◉ | – | – | – | – |
| **R3 – Rezervacije** | – | – | – | – | – | – | ◉ | ◉ | – | – |
| **R4 – Napredno** | – | – | – | – | – | – | – | – | ◉ | ◉ |

| Sprint | Kapacitet | Napomena |
|--------|-----------|----------|
| S5 | ~62% | Namjerno lakši – auth mora biti solidno istestiran |
| S6 | ~95% | Kritičan – 8 članova paralelno na odvojenim CRUD oblastima |
| S7 | ~65% | Osnova rezervacijskog toka: kalendar, kreiranje, odabir opreme |
| S8 | ~80% | Validacija konflikta, odobravanje zahtjeva, notifikacije |
| S9 | ~40% | Pregled/otkazivanje rezervacija + buffer za refactoring i bugove |
| S10 | ~85% | Kvarovi, filtriranje, profil, audit log + finalna demo priprema |

*✓ Završeno &nbsp;&nbsp; ↻ U toku &nbsp;&nbsp; ◉ Planiran release &nbsp;&nbsp; – Nije u sprintu*
