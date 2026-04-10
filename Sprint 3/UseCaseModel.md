# Use Case Model

## UC01 – Prijava na sistem

**Akter:** Registrovani korisnik  
**Opis:** Korisnik se prijavljuje u sistem kako bi pristupio funkcionalnostima.  

**Preduslovi:**
- Korisnik ima validan nalog

**Glavni tok:**
1. Korisnik otvara stranicu za prijavu  
2. Unosi korisničko ime i lozinku  
3. Sistem validira podatke  
4. Sistem prijavljuje korisnika  

**Alternativni tokovi:**
- 3a. Pogrešni podaci → sistem prikazuje grešku  
- 3b. Korisnik ne postoji → sistem prikazuje poruku  
- 3c. Sistem nedostupan → prikazuje grešku  

**Ishod:**
- Korisnik je prijavljen u sistem  

---

## UC02 – Kreiranje korisnika

**Akter:** Administrator  
**Opis:** Administrator kreira novi korisnički nalog.  

**Preduslovi:**
- Administrator je prijavljen  

**Glavni tok:**
1. Administrator otvara formu  
2. Unosi podatke (korisničko ime, lozinka, uloga)  
3. Sistem validira podatke  
4. Sistem kreira korisnika  

**Alternativni tokovi:**
- 3a. Nevalidni podaci → greška  
- 3b. Korisničko ime već postoji → sistem odbija unos  

**Ishod:**
- Korisnik je kreiran  

---

## UC03 – Upravljanje kabinetima

**Akter:** Administrator  
**Opis:** Administrator upravlja kabinetima.  

**Preduslovi:**
- Administrator je prijavljen  

**Glavni tok:**
1. Otvara listu kabineta  
2. Dodaje ili uređuje kabinet  
3. Sistem sprema promjene  

**Alternativni tokovi:**
- 2a. Nevalidni podaci  
- 2b. Kabinet već postoji  

**Ishod:**
- Kabinet ažuriran  

---

## UC04 – Upravljanje opremom

**Akter:** Administrator / Tehničar  
**Opis:** Upravljanje opremom u sistemu.  

**Preduslovi:**
- Korisnik ima odgovarajuću ulogu  

**Glavni tok:**
1. Otvara listu opreme  
2. Dodaje ili uređuje opremu  
3. Sistem validira podatke  
4. Sistem sprema promjene  

**Alternativni tokovi:**
- 3a. Nevalidni podaci  
- 3b. Oprema već postoji  
- 3c. Nedozvoljena akcija  

**Ishod:**
- Oprema ažurirana  

---

## UC05 – Definisanje radnog vremena

**Akter:** Administrator  
**Opis:** Administrator definiše radno vrijeme kabineta.  

**Preduslovi:**
- Kabinet postoji  

**Glavni tok:**
1. Administrator bira kabinet  
2. Unosi radno vrijeme  
3. Sistem validira unos  
4. Sprema podatke  

**Alternativni tokovi:**
- 3a. Nevalidan unos

**Ishod:**
- Radno vrijeme postavljeno  

---

## UC06 – Kreiranje termina

**Akter:** Tehničar  
**Opis:** Tehničar kreira termine unutar radnog vremena.  

**Preduslovi:**
- Radno vrijeme definisano  

**Glavni tok:**
1. Tehničar bira kabinet  
2. Unosi termin  
3. Sistem provjerava radno vrijeme  
4. Sprema termin  

**Alternativni tokovi:**
- 3a. Termin van radnog vremena  
- 3b. Preklapanje termina  
- 3c. Oprema nije dostupna  

**Ishod:**
- Termin kreiran  

---

## UC07 – Pregled termina i opreme

**Akter:** Autentifikovani korisnik  
**Opis:** Pregled dostupnih termina i opreme.  

**Preduslovi:**
- Korisnik je prijavljen  

**Glavni tok:**
1. Otvara pregled  
2. Sistem prikazuje podatke  
3. Korisnik filtrira  

**Alternativni tokovi:**
- 2a. Nema dostupnih podataka  
- 3a. Nevalidan filter  

**Ishod:**
- Prikaz rezultata  

---

## UC08 – Profesor zauzima termin

**Akter:** Profesor / Asistent  
**Opis:** Profesor rezerviše termin.  

**Preduslovi:**
- Termin postoji  

**Glavni tok:**
1. Pregled termina  
2. Odabir termina  
3. Profesor postavlja maksimalan broj studenata za termin
4. Sistem rezerviše termin  

**Alternativni tokovi:**
- 4a. Termin već zauzet  

**Ishod:**
- Termin rezervisan  

---

## UC09 – Profesor otkazuje termin

**Akter:** Profesor / Asistent  
**Opis:** Otkazivanje termina.  

**Preduslovi:**
- Termin rezervisan  

