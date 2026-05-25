# Sprint Backlog – Sprint 9

## Stavke sprint backloga

| ID | Naziv zadatka ili story-a | Opis | Povezani US | Odgovorna osoba ili osobe | Status | Napomena |
|-|-|-|-|-|-|-|
| 1 | Prijava i evidencija kvara opreme | Implementiran je tok prijave kvara od strane profesora, uključujući validaciju, unos komentara i automatsku promjenu statusa opreme na neispravnu. | US09, US28 | Emina Hamamdžić | Završeno | Status opreme se odmah ažurira i prikazuje korisnicima |
| 2 | Ograničenje aktivnih zahtjeva po studentu | Uveden je sistemski limit aktivnih studentskih zahtjeva uz validaciju i poruku upozorenja kada je limit dostignut. | US16 | Refik Mujčinović | Završeno | Aktivni zahtjevi su statusi Na čekanju i Odobren |
| 3 | Sigurnost prvog pristupa | Dodana je obavezna promjena lozinke pri prvom loginu za novokreirane korisnike koje kreira administrator. | US47 | Hamza Hadžić | Završeno | Ne odnosi se na seedovane korisnike |
| 4 | Pregled rezervacija i upravljanje zahtjevima studenta | Student na jednom ekranu vidi aktivne rezervacije i svoje zahtjeve, te može otkazati rezervaciju ili poništiti zahtjev na čekanju. | US13A, US13B | Hamza Hadžić | Završeno | Implementirani su odvojeni tabovi unutar ekrana "Moje rezervacije" |
| 5 | Arhiviranje opreme i arhivski filteri | Trajno brisanje opreme je zamijenjeno arhiviranjem, uz pregled arhivirane opreme, vraćanje iz arhive i prilagođene filtere na stranici opreme. | US49 | Hamza Hadžić | Završeno | Arhivirana oprema nije prikazana u standardnim aktivnim listama |
| 6 | Dokumentacija i digitalna uputstva za opremu | Omogućen je upload PDF uputstava (do 10MB) i unos URL linkova za video ili protokole pri kreiranju/uređivanju opreme, uz prikaz i preuzimanje na detaljima opreme. | US51 | Emina Hamamdžić | Završeno | Linkovi se validiraju na ispravan URL format |

## **Cilj sprinta:** Pouzdanije upravljanje opremom, sigurniji korisnički nalozi, veća samostalnost studenata i dostupnost edukacijskih materijala

| ID | Naziv storyja | Opis | Tip | Story Pts | Prioritet | Status |
|----|-------------|------|-----|-----------|-----------|--------|
| US09 | Prijava kvara | Profesor prijavljuje kvar opreme kako bi sistem vodio preciznu evidenciju ispravnosti. | Feature | 3 | Medium | Završeno |
| US28 | Automatsko povlačenje neispravne opreme | Sistem automatski označava opremu kao neispravnu nakon prijave kvara i prikazuje njen ažurirani status korisnicima. | Feature | 2 | Medium | Završeno |
| US16 | Limitiranje aktivnih zahtjeva po korisniku | Sistem ograničava maksimalan broj aktivnih zahtjeva po studentu i prikazuje poruku upozorenja kada se limit dostigne. | Feature | 2 | Medium | Završeno |
| US47 | Obavezna promjena lozinke pri prvom loginu | Novi korisnik kojeg kreira administrator mora promijeniti privremenu lozinku prije nastavka korištenja sistema. | Feature | 3 | High | Završeno |
| US13A | Pregled mojih rezervacija i zahtjeva | Student na jednom mjestu vidi svoje odobrene rezervacije i poslane zahtjeve. | Feature | 2 | Medium | Završeno |
| US13B | Otkazivanje rezervacije i povlačenje zahtjeva | Student može otkazati odobrenu rezervaciju ili poništiti zahtjev koji je još na čekanju. | Feature | 3 | Medium | Završeno |
| US49 | Arhiviranje opreme umjesto trajnog brisanja | Tehničar ili administrator arhivira opremu, zadržava historiju i po potrebi vraća stavku iz arhive. | Feature | 2 | Medium | Završeno |
| US51 | Dokumentacija i uputstva za opremu | Laboratorijski tehničar prilaže PDF uputstva, video linkove ili sigurnosne protokole uz specifičnu opremu radi pravilnog korištenja. | Feature | 2 | Low | Završeno |
| US50 | Obavijesti korisnicima o otkazivanju rezervacija zbog kvara | Sistem automatski obavještava pogođene korisnike kada se njihove buduće rezervacije otkažu zbog prijavljenog kvara opreme. | Feature | 2 | Medium | To Do |
| US51 | Općenite obavijesti o kvarovima i dostupnosti opreme | Ovlašteni korisnik šalje kratku općenitu obavijest svim korisnicima o važnim kvarovima, servisima ili promjenama dostupnosti opreme. | Feature | 2 | Medium | To Do |

