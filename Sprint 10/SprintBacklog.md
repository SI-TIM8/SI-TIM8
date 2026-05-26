# Sprint Backlog – Sprint 10

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a | Opis | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|-|
| 1 | Verifikacija email adrese i status verifikacije | Implementiran je tok verifikacije email adrese sa slanjem verifikacionog linka, prikazom statusa na profilu i mogućnošću ponovnog slanja emaila. | US46 | Hamza Hadžić | Završeno | Email-zavisne funkcionalnosti dostupne su tek nakon verifikacije |
| 2 | Podsjetnici prije termina | Sistem šalje in-app podsjetnike prije termina, a email podsjetnike samo korisnicima sa verifikovanom email adresom. | US50 | Hamza Hadžić | Završeno | Vrijeme slanja je konfigurabilno, npr. 24h i 1h ranije |
| 3 | Prikaz i uređivanje profila korisnika | Korisnik na stranici "Moj profil" vidi osnovne podatke, status email verifikacije i može ažurirati ime i prezime, email adresu i korisničko ime. | US51, US52 | Emina Hamamdžić | Završeno | Prikaz uloge i statusa naloga je read-only |
| 4 | Promjena lozinke iz profila | Korisnik može iz profila otvoriti formu za promjenu lozinke i sačuvati novu lozinku uz odgovarajuće validacije. | US04 | Emina Hamamdžić | Završeno | Odvojeno od flowa prve prijave |
| 5 | Prikaz nedavnih aktivnosti | Na profilu je prikazana lista nedavnih aktivnosti relevantnih za prijavljenog korisnika. | US53 | Aner Atović | Završeno | Prikazuju se naslov, opis i meta informacije o aktivnosti |

## **Cilj sprinta:** Potpuniji korisnički profil i pouzdanija email komunikacija

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US46 | Verifikacija email adrese | Korisnik potvrđuje svoju email adresu putem verifikacionog linka i dobija jasan prikaz statusa verifikacije. | Feature | 3 | High | Završeno |
| US50 | Podsjetnik prije termina | Student dobija podsjetnik prije rezervisanog termina kroz in-app kanal i email kada je adresa verifikovana. | Feature | 3 | Medium | Završeno |
| US51 | Prikaz profila | Korisnik pregledava svoje profilne podatke, status naloga i status email verifikacije na jednom mjestu. | Feature | 2 | Medium | Završeno |
| US52 | Uređivanje podataka na profilu | Korisnik ažurira svoje osnovne podatke na profilu uz validaciju unosa i trenutno osvježavanje prikaza. | Feature | 2 | Medium | Završeno |
| US04 | Promjena lozinke | Korisnik mijenja svoju lozinku iz profila kako bi dodatno osigurao nalog. | Feature | 3 | Medium | Završeno |
| US53 | Prikaz nedavnih aktivnosti | Korisnik vidi listu svojih nedavnih aktivnosti u sistemu radi boljeg pregleda i transparentnosti. | Feature | 2 | Medium | Završeno |

# Detaljni User Stories (US)

---

### US46 – Verifikacija email adrese

*Kao korisnik, želim potvrditi svoju email adresu putem verifikacionog linka, kako bi sistem znao da koristim validnu i dostupnu email adresu.*

**Acceptance Criteria:**

* Nakon kreiranja naloga ili promjene email adrese sistem šalje verifikacioni email sa jedinstvenim linkom.
* Klik na verifikacioni link potvrđuje email adresu korisnika.
* Sistem ne otkriva osjetljive podatke kroz verifikacioni link.
* Korisnik vidi jasan status da li je email verifikovan ili nije.
* Neverifikovan korisnik može ponovo zatražiti slanje verifikacionog emaila.

---

### US50 – Podsjetnik prije termina

*Kao student, želim dobiti podsjetnik prije rezervisanog termina, kako ne bih propustio laboratorijsku vježbu.*

**Acceptance Criteria:**

* Sistem šalje in-app i/ili email podsjetnik prije termina.
* Vrijeme podsjetnika je konfigurabilno, npr. 24h i 1h ranije.
* Podsjetnik sadrži datum, vrijeme i laboratoriju.
* Ne šalju se podsjetnici za otkazane termine.
* Email podsjetnik se šalje samo korisnicima sa verifikovanom email adresom, dok je in-app obavijest dostupna u svakom slučaju.

---

### US51 – Prikaz profila

*Kao korisnik, želim pregledati svoje profilne podatke, kako bih imao uvid u informacije povezane sa svojim nalogom.*

**Acceptance Criteria:**

* Korisnik može otvoriti stranicu profila iz navigacije ili menija naloga.
* Prikazuju se osnovni podaci kao što su ime i prezime, email, korisničko ime i uloga.
* Prikazuje se status naloga i status email verifikacije.
* Korisnik ne može vidjeti tuđe profilne podatke preko ovog prikaza.
* Ako podaci nisu dostupni, sistem prikazuje jasnu poruku o grešci.

---

### US52 – Uređivanje podataka na profilu

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

### US53 – Prikaz nedavnih aktivnosti

*Kao korisnik, želim vidjeti svoje nedavne aktivnosti u sistemu, kako bih imao pregled posljednjih radnji vezanih za moj nalog i korištenje aplikacije.*

**Acceptance Criteria:**

* Profil prikazuje listu nedavnih aktivnosti korisnika.
* Aktivnosti uključuju naslov, opis i meta informacije o radnji.
* Prikazuju se samo aktivnosti relevantne za prijavljenog korisnika.
* Ako nema aktivnosti, prikazuje se odgovarajuća prazna poruka.

---
