# Architecture Overview (Pregled arhitekture)

## Kratak opis arhitektonskog pristupa

Sistem za upravljanje laboratorijskom opremom i terminima zasnovan je na klasičnoj **četveroslojnoj (layered) arhitekturi** koja razdvaja odgovornosti između četiri horizontalna sloja: 
- **Prezentacijski sloj (Frontend)** - upravlja komunikacijom s korisnikom, prima HTTP zahtjeve i vraća odgovore.
- **Aplikacijski sloj** - upravlja izvršavanjem korisničkih zahtjeva i povezuje prezentacijski sloj s ostatkom sistema.
- **Domenski sloj** - sadrži poslovna pravila i logiku sistema.
- **Sloj za pristup podacima** - upravlja pristupom bazi podataka i omogućava čitanje i zapisivanje podataka.

Svaki sloj ima jasno definisanu odgovornost i komunicira isključivo sa susjednim slojevima. Ovakav pristup omogućava:
- **Razdvajanje odgovornosti** - svaki sloj ima svoju jasnu ulogu što sistem čini preglednijim i lakšim za održavanje.
- **Jednosmjerna zavisnost** - zavisnosti idu samo od viših ka nižim slojevima, što smanjuje međusobnu povezanost komponenti.
- **Lakše testiranje** - svaki sloj se može testirati zasebno.
- **Bolja preglednost sistema** - standardna struktura omogućava lakše snalaženje u projektu i brže razumijevanje koda.
- **Lakše održavanje i izmjene** - promjene su lokalizovane na određeni sloj, što smanjuje rizik od grešaka pri uvođenju novih funkcionalnosti.

---

## Glavne komponente sistema (Kompletna lista)

Sistem je organizovan u 4 sloja, a svaki od 8 entiteta iz baze podataka ima svoje komponente u svakom sloju kako bi se osigurala potpuna pokrivenost funkcionalnosti:

### 1. Presentation layer (API Kontroleri)
Odgovoran za korisnički interfejs i interakciju sa sistemom:
- Prikaz podataka (termini, oprema, zahtjevi)
- Unos podataka putem formi
- Slanje HTTP zahtjeva prema backendu
- Osnovna validacija unosa

### 2. Application layer (Servisi i API Kontroleri)
Sadrže poslovnu logiku, orkestraciju procesa i API endpointe:

#### API Kontroleri
- `KorisnikController` – Upravljanje profilima i autentifikacijom.
- `TerminController` – Rezervacije i pregled rasporeda.
- `ZahtjevController` – Obrada zahtjeva za termine ili opremu.
- `KabinetController` – Upravljanje podacima o laboratorijama.
- `ObjekatController` – Upravljanje lokacijama i radnim vremenom.
- `OpremaController` – Inventura i status laboratorijske opreme.
- `EvidencijaController` – Praćenje istorije i stanja opreme.
- `KorisnikObjekatController` – Upravljanje vezama između osoblja i objekata.

#### Servisi
- `KorisnikService` – Logika registracije, prijave i JWT generisanja.
- `TerminService` – Provjera dostupnosti i zakazivanje termina.
- `ZahtjevService` – Workflow odobravanja ili odbijanja zahtjeva.
- `KabinetService` – Organizacija kabineta unutar objekata.
- `ObjekatService` – Administracija radnog vremena i lokacija.
- `OpremaService` – Dodavanje opreme i dodjela kabinetima.
- `EvidencijaService` – Automatizovano bilježenje promjena statusa.
- `KorisnikObjekatService` – Logika dodjele prava pristupa objektima.