**Glavni tok:**
1. Pregled termina  
2. Odabir termina  
3. Otkazivanje  

**Alternativni tokovi:**
- 2a. Termin ne postoji  

**Ishod:**
- Termin oslobođen  

---

## UC10 – Tehničar otkazuje termin

**Akter:** Tehničar  
**Opis:** Tehničar uklanja termin.  

**Preduslovi:**
- Termin postoji  

**Glavni tok:**
1. Pregled termina  
2. Otkazivanje termina  
3. Sistem briše termin  

**Alternativni tokovi:**
- 2a. Termin ne postoji  
- 2b. Termin već rezervisan  

**Ishod:**
- Termin uklonjen  

---

## UC11 – Slanje zahtjeva za rezervaciju opreme

**Akter:** Student  
**Opis:** Student šalje zahtjev za rezervaciju opreme u okviru odabranog termina.  

**Preduslovi:**
- Termin je rezervisan od strane profesora  
- Oprema je dostupna  

**Glavni tok:**
1. Student odabire termin  
2. Sistem prikazuje dostupnu opremu za taj termin  
3. Student odabire opremu  
4. Student šalje zahtjev za rezervaciju  
5. Sistem sprema zahtjev  

**Alternativni tokovi:**
- 2a. Nema dostupne opreme   
- 4a. Termin je popunjen  

**Ishod:**
- Zahtjev za rezervaciju opreme je poslan 

---

## UC12 – Pregled zahtjeva

**Akter:** Profesor  
**Opis:** Pregled zahtjeva studenata.  

**Preduslovi:**
- Postoje zahtjevi  

**Glavni tok:**
1. Otvara listu  
2. Pregleda zahtjeve  

**Alternativni tokovi:**
- 2a. Nema zahtjeva  

**Ishod:**
- Zahtjevi prikazani  

---

## UC13 – Odobravanje zahtjeva

**Akter:** Profesor  
**Opis:** Odobravanje zahtjeva.  

**Preduslovi:**
- Postoji zahtjev za rezervaciju  

**Glavni tok:**
1. Odabir zahtjeva  
2. Odobravanje  
3. Sistem ažurira status  

**Alternativni tokovi:**
- 1a. Zahtjev ne postoji  
- 2a. Termin pun u međuvremenu  

**Ishod:**
- Zahtjev odobren  

---

## UC14 – Odbijanje zahtjeva

**Akter:** Profesor  
**Opis:** Odbijanje zahtjeva.  

**Preduslovi:**
- Postoji zahtjev za rezervaciju  

**Glavni tok:**
1. Odabir zahtjeva  
2. Odbijanje  
3. Sistem ažurira status  

**Alternativni tokovi:**
- 1a. Zahtjev ne postoji  

**Ishod:**
- Zahtjev odbijen  
---

## UC15 – Pregled vlastitih zahtjeva

**Akter:** Student  
**Opis:** Student pregledava svoje zahtjeve.  

**Glavni tok:**
1. Otvara listu  
2. Sistem prikazuje status  

**Alternativni tokovi:**
- 2a. Nema zahtjeva  

**Ishod:**
- Status prikazan  

---

## UC16 – Otkazivanje zahtjeva

**Akter:** Student  
**Opis:** Student otkazuje zahtjev.  

**Glavni tok:**
1. Odabir zahtjeva  
2. Otkazivanje  

**Alternativni tokovi:**
- 1a. Zahtjev ne postoji  
- 1b. Zahtjev već obrađen  

**Ishod:**
- Zahtjev uklonjen  

---

## UC17 – Prijava kvara

**Akter:** Student / Profesor / Tehničar  
**Opis:** Prijava kvara opreme.  

**Glavni tok:**
1. Otvara formu  
2. Unosi opis  
3. Sistem kreira prijavu  
4. Sistem mijenja status opreme  

**Alternativni tokovi:**
- 2a. Nevalidan unos  
- 3a. Kvar već prijavljen  

**Ishod:**
- Kvar prijavljen  

---

## UC18 – Pregled kvarova

**Akter:** Tehničar  
**Opis:** Pregled kvarova.  

**Glavni tok:**
1. Otvara listu  
2. Pregleda kvarove  

**Alternativni tokovi:**
- 2a. Nema kvarova  

**Ishod:**
- Lista prikazana  

---

## UC19 – Obrada kvara

**Akter:** Tehničar  
**Opis:** Obrada prijavljenog kvara.  

**Preduslovi:**
- Postoji prijava kvara  

**Glavni tok:**
1. Tehničar pregledava kvar  
2. Ažurira status kvara  
3. Sistem evidentira promjene  

**Alternativni tokovi:**
- 2a. Kvar nije moguće odmah riješiti  

**Ishod:**
- Status kvara je ažuriran  

---
