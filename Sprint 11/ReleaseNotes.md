## Release Notes

**Projekat:** LABsistem  
**Verzija izdanja:** Finalna MVP verzija  
**Period razvoja:** Sprint 1 - Sprint 11  
**Akademska godina:** 2025/2026

---

## 1. Pregled izdanja

Ovo izdanje predstavlja finalnu MVP verziju sistema **LABsistem**, web aplikacije za upravljanje laboratorijskim resursima, kabinetima, opremom i rezervacijama termina. Sistem je razvijen kao centralizovana platforma koja podržava rad više korisničkih uloga i digitalizuje rezervaciju laboratorija, evidenciju stanja opreme, obradu kvarova i komunikaciju između korisnika.

Finalna verzija obuhvata kompletan tok rada od autentifikacije korisnika, preko upravljanja resursima i rezervacijama, do obavijesti, korisničkog profila i naprednih pomoćnih funkcionalnosti.

---

## 2. Ključne isporučene funkcionalnosti

### 2.1 Autentifikacija i sigurnost
- Prijava i odjava korisnika uz JWT autentifikaciju.
- Role-based access control za administratora, profesora/asistenta, studenta i laboratorijskog tehničara.
- Automatski istek sesije i zaštita ruta.
- Oporavak lozinke putem email linka.
- Verifikacija email adrese.
- Obavezna promjena lozinke pri prvom loginu za korisnike koje kreira administrator.
- Sigurnosni email alert pri promjeni lozinke ili email adrese.

### 2.2 Upravljanje korisnicima i resursima
- Kreiranje, aktivacija, deaktivacija, pretraga i filtriranje korisnika.
- Upravljanje kabinetima, radnim vremenom i blokadama kabineta.
- CRUD upravljanje laboratorijskom opremom i tipovima opreme.
- Upravljanje statusima opreme i dostupnošću za korištenje.
- Arhiviranje opreme umjesto trajnog brisanja.
- Dodavanje PDF dokumentacije i URL uputstava uz opremu.
- Pregled detalja kabineta zajedno sa pripadajućom opremom.

### 2.3 Rezervacije i termini
- Kalendarski pregled slobodnih termina po kabinetu.
- Podnošenje zahtjeva za rezervaciju uz odabir kabineta, termina i opreme.
- Validacija konflikta i sprječavanje vremenskog preklapanja termina.
- Pregled zahtjeva za rezervaciju od strane profesora/asistenata.
- Odobravanje i odbijanje zahtjeva uz komentar.
- Ograničenje broja aktivnih zahtjeva po studentu.
- Pregled vlastitih rezervacija i zahtjeva na jednom mjestu.
- Otkazivanje odobrene rezervacije i povlačenje zahtjeva na čekanju.
- Filtriranje i export rezervacija u CSV i PDF formatu.

### 2.4 Upravljanje kvarovima i stanjem opreme
- Prijava kvara opreme od strane profesora.
- Automatska promjena statusa opreme u neispravnu nakon prijave kvara.
- Obrada prijavljenih kvarova od strane tehničara.
- Automatsko otkazivanje budućih rezervacija pogođenih kvarom.
- Slanje obavijesti korisnicima čije su rezervacije pogođene kvarom.
- Vizualni prikaz zdravlja opreme kroz dashboard komponentu.

### 2.5 Obavijesti, profil i korisničko iskustvo
- In-app obavijesti za ključne događaje u sistemu.
- Email obavijesti za korisnike sa verifikovanom email adresom.
- Podsjetnici prije termina.
- Pregled i uređivanje korisničkog profila.
- Prikaz nedavnih aktivnosti na profilu.
- Dark/Light mode sa pamćenjem korisničkog izbora.
- Dugme za povratak na vrh stranice.

---

## 3. Poslovna vrijednost izdanja

Finalno izdanje omogućava:
- centralizovano upravljanje laboratorijskim resursima
- jasnu podjelu odgovornosti prema korisničkim ulogama
- pouzdaniji proces rezervacije i odobravanja termina
- sistemsku evidenciju stanja opreme i obradu kvarova
- bolju komunikaciju kroz in-app i email obavijesti

Na ovaj način LABsistem zamjenjuje neformalne i nepouzdane procese rezervacija strukturiranim i preglednim digitalnim rješenjem.

---

## 4. Tehnički sažetak izdanja

Sistem je realizovan kao web aplikacija sa React frontend dijelom, ASP.NET Core backendom i PostgreSQL bazom podataka. Pokretanje i isporuka sistema podržani su kroz Docker i CI/CD pipeline, dok je kvalitet funkcionalnosti provjeravan kroz unit, integracijske, E2E i odabrane NFR testove.

---

## 5. Napomena o opsegu

Ovo izdanje predstavlja finalnu MVP verziju projekta. Sve ključne funkcionalnosti planirane za osnovni tok rada sistema su implementirane, dok su pojedine napredne funkcionalnosti ostavljene za eventualne buduće iteracije.
