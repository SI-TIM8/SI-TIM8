# Dokumentacija rada tima

**Projekat:** Sistem za upravljanje laboratorijskim resursima  
**Tim:** Aner Atović, Hamza Hadžić, Emina Hamamdžić, Alma Jusufbegović, Merima Glušac, Haris Macić, Refik Mujčinović, Haris Sadiković  
**Trajanje projekta:** Sprint 1 – Sprint 11 
**Akademska godina:** 2025/2026

---

## 1. Svrha projekta

Cilj projekta bio je razviti web aplikaciju za digitalizaciju upravljanja laboratorijskim resursima na fakultetu. Sistem zamjenjuje nestrukturirane, ručne procese rezervacije laboratorijskog prostora i opreme, kao što su dogovaranje putem emaila, fizičke liste ili usmeni dogovori, centralizovanom platformom kojoj svi akteri pristupaju putem preglednika.

Krajnji rezultat je funkcionalna, višekorisni sistem s podrškom za četiri korisničke uloge, kompletnim tokom rezervacije od podnošenja do odobrenja, upravljanjem inventarom i opremom, sistemom obavijesti te nizom sekundarnih funkcionalnosti poput email verifikacije, dark/light moda i vizualnih indikatora stanja opreme.

---

## 2. Problem koji sistem rješava

Laboratorije na fakultetu dijeli više korisničkih grupa, studenti, profesori, asistenti i tehničko osoblje. Prije uvođenja sistema, nije postojao strukturiran mehanizam za:

- **Rezervaciju termina i opreme** - studenti nisu imali pregled slobodnih termina niti mogućnost slanja formalnog zahtjeva za rezervaciju.
- **Evidenciju stanja opreme** - kvarovi nisu sistematski prijavljivani niti praćeni, što je dovodilo do situacija u kojima se neispravna oprema nudila za rezervaciju.
- **Upravljanje pristupom** - nije bilo jasne podjele prema ulogama; svako je imao isti nivo uvida i mogućnosti.
- **Komunikaciju između aktera** - obavijesti o odobrenju, odbijanju ili kvaru opreme bile su neformalne i nepouzdane.

*Sistem direktno adresira sve četiri oblasti kroz strukturiran tok rada, automatizovane promjene statusa i integrisani sistem obavijesti.*

---

## 3. Glavne korisničke uloge

| Uloga | Opis |
|-------|------|
| **Administrator** | Upravlja korisničkim nalozima (kreiranje, aktivacija, deaktivacija), kabinetima, radnim vremenom, blokiranjem perioda i inventarom opreme. Jedini može kreirati nove naloge u sistemu. |
| **Profesor / Asistent** | Pregleda zahtjeve za rezervaciju, odobrava ili odbija ih uz mogućnost komentara. Prijavljuje kvarove opreme. Može filtrirati i exportovati listu rezervacija. |
| **Student** | Pregleda slobodne termine u kalendarskom prikazu, podnosi zahtjeve za rezervaciju s odabirom kabineta i opreme, prati status vlastitih zahtjeva te ih može otkazati. |
| **Laboratorijski tehničar** | Upravlja inventarom opreme (dodavanje, uređivanje, arhiviranje, tipovi opreme), ažurira status opreme, obrađuje prijavljene kvarove i ima uvid u zdravlje opreme kroz vizualni dashboard. |

---

## 4. Glavne implementirane funkcionalnosti

### 4.1 Autentifikacija i autorizacija
- Prijava korisnika s JWT token autentifikacijom i automatskim istekom sesije pri neaktivnosti.
- Role-based access control (RBAC) - svaka uloga vidi i može koristiti samo njoj namijenjene dijelove aplikacije.
- Obavezna promjena lozinke pri prvom loginu za korisnike koje kreira administrator.
- Oporavak zaboravljene lozinke putem vremenski ograničenog email linka.
- Verifikacija email adrese s prikazom statusa verifikacije na profilu.

### 4.2 Upravljanje resursima (admin / tehničar)
- Kreiranje, uređivanje, deaktivacija i pretraga korisnika s filterima po ulozi i statusu.
- Upravljanje kabinetima: dodavanje, uređivanje, definisanje radnog vremena i blokiranje perioda.
- Upravljanje opremom: CRUD operacije, kategorizacija po tipu, praćenje statusa dostupnosti.
- Arhiviranje opreme umjesto trajnog brisanja - historija i kvarovi ostaju sačuvani, oprema se može vratiti iz arhive.
- Upload PDF dokumentacije i URL linkova uz opremu (max 10 MB po fajlu).
- Vizualni „prsten zdravlja opreme" na dashboardu - color-coded prikaz postotaka ispravne, neispravne i arhivirane opreme.