# Detaljni User Stories (US)

---

### US09 – Prijava kvara

*Kao profesor, želim prijaviti kvar opreme, kako bi imali preciznu evidenciju ispravnosti.*

**Acceptance Criteria:**

* Forma za prijavu kvara je dostupna.
* Omogućen je unos detaljnog opisa problema.
* Kvar se evidentira u sistemu.
* Nakon prijave kvara status opreme se automatski mijenja u "neispravna".

---

### US28 – Automatsko povlačenje neispravne opreme

*Kao sistem, želim automatski označiti opremu kao neispravnu nakon prijave kvara i prikazati njen ažurirani status korisnicima, kako bi se spriječila dalja upotreba neispravne opreme i omogućilo njeno dalje tehničko rješavanje.*

**Acceptance Criteria:**

* Nakon prijave kvara status opreme se automatski mijenja u "neispravna".
* Ažurirani status opreme je vidljiv korisnicima u sistemu.
* Oprema ostaje u neispravnom ili servisnom stanju dok tehničar ili administrator ne promijeni njen status.
* Sistem omogućava tehničaru da kroz obradu kvara dalje upravlja statusom opreme do njenog vraćanja u ispravno stanje.

---

### US16 – Ograničenje broja aktivnih zahtjeva po studentu

*Kao sistem, želim ograničiti maksimalan broj aktivnih zahtjeva po studentu, kako bi raspodjela termina bila pravednija i kako pojedinačni korisnici ne bi zauzimali prevelik broj mjesta u sistemu.*

**Acceptance Criteria:**

* Sistem onemogućava slanje novog zahtjeva ako je student dosegao definisani limit aktivnih zahtjeva.
* Kao aktivni zahtjevi računaju se zahtjevi sa statusom "Na čekanju" i "Odobren".
* Student dobija jasnu poruku upozorenja kada pokuša poslati novi zahtjev nakon dostizanja limita.
* Nakon otkazivanja ili završetka postojećih zahtjeva student ponovo može slati nove zahtjeve u okviru dozvoljenog limita.

---

### US47 – Obavezna promjena lozinke pri prvom loginu

*Kao korisnik kojem je nalog kreirao administrator, želim biti primoran promijeniti privremenu lozinku pri prvom loginu, kako bi moj nalog bio sigurniji i poznat samo meni.*

**Acceptance Criteria:**

* Korisnik se može prijaviti sa privremenom lozinkom samo do ekrana za promjenu lozinke.
* Dok ne promijeni lozinku, korisnik ne može pristupiti ostatku aplikacije.
* Nova lozinka mora zadovoljiti postojeća sigurnosna pravila sistema.
* Nakon uspješne promjene lozinke, korisnik dobija normalan pristup aplikaciji.
* Sistem više ne traži obaveznu promjenu lozinke nakon što je korisnik jednom uspješno postavi.

---

### US13A – Pregled mojih rezervacija i zahtjeva

*Kao student, želim na jednom mjestu vidjeti svoje odobrene rezervacije i poslane zahtjeve, kako bih imao jasan pregled statusa svojih prijava i budućih termina.*

**Acceptance Criteria:**

* Student može otvoriti ekran "Moje rezervacije" iz postojećeg menija.
* Ekran prikazuje tab "Aktivne rezervacije" sa odobrenim budućim terminima studenta.
* Ekran prikazuje tab "Moji zahtjevi" sa studentskim zahtjevima koji nisu odobrene rezervacije.
* Za svaku stavku prikazuju se datum, vrijeme, kabinet, profesor i status.
* Student vidi samo svoje podatke i ne može pristupiti tuđim rezervacijama ili zahtjevima.

