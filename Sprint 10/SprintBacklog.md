# Sprint Backlog – Sprint 10

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a | Opis | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|-|
| 1 | Verifikacija email adrese i status verifikacije | Implementiran je tok verifikacije email adrese sa slanjem verifikacionog linka, prikazom statusa na profilu i mogućnošću ponovnog slanja emaila. | US38 | Hamza Hadžić | Završeno | Email-zavisne funkcionalnosti dostupne su tek nakon verifikacije |
| 2 | Podsjetnici prije termina | Sistem šalje in-app podsjetnike prije termina, a email podsjetnike samo korisnicima sa verifikovanom email adresom. | US40 | Hamza Hadžić | Završeno | Vrijeme slanja je konfigurabilno, npr. 24h i 1h ranije |
| 3 | Prikaz i uređivanje profila korisnika | Korisnik na stranici "Moj profil" vidi osnovne podatke, status email verifikacije i može ažurirati ime i prezime, email adresu i korisničko ime. | US51, US52 | Emina Hamamdžić | Završeno | Prikaz uloge i statusa naloga je read-only |
| 4 | Promjena lozinke iz profila | Korisnik može iz profila otvoriti formu za promjenu lozinke i sačuvati novu lozinku uz odgovarajuće validacije. | US04 | Emina Hamamdžić | Završeno | Odvojeno od flowa prve prijave |
| 5 | Prikaz nedavnih aktivnosti | Na profilu je prikazana lista nedavnih aktivnosti relevantnih za prijavljenog korisnika. | US53 | Aner Atović | Završeno | Prikazuju se naslov, opis i meta informacije o aktivnosti |
| 6 | Sigurnosni email alert za promjene profila | Sistem šalje sigurnosni email alert kada korisniku bude promijenjen email ili lozinka. | US54 | Refik Mujčinović | Završeno | Email se šalje samo verifikovanim korisnicima |
| 7 | Filtriranje rezervacija po datumu i kabinetu | Omogućavanje profesoru lakši pregled rezervisanih termina i njihov eksport | US49, US50 | Alma Jusufbegović | Završeno | Fitritanje po datumu i kabinetu, eksport kao csv ili pdf file |
| 8 | Dodatno testiranje funkcionalnosti | US13B, US27, US31, US33, US38, US40 | Haris Macić, Haris Sadiković | Završeno | Testiranje dosad preskočenih funkcionalnosti |
| 9 | Dark/Light mode | US52 | Merima Glušac | Završeno | Dodavanje mogućnosti da korisnici odaberu svijetli ili tamni prikaz|
| 10 | Prsten zdravlja opreme | US53 | Merima Glušac | Završeno | Omogućava vizualni prikaz tehničaru, profesoru i adminu u postotak ispravne opreme, opreme u kvaru i na servisu, kao i uklonjene opreme u sistemu.|


## **Cilj sprinta:** Potpuniji korisnički profil, pouzdanija email komunikacija, popunjavanje detalja za bolje korisničko iskustvo

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US38 | Verifikacija email adrese | Korisnik potvrđuje svoju email adresu putem verifikacionog linka i dobija jasan prikaz statusa verifikacije. | Feature | 3 | High | Završeno |
| US40 | Podsjetnik prije termina | Student dobija podsjetnik prije rezervisanog termina kroz in-app kanal i email kada je adresa verifikovana. | Feature | 3 | Medium | Završeno |
| US46 | Prikaz profila | Korisnik pregledava svoje profilne podatke, status naloga i status email verifikacije na jednom mjestu. | Feature | 2 | Medium | Završeno |
| US47 | Uređivanje podataka na profilu | Korisnik ažurira svoje osnovne podatke na profilu uz validaciju unosa i trenutno osvježavanje prikaza. | Feature | 2 | Medium | Završeno |
| US04 | Promjena lozinke | Korisnik mijenja svoju lozinku iz profila kako bi dodatno osigurao nalog. | Feature | 3 | Medium | Završeno |
| US48 | Prikaz nedavnih aktivnosti | Korisnik vidi listu svojih nedavnih aktivnosti u sistemu radi boljeg pregleda i transparentnosti. | Feature | 2 | Medium | Završeno |
| US51 | Sigurnosni alert za promjene profila | Korisnik dobija sigurnosnu email obavijest kada dođe do promjene osjetljivih profilnih podataka. | Feature | 2 | Medium | Završeno |
| US52 | Dark/Light mode | Korisnik ima mogućnost da odabere željeni prikaz sistema. | Feature | 2 | Low | Završeno |
| US53 |  Prsten zdravlja opreme | Korisnik dobija uvid u vizuelni prikaz ispravne i neispravne opreme | Feature | 2 | Low | Završeno |

# Detaljni User Stories (US)

---

### US38 – Verifikacija email adrese

*Kao korisnik, želim potvrditi svoju email adresu putem verifikacionog linka, kako bi sistem znao da koristim validnu i dostupnu email adresu.*

**Acceptance Criteria:**

* Nakon kreiranja naloga ili promjene email adrese sistem šalje verifikacioni email sa jedinstvenim linkom.
* Klik na verifikacioni link potvrđuje email adresu korisnika.
* Sistem ne otkriva osjetljive podatke kroz verifikacioni link.
* Korisnik vidi jasan status da li je email verifikovan ili nije.
* Neverifikovan korisnik može ponovo zatražiti slanje verifikacionog emaila.

---

### US40 – Podsjetnik prije termina

*Kao student, želim dobiti podsjetnik prije rezervisanog termina, kako ne bih propustio laboratorijsku vježbu.*

**Acceptance Criteria:**

