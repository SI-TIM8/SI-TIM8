# Architecture Overview (Pregled arhitekture)

## Kratak opis arhitektonskog pristupa

Sistem za upravljanje laboratorijskom opremom i terminima zasnovan je na klasičnoj četveroslojnoj (layered) arhitekturi unutar API-First pristupa. Backend je potpuno odvojen od frontenda, što omogućava nezavisni razvoj i veću fleksibilnost sistema.

Arhitektura se oslanja na strogu separaciju odgovornosti između slojeva kako bi se kompleksna poslovna pravila (posebno vezana za korisničke uloge i dozvole) mogla mijenjati bez utjecaja na ostale dijelove sistema, uključujući bazu podataka.

---

## Glavne komponente sistema (Moduli)

Sistem je podijeljen na četiri ključna funkcionalna modula:

### 1. Identity modul (Autentifikacija i autorizacija)

Ovaj modul upravlja identitetom korisnika i njihovim pravima pristupa.

**Odgovornosti:**
- Autentifikacija korisnika (login)
- Generisanje JWT tokena
- Autorizacija putem Role-Based Access Control (RBAC)
- Upravljanje korisnicima i rolama

**Način rada:**
Nakon uspješne prijave, korisniku se izdaje JWT token koji sadrži identifikacione podatke i rolu korisnika. Taj token se šalje uz svaki naredni zahtjev i koristi se za provjeru dozvola pristupa.

---

### 2. Inventory modul (Upravljanje opremom)

Ovaj modul upravlja svim resursima unutar sistema.

**Odgovornosti:**
- Kreiranje, izmjena i brisanje opreme
- Praćenje statusa opreme (npr. dostupna, neispravna)
- Evidencija lokacije opreme (kabineti)

**Interakcija:**
- Admin dodaje i upravlja opremom
- Tehničar ažurira stanje i lokaciju
- Ostali korisnici imaju read-only pristup

---

### 3. Scheduling modul (Rezervacije i termini)

Ovo je najkompleksniji modul jer implementira ključnu poslovnu logiku sistema.

**Odgovornosti:**
- Upravljanje terminima i rasporedima
- Provjera konflikata (preklapanja termina)
- Upravljanje dostupnošću kabineta

**Poslovna pravila:**
- Nije dozvoljeno rezervisati isti kabinet u istom vremenu
- Termin mora biti unutar definisanog radnog vremena
- Postoje ograničenja broja učesnika po terminu

---

### 4. Workflow modul (Zahtjevi i notifikacije)

Ovaj modul upravlja komunikacijom između korisnika i procesima unutar sistema.

**Odgovornosti:**
- Upravljanje zahtjevima (npr. prijava kvarova)
- Notifikacije (real-time ili asinhrone)
- Upravljanje statusima procesa

**Primjeri:**
- Student šalje zahtjev → profesor dobija notifikaciju  
- Profesor prijavljuje kvar → tehničar obrađuje zahtjev  
- Status opreme se automatski ažurira kroz workflow  

---

## Odgovornosti komponenti (Slojevi)

### 1. Presentation layer (API sloj)

- Prima HTTP zahtjeve  
- Validira ulazne podatke  
- Vrši autentifikaciju korisnika (JWT)  
- Formatira odgovore  

**Napomena:** Ne sadrži poslovnu logiku.

---

### 2. Application layer (Service sloj)

- Orkestrira izvršavanje korisničkih zahtjeva  
- Implementira autorizaciju (RBAC)  
- Koordinira rad između domain i data sloja  

**Primjer:**  
Provjera da li korisnik ima pravo rezervisati termin i da li su zadovoljeni svi uslovi.

---

### 3. Domain layer (Poslovna logika)

- Sadrži poslovne entitete (User, Equipment, Reservation)  
- Implementira ključna pravila sistema:
  - Termin ne može biti u prošlosti  
  - Ne smije doći do konflikta termina  
  - Oprema mora biti dostupna za rezervaciju  

**Napomena:** Ovaj sloj ne zna ništa o HTTP-u niti o bazi podataka.

---

### 4. Data access layer (Infrastructure sloj)

- Komunikacija sa bazom podataka  
- CRUD operacije  
- Mapiranje između domenskih modela i baze  

**Dodatne odgovornosti:**
- Heširanje lozinki  
- Upravljanje upitima i performansama  

---

## Tok podataka i interakcija

### Tipičan tok zahtjeva:

1. Klijent šalje HTTP zahtjev  
2. Presentation layer validira podatke i autentificira korisnika  
3. Application layer obrađuje zahtjev  
4. Domain layer primjenjuje poslovna pravila  
5. Data layer izvršava operacije nad bazom  
6. Odgovor se vraća nazad klijentu  

---

### Primjer: prijava na termin

1. Student šalje zahtjev  
2. Sistem provjerava kapacitet i konflikt termina  
3. Ako su uslovi zadovoljeni → rezervacija se upisuje u bazu  

---

## Ključne tehničke odluke

### API-First pristup
Backend je nezavisan od frontenda, što omogućava razvoj više klijenata (web, mobilni).

---

### RBAC (Role-Based Access Control)
Autorizacija se implementira u Application sloju kako bi se centralizovala pravila pristupa.

---

### DTO (Data Transfer Objects)
Slojevi komuniciraju putem DTO objekata kako bi se:
- sakrili interni podaci  
- kontrolisao izlaz prema klijentu  
- smanjila zavisnost između slojeva  

---

### JWT autentifikacija
JWT token se koristi za autentifikaciju i autorizaciju korisnika.

---

### Razdvajanje frontend/backend
Omogućava:
- nezavisan razvoj  
- lakše testiranje  
- veću fleksibilnost sistema  

---

## Tehnološki stack

- Backend: ASP.NET Core  
- Frontend: React  
- Baza: PostgreSQL  

---

## Ograničenja i rizici arhitekture

- Kompleksnost autorizacije (RBAC pravila mogu postati teško održiva)  
- Veći overhead zbog slojevite arhitekture  
- Teže debugovanje zbog više slojeva  
- Domain sloj može postati preopterećen poslovnom logikom  
- Performanse mogu biti pogođene dodatnim mapiranjima  

---

## Otvorena pitanja

1. Koji algoritam koristiti za hashiranje lozinki (npr. bcrypt, Argon2)?  
2. Da li implementirati refresh token rotaciju?  
3. Kako riješiti konkurentne rezervacije (optimistic vs pessimistic locking)?  
4. Da li koristiti centralizovani error handling?  
5. Kako riješiti notifikacije (real-time vs async queue)?  

---

## Prijedlog razvoja (iterativni pristup)

- **Sedmica 1:** Setup arhitekture + autentifikacija  
- **Sedmica 2:** Inventory modul  
- **Sedmica 3:** Scheduling modul  
- **Sedmica 4:** Workflow modul + notifikacije  