### 3. Domain layer (Entiteti)
Čiste klase koje predstavljaju podatke i pravila:
- `Korisnik` (ID, ImePrezime, Email, Username, Password, Uloga)
- `Kabinet` (ID, Naziv)
- `Oprema` (ID, Naziv, SerijskiBroj, Status, KabinetID, KreatorID)
- `Termin` (ID, VrijemePocetka, VrijemeKraja, Datum, Status, KabinetID, KreatorID)
- `Zahtjev` (ID, Status, Komentar, TerminID, OpremaID, StudentID)
- `Evidencija` (ID, Status, Komentar, OpremaID, KorisnikID, Datum)
- `Objekat` (ID, Lokacija, RadnoVrijeme)
- `Historija` (ID, Entitet, Akcija, Vrijeme, KorisnikID)
- `Historija_Termin` (ID, TerminID, Promjena, Vrijeme, KorisnikID)
- `Obavijest` (ID, Sadrzaj, Datum, KorisnikID, Status)
- `Recenzija` (ID, Ocjena, Komentar, Datum, KorisnikID)
- `Oprema_Recenzija` (ID, OpremaID, RecenzijaID)
- `Korisnik_Objekat` (ID, KorisnikID, ObjekatID)

### 4. Data access layer (Repozitoriji)
Zaduženi za direktne SQL operacije i komunikaciju sa bazom:
- `KorisnikRepository`
- `TerminRepository`
- `ZahtjevRepository`
- `KabinetRepository`
- `ObjekatRepository`
- `OpremaRepository`
- `EvidencijaRepository`
- `KorisnikObjekatRepository`
- `HistorijaRepository`
- `ObavijestRepository`
- `RecenzijaRepository`
  
---

## Odgovornosti komponenti

### Presentation layer
Frontend sloj je zadužen za prikaz podataka korisniku i interakciju sa sistemom. Korisnik putem ovog sloja šalje zahtjeve prema backendu i prima odgovore u obliku prikaza (UI).

### Application layer
Ovaj sloj upravlja izvršavanjem korisničkih akcija i povezuje preostale slojeve. Prima zahtjev od prezentacijskog sloja, poziva logiku iz domenskog sloja i repozitorije, te vraća finalni rezultat. Ovdje se vrši i generisanje tokena prilikom prijave korisnika.

API kontroleri unutar ovog sloja definišu rute (npr. `/termin`, `/termin/{id}`), vrše validaciju ulaznih podataka i autentifikaciju korisnika putem JWT tokena.

### Domain layer
Predstavlja srce sistema gdje su definisana sva ključna pravila:
- Provjera preklapanja termina u određenim kabinetima.
- Validacija dostupnosti i ispravnosti laboratorijske opreme.
- Provjera korisničkih ovlaštenja na osnovu uloga definisanih u sistemu.

### Data access layer
Odgovoran za interakciju s bazom podataka. Definiše strukturu entiteta prema tabelama u bazi. Važna odgovornost ovog sloja je i sigurno heširanje lozinki prije njihovog trajnog čuvanja.

---

## Tok podataka i interakcija

Zahtjev korisnika prolazi kroz slojeve u strogo definisanom redoslijedu.

**Standardni tok zahtjeva:**
1. Klijent šalje HTTP zahtjev na API rutu.
2. **Controller** prima zahtjev, provjerava ispravnost podataka i identitet korisnika.
3. **Service** preuzima zahtjev i koordinira rad između domenskih pravila i baze.
4. **Domain entitet** potvrđuje da su sva poslovna pravila zadovoljena.
5. **Repository** izvršava potrebnu akciju nad bazom podataka (upis ili čitanje).
6. Odgovor se vraća klijentu kroz iste slojeve obrnutim redoslijedom.

**Primjer procesa kreiranja rezervacije (Termin):**
1. Klijent šalje POST zahtjev na rutu `/termin` s podacima (Datum, VrijemePocetka, KabinetID)
2. `TerminController` radi validaciju formata i autentifikaciju.
3. `TerminService` poziva `Termin` domenski entitet za provjeru dostupnosti.
4. `Termin` provjerava poslovna pravila - preklapanje termina u istom kabinetu.
5. Ako je slobodno, `TerminRepository` zapisuje podatke u bazu.

---

## Ključne tehničke odluke