* Sistem šalje in-app i/ili email podsjetnik prije termina.
* Vrijeme podsjetnika je konfigurabilno, npr. 24h i 1h ranije.
* Podsjetnik sadrži datum, vrijeme i laboratoriju.
* Ne šalju se podsjetnici za otkazane termine.
* Email podsjetnik se šalje samo korisnicima sa verifikovanom email adresom, dok je in-app obavijest dostupna u svakom slučaju.

---

### US46 – Prikaz profila

*Kao korisnik, želim pregledati svoje profilne podatke, kako bih imao uvid u informacije povezane sa svojim nalogom.*

**Acceptance Criteria:**

* Korisnik može otvoriti stranicu profila iz navigacije ili menija naloga.
* Prikazuju se osnovni podaci kao što su ime i prezime, email, korisničko ime i uloga.
* Prikazuje se status naloga i status email verifikacije.
* Korisnik ne može vidjeti tuđe profilne podatke preko ovog prikaza.
* Ako podaci nisu dostupni, sistem prikazuje jasnu poruku o grešci.

---

### US47 – Uređivanje podataka na profilu

*Kao korisnik, želim izmijeniti svoje profilne podatke, kako bih održavao tačne i ažurne informacije o svom nalogu.*

**Acceptance Criteria:**

* Korisnik može izmijeniti dozvoljena polja profila.
* Sistem validira unesene podatke prije spremanja.
* Nakon uspješne izmjene prikazuje se potvrda o ažuriranju.
* Ažurirani podaci su odmah vidljivi na profilu.
* Ako korisnik promijeni email adresu, nova adresa se označava kao neverifikovana.

---

### US04 – Promjena lozinke

*Kao korisnik, želim promijeniti lozinku, kako bih osigurao svoj profil.*

**Acceptance Criteria:**

* Korisnik može otvoriti formu za promjenu lozinke iz svog profila.
* Unose se trenutna lozinka, nova lozinka i potvrda nove lozinke.
* Sistem validira ispravnost unosa i podudaranje potvrde.
* Nakon uspješne promjene korisnik dobija jasnu potvrdu.

---

### US48 – Prikaz nedavnih aktivnosti

*Kao korisnik, želim vidjeti svoje nedavne aktivnosti u sistemu, kako bih imao pregled posljednjih radnji vezanih za moj nalog i korištenje aplikacije.*

**Acceptance Criteria:**

* Profil prikazuje listu nedavnih aktivnosti korisnika.
* Aktivnosti uključuju naslov, opis i meta informacije o radnji.
* Prikazuju se samo aktivnosti relevantne za prijavljenog korisnika.
* Ako nema aktivnosti, prikazuje se odgovarajuća prazna poruka.

---

### US51 – Sigurnosni alert za promjene profila

*Kao korisnik, želim dobiti sigurnosni email kada dođe do promjene osjetljivih podataka na mom profilu, kako bih mogao reagovati ako promjena nije bila moja.*

**Acceptance Criteria:**

* Sistem šalje sigurnosni email kada korisniku bude promijenjen email ili lozinka.
* Obavijest sadrži tip promjene i vrijeme promjene.
* Email se šalje samo korisnicima sa verifikovanom email adresom.
* Ako korisnik nije napravio promjenu, obavijest sadrži jasnu preporuku za resetovanje lozinke.

---
### US-49 – Filtriranje rezervacija po datumu i kabinetu

*Kao profesor, želim filtrirati listu rezervacija po datumu i kabinetu, kako bih lakše pronašao relevantne termine.*

**Acceptance Criteria:**

* Dostupni su filteri za početni datum (Od), krajnji datum (Do) i kabinet.
* Dropdown za kabinet prikazuje samo kabinete koji postoje u učitanim rezervacijama.
* Filteri se mogu kombinovati istovremeno.
* Tabela se ažurira odmah pri promjeni bilo kojeg filtera.
* Dugme "Resetuj filtere" prikazuje se samo kada je barem jedan filter aktivan.
* Nakon resetovanja prikazuju se sve rezervacije.
* Ako nema rezultata za odabrane filtere, prikazuje se odgovarajuća poruka.

---

### US-50 – Export rezervacija i historije termina u PDF i CSV

*Kao profesor, želim exportovati listu rezervacija i historiju termina u PDF ili CSV format, kako bih mogao čuvati ili dijeliti podatke izvan aplikacije.*

**Acceptance Criteria:**

* Na stranici liste rezervacija i historije termina dostupna su dva dugmeta — Export CSV i Export PDF.
* Dugmad su vidljiva samo kada postoje podaci za export.
* Export uključuje samo trenutno filtrirane podatke, ne cijelu listu.
* CSV fajl se ispravno otvara u Excelu sa bosanskim karakterima.
* PDF fajl sadrži naslov, datum generisanja i tabelu sa svim relevantnim kolonama.

---

### US-52 – Dark/Light mode

*Kao korisnik, želim moći promijeniti izgled UI-a, kako bi si popravio korisničko iskustvo.*

**Acceptance Criteria:**

* Korisnik je prijavljen u sistem
* Ikona za mijenjanje mode-a je na vidljivom lako-uočljivoj poziciji.
* Korisnik odabere željeni mod koji sistem pamti bez potrebe za novim podešavanjem.

---

### US-53 – Prsten zdravlja opreme

*Kao tehničar/admin/profesor, želim imati uvid u količinu ispravne/neispravne opreme, kako bih imao bolji uvid u rad sistema.*

**Acceptance Criteria:**

* Korisnik je prijavljen u sistem.
* Na dashboard-u ima vizualni prikaz i procenat ispravne opreme
* Sve je color-coded u odnosu na status opreme.

---
