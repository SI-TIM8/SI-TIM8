# INITIAL RELEASE PLAN

## 1. Uvod
Plan je zasnovan na svim dokumentima kreiranim tokom Sprintova 1–3: Product Backlog (44 stavke), User Stories (US01–US34), Nefunkcionalni zahtjevi (NFR-01–NFR-20), Architecture Overview, Domain Model, Use Case Model i ERD. Ukupni vremenski okvir projekta je 13 sedmica, a razvoj traje od Sprinta 4 (inicijalizacija) do Sprinta 10 (finalne funkcionalnosti).

---

## 2. Pregled svih releasea

Sistem je podijeljen na 5 releasea koji odgovaraju logičkim skupinama funkcionalnosti. Svaki release zatvara jedan inkrement sistema koji je upotrebljiv i testabilan. Nije moguće preći na sljedeći release bez završetka prethodnog, jer postoje čvrste zavisnosti između slojeva.

| Release | Naziv | Sprintovi | Isporuka |
|---------|-------|-----------|-------------------|
| **R0** | Projektna dokumentacija i inicijalizacija | Sprint 1, 2, 3 i 4 | Kompletna projektna dokumentacija i postavljeni tehnički temelji |
| **R1** | Autentifikacija i jezgro sistema | Sprint 5 | Funkcionalan login sistem sa JWT tokenima i role-based pristupom |
| **R2** | Administracija i inventar | Sprint 6 | Admin panel za upravljanje korisnicima, kabinetima i opremom |
| **R3** | Rezervacijski sistem | Sprint 7 i Sprint 8 |  Funkcionalan end-to-end tok rezervacije termina i opreme |
| **R4** | Upravljanje opremom i napredne funkcionalnosti | Sprint 9 i Sprint 10 | Pregled i otkazivanje rezervacija, kvarovi, profil i sekundarne funkcionalnosti |

---

### Release 1 – Autentifikacija i jezgro sistema

**Obuhvaćeni sprintovi:** Sprint 5  
**Ključna isporuka:** Funkcionalan login sistem sa JWT tokenima i role-based pristupom  
**Kapacitet tima:** ~62% (namjerno – auth mora biti temeljno istestiran)  
**Zavisnosti:** Release 0 mora biti završen. Tehnički skelet mora biti postavljen prije prvog commita poslovne logike.  
**Glavni rizici:** Nedovoljno iskustvo tima sa JWT implementacijom može uzrokovati sigurnosne propuste (R028); neautorizirani pristup admin panelu ako RBAC nije ispravno implementiran (R001).

---

#### Sprint 5 |

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
**Ključna isporuka:** Admin panel za upravljanje korisnicima, kabinetima i opremom  
**Kapacitet tima:** ~95% (kritičan sprint – svi rade paralelno na jasno odvojenim CRUD oblastima)  
**Zavisnosti:** Release 1 mora biti završen. CRUD operacije zahtijevaju funkcionalan JWT i RBAC sistem – bez autentifikacije nije moguće razlikovati administratora od studenta.  
**Glavni rizici:** Preopterećenost tima u Sprintu 6 zbog najvećeg broja paralelnih featura (R–); neslaganje oko tehničkih odluka pri integraciji komponenti (R027).

---

#### Sprint 6 | TBD

**Cilj sprinta:** Kreiranje CRUD operacija za osnovne resurse sistema

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US01 / US02 | Kreiranje, deaktivacija i aktivacija korisnika | Administrator kreira korisničke naloge, deaktivira neaktivne korisnike i po potrebi ih ponovo aktivira; deaktivacija zahtijeva confirmation dialog | Feature | 5 | High | To Do |
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
| M2 | Korisnici CRUD – backend i baza, uključujući deaktivaciju i aktivaciju |
| M3 | Kabineti + blokiranje perioda (US22/23) |
| M4 | Oprema CRUD – frontend (US06/07) |
| M5 | Status opreme – backend logika (US08) |
| M6 | Radno vrijeme laboratorije (US17) |
| M7 | Termini – definisanje, izmjena, brisanje (US18/19/20) |
| M8 | Integracijski testovi za sve CRUD operacije |

> **Sažetak releasea:** Sprint 6 je najopterećeniji u cijelom projektu. Ključ uspjeha je da svaki član ima strogo odvojenu oblast. M8 je dedikovan za testiranje dok ostali razvijaju. Na kraju sprinta administrator može upravljati korisnicima, kabinetima i opremom, pri čemu se korisnici ne brišu trajno nego aktiviraju/deaktiviraju, a tehničar može definisati termine i radno vrijeme. Ovo je preduslov za funkcionisanje rezervacijskog sistema.