### 4.3 Rezervacijski sistem
- Kalendarski prikaz slobodnih termina po kabinetu s color-coding statusima.
- Podnošenje zahtjeva za rezervaciju s odabirom kabineta, termina i željene opreme.
- Sistemska validacija konflikta - sprječavanje duplih i preklapajućih rezervacija.
- Ograničenje broja aktivnih zahtjeva po studentu (zahtjevi u statusu *Na čekanju* i *Odobren*).
- Odobravanje ili odbijanje zahtjeva od strane profesora/asistenta uz obavezni ili opcionalni komentar.
- Otkazivanje odobrene rezervacije i povlačenje zahtjeva na čekanju od strane studenta.

### 4.4 Upravljanje kvarovima
- Profesor prijavljuje kvar opreme s opisom problema - status opreme se automatski mijenja u *Neispravna*.
- Sistem automatski otkazuje sve buduće rezervacije koje koriste neispravnu opremu i šalje obavijesti pogođenim korisnicima.
- Tehničar ažurira status opreme tokom procesa popravke i vraćanja u ispravno stanje.

### 4.5 Obavijesti i komunikacija
- In-app sistem obavijesti (zvono) za sve ključne događaje: odobrenje/odbijanje zahtjeva, otkazivanje zbog kvara, opće obavijesti o dostupnosti.
- Email obavijesti za korisnike s verifikovanom email adresom.
- Podsjetnici prije termina (konfigurabilno, npr. 24h i 1h unaprijed).
- Sigurnosni email alert pri promjeni lozinke ili email adrese.

### 4.6 Korisnički profil i UX
- Pregled i uređivanje profilnih podataka uz validaciju; promjena email adrese pokreće ponovnu verifikaciju.
- Prikaz nedavnih aktivnosti korisnika na stranici profila.
- Filtriranje rezervacija po datumu i kabinetu s exportom u CSV i PDF format.
- Dark/Light mode s persistentnom postavkom.
- Dugme za povratak na vrh stranice pri dužem scrollovanju.

---

## 5. Pregled rada kroz sprintove

| Sprint | Cilj | Ključne isporuke |
|--------|------|-----------------|
| **Sprint 1** | Projektna osnova | Definicija problema, Product Vision, Stakeholder Map, inicijalni Product Backlog |
| **Sprint 2** | Zahtjevi | User storiji s acceptance kriterijima, prioritizacija backloga, NFR zahtjevi |
| **Sprint 3** | Arhitektura | Domain Model, Use Case Model, Architecture Overview, ERD, Risk Register |
| **Sprint 4** | Inicijalizacija | Definition of Done, Release Plan, tehnički skelet projekta, Branching Strategy |
| **Sprint 5** | Autentifikacija | Login UI, JWT, RBAC, sigurna odjava, istek sesije, CI/CD pipeline, Docker konfiguracija |
| **Sprint 6** | Administracija | CRUD korisnika s pretragom i filtriranjem, CRUD opreme, radno vrijeme, kabineti, termini |
| **Sprint 7** | Rezervacije | Kalendarski prikaz, kreiranje zahtjeva, odabir opreme, odobravanje/odbijanje, pregled zahtjeva |
| **Sprint 8** | Stabilizacija R3 | Validacija konflikta termina, oporavak lozinke, notifikacije o statusu zahtjeva, tipovi opreme, E2E testiranje |
| **Sprint 9** | Napredne funkcionalnosti | Prijava i obrada kvara, ograničenje zahtjeva, prvi login, pregled i otkazivanje rezervacija, arhiviranje opreme, dokumentacija uz opremu, obavijesti, load testiranje |
| **Sprint 10** | Finalizacija | Email verifikacija, podsjetnici, profil, promjena lozinke, nedavne aktivnosti, sigurnosni alert, filtriranje i export rezervacija, dark mode, prsten zdravlja, regresijsko testiranje |

---

## 6. Šta je završeno, djelimično završeno ili nije završeno

### Završeno (Done)
Ukupno **61 od 65 stavki** Product Backloga je implementirano i testirano. Sve planirane funkcionalnosti MVP-a su isporučene:

