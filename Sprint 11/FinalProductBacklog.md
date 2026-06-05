# Finalni Product Backlog

> **Legenda statusa:**
> - `Done` – stavka implementirana i testirana
> - `Partially Done` – djelimično implementirano
> - `To Do` – nije početo
> - `Deferred` –  ostavljeno za budući rad

| ID | Naziv stavke | Kratak opis | Tip stavke | Prioritet | Procjena (SP) | Status | Sprint/Release | Napomena |
|----|--------------|-------------|------------|-----------|---------------|--------|----------------|----------|
| 01 | Definisati problem i ciljeve sistema | Razraditi problem koji sistem rješava i osnovne ciljeve proizvoda | research | High | 3 | Done | Sprint 1 | |
| 02 | Izraditi Product Vision | Definisati viziju sistema i vrijednost za korisnike | documentation | High | 5 | Done | Sprint 1 | |
| 03 | Identifikovati stakeholdere sistema | Prepoznati sve ključne stakeholder grupe | research | High | 3 | Done | Sprint 1 | |
| 04 | Izraditi Stakeholder Map | Vizualizacija stakeholdera i njihovih interesa | documentation | High | 3 | Done | Sprint 1 | |
| 05 | Napraviti početni Product Backlog | Izraditi početnu verziju backloga | documentation | High | 5 | Done | Sprint 1 | |
| 06 | Razraditi prioritetne user storyje | Definisati ključne user storyje za MVP | documentation | High | 5 | Done | Sprint 2 | |
| 07 | Definisati acceptance criteria | Napisati mjerljive kriterije za storyje | documentation | High | 5 | Done | Sprint 2 | |
| 08 | Uraditi prioritizaciju backloga | Sortirati backlog po poslovnoj vrijednosti | research | High | 3 | Done | Sprint 2 | |
| 09 | Definisati početne NFR zahtjeve | Identifikovati sigurnosne, performansne i UX zahtjeve | documentation | Medium | 3 | Done | Sprint 2 | |
| 10 | Izraditi Risk Register | Identifikovati rizike i plan mitigacije | documentation | Medium | 3 | Done | Sprint 3 | |
| 11 | Napraviti Domain Model / Use Case Model | Modelovati entitete i odnose sistema | documentation | High | 5 | Done | Sprint 3 | |
| 12 | Izraditi Architecture Overview | Definisati arhitekturu sistema | documentation | High | 5 | Done | Sprint 3 | |
| 13 | Pripremiti Test Strategy | Definisati pristup testiranju | documentation | Medium | 3 | Done | Sprint 3 | |
| 14 | Definisati Definition of Done | Dogovoriti kriterije završetka taska | documentation | Medium | 2 | Done | Sprint 4 | |
| 15 | Napraviti Initial Release Plan | Planirati inkremente i MVP | documentation | Medium | 3 | Done | Sprint 4 | |
| 16 | Razrada modela podataka sistema | Definisati strukturu baze i relacije između entiteta | documentation | Medium | 3 | Done | Sprint 3 | |
| 17 | Postavka tehničkog skeletona sistema | Inicijalizacija projekta, repozitorija i osnovne strukture sistema | technical task | High | 5 | Done | Sprint 4 | |
| 18 | Autentifikacija – Login i tokeni | Implementirati login formu i JWT token autentifikaciju | feature | High | 3 | Done | Sprint 5 | US03, US29 |
| 19 | Autentifikacija – Role based | Implementirati role-based access control (RBAC) za sve uloge | feature | High | 3 | Done | Sprint 5 | US30 |
| 20 | Autentifikacija – Logout i sesija | Implementirati odjavu i upravljanje sesijom | feature | High | 2 | Done | Sprint 5 | US31, US32 |
| 21 | Upravljanje korisnicima | Administrator kreira i upravlja korisnicima | feature | High | 5 | Done | Sprint 6 | US01, US02 |
| 22 | Upravljanje kabinetima | Administrator vrši dodavanje i upravljanje kabinetima | feature | High | 3 | Done | Sprint 6 | US22 |
| 23 | Upravljanje inventarom – CRUD opreme | Admin dodaje, uređuje i briše laboratorijsku opremu; vezuje opremu za kabinet | feature | High | 5 | Done | Sprint 6 | US06, US07 |
| 24 | Upravljanje inventarom – status dostupnosti | Administrator upravlja statusom dostupnosti laboratorijske opreme | feature | High | 4 | Done | Sprint 6 | US08 |
| 25 | Definisanje radnog vremena kabineta | Admin definiše radno vrijeme i trajanje termina | feature | High | 5 | Done | Sprint 6 | US17; na osnovu radnog vremena sistem pravi raspored termina |
| 26 | Upravljanje terminima | Tehničar definiše, mijenja i briše termine za pojedinačne laboratorije | feature | Medium | 3 | Done | Sprint 6 | US18, US19, US20 |
| 27 | Blokiranje kabineta | Administrator blokira korištenje kabineta u određenim periodima (praznici, čišćenje...) | feature | Medium | 3 | Done | Sprint 6 | US23 |
| 28 | Pregled termina – Kalendar po kabinetu | Prikaz kalendara slobodnih termina po kabinetu | feature | High | 3 | Done | Sprint 7 | US24 |
| 29 | Pregled termina – Dostupnost opreme | Prikaz dostupnosti opreme u realnom vremenu | feature | High | 3 | Done | Sprint 7 | US21 |
| 30 | Pregled termina – Kombinovani prikaz | UI za kombinirani prikaz termina i dostupnosti opreme | feature | High | 2 | Done | Sprint 7 | |
| 31 | Odabir termina i podnošenje rezervacije | Korisnik bira kabinet, slobodan termin i podnosi zahtjev za rezervaciju | feature | High | 3 | Done | Sprint 7 | US11; oprema se bira unutar jednog kabineta |
| 32 | Odabir opreme unutar rezervacije | Korisnik odabire željenu opremu iz kabineta kao dio zahtjeva za rezervaciju | feature | Medium | 2 | Done | Sprint 7 | US25 |
| 33 | Validacija konflikta termina | Sprječavanje rezervacija koje se vremenski preklapaju | technical task | High | 5 | Done | Sprint 8 | US26 |
| 34 | Pregled zahtjeva za rezervaciju (profesor/asistent) | Profesor/asistent vidi listu pristiglih zahtjeva s detaljima (termin, korisnik, oprema) | feature | High | 2 | Done | Sprint 8 | US15, US27; status rezervacije može se mijenjati do početka termina |
| 35 | Odobravanje / odbijanje rezervacije | Profesor/asistent odobrava ili odbija zahtjev uz komentar; status se mijenja do početka termina | feature | High | 3 | Done | Sprint 8 | US15 |
| 36 | Ažuriranje statusa opreme (tehničar) | Tehničar ažurira status opreme tokom obrade prijavljenog kvara i vraćanja opreme u ispravno stanje | feature | High | 3 | Done | Sprint 9 | US08 |
| 37 | Automatsko označavanje opreme kao neispravne | Sistem nakon prijave kvara automatski označava opremu kao neispravnu i prikazuje njen ažurirani status korisnicima | feature | Medium | 2 | Done | Sprint 9 | US28 |
| 38 | Ograničenje broja aktivnih zahtjeva po studentu | Sistem ograničava maksimalan broj aktivnih zahtjeva po studentu i prikazuje upozorenje kada se limit dostigne | feature | Medium | 2 | Done | Sprint 9 | US16; aktivni zahtjevi su statusi Na čekanju i Odobren |
| 39 | Pregled vlastitih rezervacija i zahtjeva | Student na jednom mjestu vidi svoje odobrene rezervacije i poslane zahtjeve | feature | Medium | 2 | Done | Sprint 9 | US13A |
| 40 | Filtriranje i pretraga opreme | Pretraga opreme po nazivu i kabinetu | feature | Medium | 3 | Done | Sprint 10 | US10 |
| 41 | Upravljanje korisničkim profilom | Korisnik može promijeniti lozinku i podatke | feature | Low | 3 | Done | Sprint 10 | US04, US46, US47 |
| 42 | Historija korištenja opreme | Tehničar i administrator imaju uvid u historiju korištenja opreme | feature | Low | 3 | Deferred | – | Nije u MVP |
| 43 | Prijava kvara | Profesor može prijaviti kvar opreme kako bi sistem vodio preciznu evidenciju ispravnosti i pokrenuo obradu kvara | feature | Medium | 3 | Done | Sprint 9 | US09; povezano sa stavkom 44 |
| 44 | Potvrda i obrada kvara | Tehničar obrađuje prijavu kvara i ažurira stanje opreme kroz dalji workflow rješavanja | feature | Medium | 3 | Done | Sprint 9 | Workflow završen unutar Sprint 9 toka (US09/US28) |
| 45 | Pretraga i filtriranje korisnika | Administrator može pretraživati korisnike po imenu, emailu i korisničkom imenu te filtrirati listu po ulozi i statusu aktivan/deaktiviran | feature | Medium | 3 | Done | Sprint 6 | US36; moguće je kombinovati više filtera istovremeno i resetovati prikaz |
| 46 | Verifikacija email adrese | Korisnik potvrđuje email adresu putem verifikacionog linka, uz mogućnost ponovnog slanja linka i jasan prikaz statusa verifikacije | feature | High | 3 | Done | Sprint 10 | US38; email funkcionalnosti zavise od verifikovane adrese |
| 47 | Obavezna promjena lozinke pri prvom loginu | Novi korisnik kojeg kreira administrator mora postaviti novu lozinku prije nastavka korištenja sistema | feature | High | 3 | Done | Sprint 9 | US47; ne odnosi se na seedovane korisnike |
| 48 | Podsjetnik prije termina | Sistem šalje podsjetnike prije termina kroz in-app kanal, a email samo korisnicima sa verifikovanom email adresom | feature | Medium | 3 | Done | Sprint 9 | US40; vrijeme slanja je konfigurabilno |
| 49 | Arhiviranje opreme umjesto trajnog brisanja | Tehničar ili administrator arhivira opremu, zadržava historiju i po potrebi vraća stavku iz arhive | feature | Medium | 2 | Done | Sprint 9 | US41; arhivirana oprema se ne prikazuje u standardnim aktivnim listama |
| 50 | Otkazivanje rezervacije i povlačenje zahtjeva | Student može otkazati odobrenu rezervaciju ili poništiti zahtjev koji je još na čekanju | feature | Medium | 3 | Done | Sprint 9 | US13B |
| 51 | Obavijesti korisnika | Korisnik prima obavijesti o terminima, zahtjevima i opremi | feature | Medium | 3 | Done | Sprint 9 | US40, US43, US44 |
| 52 | Detalji kabineta sa pregledom opreme | Profesor, tehničar i administrator mogu kliknuti na naziv kabineta i vidjeti detalje o kabinetu zajedno sa listom opreme | feature | Medium | 3 | Done | Sprint 9 | US45 |
| 53 | Dugme za povratak na vrh stranice | Korisnik dobija dugme koje se pojavljuje prilikom skrolovanja prema dolje i klikom ga vraća na vrh stranice | feature | Low | 1 | Done | Sprint 9 | |
| 54 | Oporavak lozinke putem emaila | Korisnik resetuje zaboravljenu lozinku putem jedinstvenog privremenog linka poslanog na email | feature | Medium | 3 | Done | Sprint 8 | US33; validacija tokena i nove lozinke |
| 55 | Upravljanje tipovima opreme | Tehničar dodaje, uređuje i briše kategorije laboratorijske opreme; lista tipova se automatski ažurira | feature | Medium | 3 | Done | Sprint 8 | US34/US37 |
| 56 | Dokumentacija i uputstva za opremu | Upload PDF uputstava (do 10MB) i unos URL linkova za video/protokole uz opremu; prikaz i preuzimanje na detaljima opreme | feature | Low | 2 | Done | Sprint 9 | US42; linkovi se validiraju na ispravan URL format |
| 57 | Prikaz nedavnih aktivnosti na profilu | Korisnik vidi listu svojih nedavnih aktivnosti u sistemu s naslovom, opisom i meta informacijama o radnji | feature | Medium | 2 | Done | Sprint 10 | US48 |
| 58 | Filtriranje i export rezervacija (profesor) | Profesor filtrira rezervacije po datumu i kabinetu te exportuje trenutno filtrirane podatke u CSV ili PDF | feature | Medium | 3 | Done | Sprint 10 | US49, US50; bosanski karakteri podržani u CSV |
| 59 | Sigurnosni email alert za promjene profila | Sistem šalje sigurnosni email kada korisniku bude promijenjen email ili lozinka; samo za verificirane korisnike | feature | Medium | 2 | Done | Sprint 10 | US51 |
| 60 | Dark/Light mode | Korisnik odabire željeni vizualni prikaz sistema; sistem pamti postavku bez potrebe za ponovnim podešavanjem | feature | Low | 2 | Done | Sprint 10 | US52 |
| 61 | Prsten zdravlja opreme | Vizualni dashboard s color-coded postotcima ispravne, neispravne i arhivirane opreme; dostupno tehničaru, profesoru i adminu | feature | Low | 2 | Done | Sprint 10 | US53 |
| 62 | Evidentiranje aktivnosti prijava (audit log) | Sistem bilježi korisničko ime i timestamp svake uspješne i neuspješne prijave; administrator može pregledati listu zapisa | feature | Low | 2 | Deferred | – | Planirano u slučaju povećanja broja sprintova kao dodatna funkcinalnost. |
| 63 | Ocjena studentima | Profesor daje, uređuje i briše ocjene studentima unutar sistema | feature | Low | 2 | Deferred | – | Planirano u slučaju povećanja broja sprintova kao dodatna funkcinalnost.|
| 64 | Recenzije na opremu | Profesori i tehničari ostavljaju komentare/recenzije na opremu | feature | Low | 1 | Deferred | – | Planirano u slučaju povećanja broja sprintova kao dodatna funkcinalnost.|
| 65 |Rješavanje kvara u To-Do sekciji tehničara | Zasebna To-Do sekcija u kojoj tehničar preuzima i rješava prijavljene kvarove te privremeno isključuje opremu iz sistema | feature | Medium | 3 | Deferred | – | Bazni workflow kvara završen u Sprint 9 |