---

### Release 3 – Rezervacijski sistem

**Obuhvaćeni sprintovi:** Sprint 7 i Sprint 8  
**Ključna isporuka:** Funkcionalan end-to-end tok rezervacije termina i opreme  
**Zavisnosti:** Release 2 mora biti završen. Rezervacijski sistem ne može funkcionisati bez prethodno kreiranog inventara – kabineti, oprema, termini i korisnici moraju postojati u bazi.  
**Glavni rizici:** Race condition – dupla rezervacija ako database locks nisu ispravno implementirani (R004; NFR-16); iznenadni novi zahtjevi koji remete scope (R011).

---

#### Sprint 7 | TBD

**Cilj sprinta:** Prikaz dostupnosti, podnošenje i upravljanje zahtjevima za rezervaciju
**Kapacitet tima:** ~75%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|--------------|------|-----|-----------|-----------|--------|
| US21 / US24 | Pregled i kalendarski prikaz slobodnih termina | Student pregledava termine u kalendarskom prikazu; color-coded status (slobodan/zauzet/blokiran); filtriranje po kabinetu | Feature | 5 | High | Završeno |
| US11 | Kreiranje zahtjeva za rezervaciju | Student rezerviše slobodan termin i opremu; sistem sprječava preklapanja i rezervacije u prošlosti; zahtjev se šalje asistentu na odobrenje | Feature | 5 | High | Završeno |
| US15 | Odobravanje i odbijanje zahtjeva | Profesor/asistent pregleda Pending zahtjeve i može ih odobriti ili odbiti uz komentar; status se ažurira u realnom vremenu | Feature | 5 | High | Završeno |
| US12 | Pregled vlastitih zahtjeva | Student pregledava sve podnesene i odobrene zahtjeve | Feature | 2 | High | Završeno |
| US13 | Otkazivanje rezervacije | Student otkazuje aktivan termin uz trenutno oslobađanje resursa | Feature | 3 | Medium | Završeno |
| US14 | Pregled svih rezervacija (asistent/profesor) | Profesor/asistent pregledava listu svih zauzetih i slobodnih termina, historiju zauzeća; opcija eksporta podataka | Feature | 3 | High | Završeno |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Kalendar UI – prikaz i color-coding (US24) |
| M2 | Lista slobodnih termina – backend (US21) |
| M3-M4 | Rezervacije CRUD – frontend, backend (US11) |
| M5-M6 | Odobravanje/odbijanje zahtjeva, Pregled vlastitih zahtjeva – frontend, backend |
| M7-M8 | Testiranje dodanih operacija – rezervacija |

> **Napomena:** Sprint 7 je završen sa proširenim opsegom u odnosu na originalni plan.
---

#### Sprint 8 | TBD

**Cilj sprinta:** Validacija konfliktnih rezervacija i workflow odobravanja  
**Kapacitet tima:** ~80%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US26 | Automatska validacija konflikta termina i opreme | Sistem provjerava bazu podataka u milisekundama prije potvrde rezervacije; poruka "Termin je u međuvremenu zauzet" | Technical Task | 5 | High | To Do |
| US27 | In-app notifikacija o odluci | Student prima email ili in-app notifikaciju o odobrenju/odbijanju, uključujući komentar asistenta | Feature | 3 | Medium | To Do |
| US37 | Automatska promjena statusa opreme po rezervaciji | Sistem automatski označava opremu kao zauzetu tokom potvrđene rezervacije i oslobađa je po završetku termina | Technical Task | 2 | Medium | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1-M2 | Database Locks + konflikt validacija (US26) |
| M3-M4 | In-app notifikacije (US27) |
| M5-M6 | Automatska promjena statusa opreme (US37) |
| M7-M8 | E2E testiranje cijelog rezervacijskog toka |

> **Sažetak sprinta:** Sprint 8 zaokružuje Release 3. Tehničke pretpostavke rezervacijskog sistema (database locks, automatski status opreme) postavljaju se uz paralelno provođenje integracijskog i E2E testiranja kompletnog toka koji je implementiran u Sprintu 7. Na kraju sprinta sistem je funkcionalno upotrebljiv i stabilan za osnovni laboratorijski rad.

---

### Release 4 – Upravljanje opremom i napredne funkcionalnosti