---

### US13B – Otkazivanje rezervacije i povlačenje zahtjeva

*Kao student, želim otkazati odobrenu rezervaciju ili poništiti zahtjev koji je još na čekanju, kako bih mogao odustati od termina bez posrednika i osloboditi mjesto drugima kada je to potrebno.*

**Acceptance Criteria:**

* Student može otkazati svoju buduću odobrenu rezervaciju iz taba "Aktivne rezervacije".
* Student može poništiti svoj zahtjev sa statusom "Na čekanju" iz taba "Moji zahtjevi".
* Nakon otkazivanja ili poništavanja status stavke se mijenja u "Otkazan".
* Otkazana odobrena rezervacija više se ne prikazuje kao aktivna rezervacija i termin se ponovo oslobađa u sistemu.
* Zahtjevi koji su već odbijeni, otkazani ili termini koji su već počeli ne mogu se ponovo otkazivati.
* Nakon uspješne akcije student dobija jasnu potvrdu u interfejsu.

---

### US49 – Arhiviranje opreme umjesto trajnog brisanja

*Kao tehničar, želim arhivirati opremu koja se više ne koristi, kako bih zadržao historiju bez prikaza u aktivnim listama.*

**Acceptance Criteria:**

* Oprema se može označiti kao arhivirana.
* Arhivirana oprema nije vidljiva u standardnim aktivnim listama.
* Historija evidencija i kvarova ostaje dostupna.
* Administrator ili tehničar može vratiti arhiviranu opremu.

---

### US51 – Dokumentacija i uputstva za opremu

*Kao laboratorijski tehničar, želim priložiti PDF uputstva, video linkove ili sigurnosne protokole uz specifičnu opremu, kako bi korisnici znali kako je pravilno i sigurno koristiti.*

**Acceptance Criteria:**

* Prilikom kreiranja ili uređivanja opreme, tehničar može uploadovati PDF fajl (maksimalno 10MB) ili unijeti eksterni URL.
* Student i profesor mogu vidjeti priloženu dokumentaciju i linkove na detaljnom prikazu pojedinačne opreme.
* Korisnici mogu direktno preuzeti PDF dokumentaciju ili klikom otvoriti eksterni video/sigurnosni protokol.
* Sistem onemogućava upload fajlova većih od 10MB i prikazuje odgovarajuću poruku validacije.
### US50 – Obavijesti korisnicima o otkazivanju rezervacija zbog kvara

*Kao korisnik koji ima rezervaciju, želim dobiti obavijest kada je moja rezervacija otkazana zbog kvara opreme, kako bih na vrijeme znao da termin više nije važeći.*

**Acceptance Criteria:**

* Kada prijava kvara automatski otkaže buduću rezervaciju, sistem kreira obavijest za pogođenog korisnika.
* Obavijest sadrži razlog otkazivanja i osnovne informacije o terminu ili opremi.
* Obavijest se prikazuje u postojećem zvonu za obavijesti.
* Broj nepročitanih obavijesti se povećava nakon kreiranja obavijesti.
* Korisnik može označiti obavijest kao pročitanu ili je obrisati iz svog prikaza.

---

### US51 – Općenite obavijesti o kvarovima i dostupnosti opreme

*Kao ovlašteni korisnik, želim poslati kratku općenitu obavijest svim korisnicima o kvarovima, servisima ili promjenama dostupnosti opreme, kako bi svi korisnici bili informisani o stanju sistema.*

**Acceptance Criteria:**

* Ovlašteni korisnik može unijeti tekst općenite obavijesti vezane za kvar, servis ili dostupnost opreme.
* Tekst obavijesti je obavezan i ograničen na razumnu dužinu.
* Sistem kreira nepročitanu obavijest za svakog aktivnog korisnika.
* Općenita obavijest se prikazuje u postojećem zvonu za obavijesti.
* Pošiljalac dobija potvrdu da je obavijest poslana.

---
