# Product Vision

## Naziv projekta
Sistem za upravljanje laboratorijskom opremom

---

## Problem koji sistem rješava
U postojećim rješenjima upravljanje opremom često se vrši putem Excel tabela ili usmeno, što dovodi do:
- neefikasnog procesa upravljanja opremom
- čestih grešaka i neusklađenosti
- duplih rezervacija i konflikata među korisnicima
- nejasnog statusa opreme (ispravna / neispravna)
- nedostatka kontrole i centralizacije podataka

Ovi problemi uzrokuju gubitak vremena, smanjuju efikasnost rada i negativno utiču na izvođenje nastavnih i istraživačkih aktivnosti.

---

## Ciljni korisnici

### Studenti
- koriste opremu i vrše rezervacije
- potreban brz i jednostavan pristup
- uvid u dostupnost i ispravnost opreme
- pregled i otkazivanje vlastitih rezervacija

### Profesori i asistenti
- odobravaju ili odbijaju zahtjeve za rezervaciju
- nadgledaju upotrebu i sigurnost opreme
- mogu prijaviti kvar opreme tehničaru

### Laboratorijski tehničari
- održavanje opreme i ažuriranje njenog statusa
- obrada prijava kvarova
- praćenje ispravnosti i dostupnosti opreme

### Administratori sistema
- upravljanje korisnicima, kabinetima i opremom
- definisanje radnog vremena, termina i pravila korištenja
- osiguravanje stabilnosti i sigurnosti sistema

---

## Vrijednost sistema
- jasan i centralizovan pregled opreme
- stalni uvid u dostupnost i status
- smanjenje grešaka i konflikata (npr. duple rezervacije)
- bolja organizacija rada u laboratoriji
- optimalnije iskorištavanje opreme
- pouzdanije izvođenje nastavnih i istraživačkih aktivnosti

---

## Scope MVP verzije

### Ključne funkcionalnosti
- evidencija opreme po kabinetima
- pregled zauzeća opreme i termina
- podnošenje i otkazivanje rezervacija
- odobravanje rezervacija od strane profesora/asistenta
- upravljanje statusom opreme i obrada kvarova
- blokiranje kabineta u određenim periodima

### Osnovne sistemske funkcije
- login sistem zasnovan na ulogama (bez registracije)
- admin panel za kreiranje korisničkih naloga i upravljanje kabinetima

---

## Šta ne ulazi u MVP
- notifikacije (email / SMS obavijesti)
- napredna analitika (korištenje, zauzetost, kvarovi)
- historija korištenja opreme
- AI funkcionalnosti (predikcija kvarova, detekcija problema)
- eksterni autentifikacioni sistemi

---

## Ključna ograničenja
- vremensko ograničenje: maksimalno 13 sedmica
- sastanci tima: subotom
- backend tehnologija: C# i .NET
- ciljna platforma: web browser

---

## Pretpostavke
- članovi tima imaju osnovno znanje u C# i .NET
- administrator kreira korisničke naloge i definiše njihove uloge

---