**Obuhvaćeni sprintovi:** Sprint 9 i Sprint 10  
**Ključna isporuka:** Prijava kvarova, automatsko otkazivanje rezervacija pri kvaru, filtriranje opreme, oporavak lozinke i audit log  
**Zavisnosti:** Release 3 mora biti završen. Upravljanje kvarovima i automatska promjena statusa direktno ovise o rezervacijskom toku koji mora biti testiran i stabilan.  
**Glavni rizici:** Nedostupnost developera u finalnim sprintovima može ugroziti demo pripremu (R009); Sprint 10 je zahtjevan po broju stavki – potrebno je pažljivo planiranje kapaciteta.

---

#### Sprint 9 | TBD

**Cilj sprinta:** Prijava kvarova, filtriranje opreme i sistemsko testiranje 
**Kapacitet tima:** ~65%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US09 / US28 | Prijava kvara + automatsko otkazivanje budućih rezervacija | Profesor prijavljuje kvar opreme; sistem mijenja status na "neispravna" i otkazuje sve Pending/Approved rezervacije; pogođeni korisnici primaju obavijest | Feature | 3+3 | Medium | To Do |
| US16 | Ograničenje broja aktivnih rezervacija po studentu | Profesor postavlja limit; sistem prikazuje upozorenje kada se limit dostigne | Feature | 2 | Medium | To Do |
| – | Sistemsko i performansno testiranje | E2E tok s kvarom opreme; load testiranje sa 50 concurrent korisnika; provjera ACID transakcija i backup mehanizma | Testing | 4 | High | To Do |

**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Forma za prijavu kvara – frontend (US09) |
| M2 | Automatsko otkazivanje rezervacija pri kvaru – backend (US28) |
| M3-M4 | Ograničenje broja rezervacija po studentu (US16)|
| M5 | Sistemsko E2E testiranje: kvar → otkazivanje → notifikacija |
| M6 |Load testiranje – 50 concurrent korisnika (NFR-13); ACID i backup provjera (NFR-17, NFR-19) |
| M7-M8 | Regresijsko testiranje svih funkcionalnosti do kraja S9 |

> **Sažetak sprinta:** Sprint 9 donosi prijavu kvarova s automatskim povlačenjem svih pogođenih rezervacija i filtriranje opreme. Slobodan kapacitet tima (M6–M8) usmjeren je na sistemsko i performansno testiranje – load test s 50 korisnika i provjera ACID/backup integriteta provode se ovdje dok je sistem već stabilan nakon S8.

---

#### Sprint 10 | TBD

**Cilj sprinta:** Oporavak lozinke, audit log, finalno testiranje i demo priprema 
**Kapacitet tima:** ~85%

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US33 | Oporavak lozinke putem emaila | Korisnik resetuje zaboravljenu lozinku putem unikatnog privremenog linka poslanog na email | Feature | 3 | Medium | To Do |
| US34 | Evidentiranje aktivnosti prijava (audit log) | Sistem bilježi korisničko ime i timestamp svake uspješne i neuspješne prijave; administrator može pregledati listu zapisa | Feature | 2 | Low | To Do |
| – | Sigurnosno testiranje | RBAC matrica svih uloga; zaštita od SQL injection; enkripcija JMBG; provjera hash algoritma lozinki | Testing | 3 | High | To Do |
| – | UI, cross-browser i lokalizacijsko testiranje | Provjera responzivnosti (360px–1920px); testiranje na Chrome, Firefox, Safari; BHS lokalizacija svih tekstova | Testing | 3 | High | To Do |
| – | UAT i finalna demo priprema | Testiranje prihvatljivosti s Product Ownerom; priprema demo verzije | Testing | 4 | High | To Do |


**Podjela rada po članovima tima:**

| Član | Oblast odgovornosti |
|------|---------------------|
| M1 | Oporavak lozinke putem emaila (US33) |
| M2 | Audit log prijava (US34) |
 M3 - M4 | Sigurnosno testiranje: RBAC matrica, SQL injection, NFR-07, NFR-08 |
| M5 | UI testiranje: responzivnost, kalendar color-coding, max 4 klika (NFR-01, NFR-04) |
| M6 | Cross-browser testiranje: Chrome v110+, Firefox v105+, Safari v15+ (NFR-10) |
| M7 | Lokalizacijska provjera – svi BHS tekstovi (NFR-18); regresijski finalni prolaz |
| M8 | UAT koordinacija + finalna demo priprema |