- Kompletna autentifikacija i autorizacija (JWT, RBAC, istek sesije, oporavak lozinke, verifikacija emaila)
- Upravljanje svim resursima: korisnici, kabineti, oprema, termini, tipovi opreme
- Kompletan rezervacijski tok: od kalendarskog pregleda do odobrenja, s validacijom konflikta
- Upravljanje kvarovima s automatskim otkazivanjem rezervacija
- Sistem obavijesti (in-app i email) za sve ključne događaje
- Korisnički profil s nedavnim aktivnostima, sigurnosnim alertom i exportom podataka
- NFR testiranje: load test s 50 concurrent korisnika, ACID provjera

### Odgođeno / Nije završeno (Deferred)
Četiri stavke planirane u Release Planu nisu ušle u finalni opseg:

| Stavka | Razlog |
|--------|--------|
| **Audit log prijava** (evidentiranje uspješnih/neuspješnih pokušaja prijave) | Deprioritizovan u finalnim sprintovima zbog kapaciteta tima |
| **Ocjena studentima** (profesor dodjeljuje ocjene unutar sistema) | Nije bio dio dogovorenog MVP-a; odgođeno za buduće verzije |
| **Recenzije na opremu** (komentari profesora i tehničara na opremu) | Odgođeno za buduće verzije |
| **Zasebna To-Do sekcija za tehničara** (workflow rješavanja kvara korak-po-korak) | Bazni workflow kvara završen u Sprint 9; napredna To-Do sekcija nije implementirana |

---

## 7. Glavne tehničke odluke

**JWT autentifikacija s kratkim istek tokenima**  
Odlučeno je da se koristi stateless JWT autentifikacija umjesto session-based pristupa, čime se izbjegava potreba za centralizovanim session store-om i olakšava horizontalno skaliranje. Token sadrži ulogu korisnika, što omogućava brzu RBAC provjeru na svakom API pozivu bez dodatnih upita bazi.

**Arhiviranje opreme umjesto trajnog brisanja**  
Trajno brisanje opreme iz sistema odbačeno je rano u razvoju jer bi uništilo referentni integritet - historija rezervacija, kvarovi i audit podaci ostali bi bez veze s opremom. Implementirano je soft-delete rješenje gdje arhivirana oprema gubi vidljivost u aktivnim listama ali zadržava sve relacije u bazi.

**Docker Compose za lokalni razvoj i CI/CD pipeline**  
Tim je od Sprinta 5 koristio Docker Compose za standardizaciju razvojnog okruženja, čime su eliminisane razlike između mašina članova tima . CI/CD pipeline postavljen istovremeno osigurao je automatsko pokretanje testova na svakom push-u.

**Automatska promjena statusa opreme pri prijavi kvara**  
Odlučeno je da prijava kvara ne zahtijeva dodatnu potvrdu tehničara da bi se oprema označila kao neispravna - status se mijenja odmah. Ovo je svjesna odluka u korist brzine zaštite korisnika od rezervacije neispravne opreme, nauštrb scenarija gdje bi prijava bila greškom podnesena.

**Soft limit za aktivne zahtjeve po studentu**  
Umjesto da se limitiranje aktivnih zahtjeva konfigurira po kabinetu ili terminu, implementiran je globalni limit po studentu. Ovo je jednostavnije rješenje koje osigurava pravedniju raspodjelu termina bez kompleksnog konfiguracionog interfejsa za administratore.

**Email obavijesti uvjetovane verifikacijom adrese**  
Sve email funkcionalnosti (podsjetnici, sigurnosni alertovi, notifikacije o zahtjevima) dostupne su isključivo korisnicima s verifikovanom email adresom. In-app obavijesti dostupne su uvijek. Ovo je odluka u korist pouzdanosti isporuke i izbjegavanja slanja poruka na nevalidne adrese.

---

## 8. Najveći problemi tokom razvoja i način rješavanja

**Problem 1: Preopterećenost Sprinta 6**  
Sprint 6 bio je najzahtjevniji u projektu - 8 članova paralelno radilo na CRUD operacijama za korisnike, kabinete, opremu i termine. Inicijalna integracija komponenti pokazala je neusklađenosti u API kontraktima (npr. različita imenovanja polja između frontenda i backenda).  
*Rješenje:* Jedan član tima (M8) bio je dedikovan isključivo integracijskim testovima i fungirao je kao živa dokumentacija API kontrakta. Prošlo se na striktno imenovanje polja dogovoreno unaprijed u zajedničkom API spec dokumentu.