### Izbor četveroslojne (Layered) arhitekture
Odabrana arhitektura osigurava strogu podjelu između korisničkog interfejsa, poslovne logike i baze podataka.
- **Opravdanje:** S obzirom na to da sistem rukuje složenim entitetima poput termina i zahtjeva (tabela `Zahtjev` i `Termin`), razdvajanje logike omogućava da se pravila o konfliktima termina mijenjaju bez utjecaja na API rute ili strukturu tabela.
- **Prednost:** Olakšano testiranje (npr. testiranje validacije radnog vremena u `Objekat` entitetu bez pokretanja cijele baze podataka).

### Razvoj Web aplikacije (ASP.NET Core & React)
Sistem je dizajniran kao web aplikacija umjesto nativne mobilne aplikacije.
- **Opravdanje:** Upravljanje laboratorijskom opremom (`Oprema`) i evidencijom (`Evidencija`) obično obavljaju tehničari i administratori na desktop računarima unutar laboratorija. 
- **Tehnološki stack:** - **ASP.NET Core:** Pruža robusnu sigurnost za rukovanje osjetljivim podacima korisnika (polje `Password`).
  - **React:** Omogućava dinamičan prikaz kalendara termina i real-time ažuriranje statusa opreme.

### JWT Autentifikacija sa Refresh Tokenom
Sigurnost sistema zasnovana je na JSON Web Tokenima (JWT).
- **Opravdanje:** Kako sistem sadrži tabelu `Korisnik` sa ulogama (kroz `KorisnikID` u drugim tabelama), JWT omogućava da se identitet korisnika prenosi bez stanja na serveru (stateless), što ubrzava rad sistema.
- **Refresh Token:** Uveden kako bi se osigurao kontinuitet rada bez potrebe za čestim prijavljivanjem, što je ključno za administratore koji vrše dugotrajan unos opreme.

### Repository Pattern i Apstrakcija Baze Podataka
Sva komunikacija sa bazom (npr. tabele `Kabinet`, `Oprema`, `Termin`) vrši se isključivo putem repozitorija.
- **Opravdanje:** Centralizacijom upita sprječava se dupliranje koda. Na primjer, provjera da li je `OpremaID` ispravan vrši se na jednom mjestu.
- **Fleksibilnost:** Ako se u budućnosti odlučimo promijeniti bazu podataka (npr. prelazak na PostgreSQL), promjene će biti lokalizovane samo u Data Access sloju.

### Domain-Driven Validation (Validacija na nivou domene)
Poslovna pravila se ne nalaze u bazi podataka niti u kontrolerima, već u domenskim entitetima.
- **Primjer:** Pravilo da se `Termin` ne može zakazati izvan `RadnoVrijeme` definisanog u tabeli `Objekat` implementirano je u domenskom sloju. 
- **Sigurnost podataka:** Heširanje lozinki se vrši neposredno prije upisa, čime se osigurava da polje `Password` u bazi nikada ne bude u "plain text" formatu.

### Upotreba DTO (Data Transfer Objects) modela
Slojevi ne razmjenjuju direktne entitete baze podataka, već posebno skrojene DTO objekte.
- **Opravdanje:** Prilikom slanja podataka o korisniku (tabela `Korisnik`), DTO osigurava da polje `Password` nikada ne napusti server i ne bude poslato ka klijentu, čime se drastično povećava sigurnost.
---

## Ograničenja i rizici arhitekture

- **Dodatni napor (Overhead):** Prolaz kroz četiri sloja je potreban čak i za najjednostavnije prikaze podataka.
- **Složenost debugovanja:** Praćenje grešaka kroz više nivoa može zahtijevati više vremena.
- **Konkurentni pristup:** Potrebno je osigurati da dva korisnika ne mogu rezervisati isti kabinet u potpuno isto vrijeme.

---

## Otvorena pitanja

1. Koji će se specifičan algoritam koristiti za zaštitu (heširanje) lozinki?
2. Koliko dugo će važiti sigurnosni tokeni prije nego što se korisnik mora ponovo prijaviti?
3. Kako će sistem obavještavati korisnike o promjenama statusa njihovih zahtjeva u realnom vremenu?
4. Hoće li postojati centralizovan sistem za bilježenje grešaka (logging) radi lakšeg održavanja?