> **Sažetak releasea i projekta:** Sprint 10 zatvara sistem. Oporavak lozinke i audit log upotpunjuju funkcionalni skup. Slobodan kapacitet tima u cijelosti je posvećen finalnom testiranju: sigurnosna provjera svih uloga i enkripcije, cross-browser i lokalizacijska validacija, te UAT s krajnjim korisnicima. Demo verzija sistema je isporučena i prezentabilna po završetku sprinta.

---

## 4. Zavisnosti između releasea

| Release | Ovisi o | Razlog zavisnosti |
|---------|---------|-------------------|
| **Release 1 (Sprint 5)** | Release 0 (Sprintovi 1–4) | Tehnički skelet i razvojni ambijent moraju biti postavljeni prije prvog commita poslovne logike. |
| **Release 2 (Sprint 6)** | Release 1 (Sprint 5) | CRUD operacije zahtijevaju funkcionalan JWT i RBAC – bez autentifikacije nije moguće razlikovati administratora od studenta. |
| **Release 3 (Sprintovi 7–8)** | Release 2 (Sprint 6) | Rezervacijski sistem ne može funkcionisati bez prethodno kreiranog inventara (kabineti, oprema, termini, korisnici). |
| **Release 4 (Sprintovi 9–10)** | Release 3 (Sprintovi 7–8) | Upravljanje kvarovima i automatska promjena statusa direktno ovise o rezervacijskom toku koji mora biti testiran i stabilan. |

---

## 5. Mapiranje ključnih NFR zahtjeva na release

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

## 7. Ključni rizici plana

| ID | Rizik | Vjerovatnoća | Uticaj | Akcije za smanjenje rizika |
|----|-------|--------------|--------|--------------------|
| R009 | Nedostupnost developera – kašnjenje isporuke | Srednja | Visok | GitHub uvid u rad; zamjenski resursi unutar tima; buffer kapacitet u S9 |
| R028 | Nedovoljno iskustvo tima za zahtjevane tehnologije | Srednja | Visok | Redovni sastanci; parni rad; istraživanje unaprijed |
| R027 | Neslaganje u timu oko tehničkih odluka | Srednja | Visok | Glasanje prostom većinom; eskalacija asistentu |
| R011 | Iznenadno dodavanje novih zahtjeva | Visoka | Srednji | Potpisan scope; svaki novi zahtjev = produženje roka |
| R001 | Neautorizirani pristup admin panelu | Niska | Visok | JWT + RBAC implementacija u Sprintu 5 (Release 1) |
| R004 | Race condition – dupla rezervacija | Srednja | Srednji | Database Locks (NFR-16) implementacija u Sprintu 8 |
| R031 | Preopterećenost tima u Sprintu 6 | Srednja | Visok | Jasna podjela oblasti po članu; M8 dedikovan isključivo za testiranje |

---

## 8. Vizualni pregled timeline-a

| Release | S1 | S2 | S3 | S4 | S5 | S6 | S7 | S8 | S9 | S10 |
|---------|----|----|----|----|----|----|----|----|----|----|
| **R0 – Dokumentacija** | ✓ | ✓ | ✓ | ↻ | – | – | – | – | – | – |
| **R1 – Autentifikacija** | – | – | – | – | ✓ | – | – | – | – | – |
| **R2 – Administracija** | – | – | – | – | – | ✓ | – | – | – | – |
| **R3 – Rezervacije** | – | – | – | – | – | – | ✓ | ◉ | – | – |
| **R4 – Napredno** | – | – | – | – | – | – | – | – | ◉ | ◉ |

| Sprint | Kapacitet | Napomena |
|--------|-----------|----------|
| S5 | ~62% | Namjerno lakši – auth mora biti solidno istestiran |
| S6 | ~95% | Kritičan – 8 članova paralelno na odvojenim CRUD oblastima |
| S7 | ~75% | Prošireni opseg: rezervacije, odobravanje, otkazivanje, pregled |
| S8 | ~80% | Database locks, notifikacije, automatski status opreme + integracijsko i E2E testiranje |
| S9 | ~65% | Kvarovi, filtriranje opreme, ograničenje rezervacija + sistemsko i load testiranje |
| S10 | ~85% | Oporavak lozinke, audit log + sigurnosno, UI, cross-browser, UAT i demo priprema |

*✓ Završeno &nbsp;&nbsp; ↻ U toku &nbsp;&nbsp; ◉ Planiran release &nbsp;&nbsp; – Nije u sprintu*