**Problem 2: Race condition pri paralelnim rezervacijama**  
Tokom testiranja Sprinta 8 otkriveno je da istovremeni zahtjevi dva studenta za isti termin mogu oba proći validaciju i kreirati duplu rezervaciju.  
*Rješenje:* Implementirani su database locks na nivou transakcije pri potvrdi rezervacije (NFR-16). Validacija dostupnosti termina i upis rezervacije izvršavaju se unutar iste ACID transakcije, što eliminira race condition.

**Problem 3: Kompleksnost workflow-a otkazivanja pri kvaru opreme**  
Automatsko otkazivanje svih budućih rezervacija pri prijavi kvara pokazalo se kompleksnijim nego što je inicijalno procijenjeno - trebalo je obraditi rubne slučajeve poput termina koji su već počeli, rezervacija s djelimično neispravnom opremom i redoslijeda slanja obavijesti.  
*Rješenje:* Workflow je razrađen iterativno kroz Sprint 9. Odlučeno je da se otkazuju samo buduće rezervacije (ne oni termini koji su već u toku), a obavijesti se šalju tek nakon što je transakcija otkazivanja uspješno potvrđena u bazi.

**Problem 4: Koordinacija tima na 8 članova**  
S timom od 8 osoba, git konflikti i nedovoljna koordinacija bili su česti, posebno u Sprintu 6 i 7 kada se radilo na međuzavisnim komponentama.  
*Rješenje:* Uvedena je praksa kratkih dnevnih sinhronizacija i jasnih branch strategija (feature branch po user storiju, merge request s code reviewom od najmanje dva člana tima). Sporovi oko tehničkih odluka rješavani su glasanjem prostom većinom.

**Problem 5: Testiranje email funkcionalnosti u razvojnom okruženju**  
Testiranje slanja emailova (verifikacija, podsjetnici, alertovi) otežano je u lokalnom Docker okruženju jer direktno slanje na realne adrese nije praktično u razvoju.  
*Rješenje:* Korišten je mail stub servis u razvojnom okruženju koji hvata sve odlazne emailove i prikazuje ih u web interfejsu bez stvarnog slanja, čime je testiranje email sadržaja i tokova moglo biti potpuno automatizovano.

---

## 9. Šta bi tim unaprijedio da se projekat nastavlja

**Zasebni workflow rješavanja kvara za tehničara**  
Trenutno tehničar ažurira status opreme direktno, bez strukturiranog toka rada. U sljedećoj iteraciji implementirala bi se To-Do lista kvarova za tehničara s koracima: *Preuzeto → U popravci → Vraćeno u sistem*, što bi dalo bolji uvid u trajanje i troškove popravki.

**Audit log prijava i akcija korisnika**  
Sistem trenutno ne bilježi ko je kada i šta radio (osim nedavnih aktivnosti na profilu koje su ograničene). Potpuni audit log s filterima i exportom bio bi neophodan za produkcijsku upotrebu, posebno zbog sigurnosnih i compliance zahtjeva.

**Sistem ocjenjivanja i praćenja pohađanja**  
Profesor trenutno nema mogućnost bilježenja prisutnosti ili dodjele ocjena unutar sistema. Ovo bi bila prirodna nadogradnja koja bi povezala laboratorijske rezervacije s akademskim evidencijama.

**Recenzije i komentari na opremu**  
Profesori i tehničari bi imali koristi od mogućnosti ostavljanja komentara na opremu - npr. napomene o specifičnostima upotrebe, skrivenim kvarovima ili preporukama za kalibraciju, što bi dopunilo formalnu dokumentaciju (PDF upute) neformalnim iskustvenim znanjem.

**Naprednija analitika i izvještaji**  
Trenutni export rezervacija u CSV/PDF bazičan je. Proširenje bi uključivalo dashboard s grafovima zauzetosti po kabinetu i periodu, statistiku kvarova po tipu opreme i izvještaj o studentskoj aktivnosti, korisno za planiranje nabavki i radnog vremena.

**Mobilna responzivnost i PWA podrška**  
Aplikacija je responzivna na širini od 360px do 1920px, ali nije optimizovana kao Progressive Web App. Dodavanje PWA manifesta i service workera omogućilo bi instalaciju na mobilne uređaje i osnovnu offline podršku (pregled vlastitih rezervacija bez konekcije).
