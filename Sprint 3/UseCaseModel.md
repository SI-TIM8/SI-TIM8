# Use Case Model

## UC01 – Prijava na sistem

**Akter:** Registrovani korisnik  
**Opis:** Korisnik se prijavljuje u sistem kako bi pristupio funkcionalnostima.  

**Preduslovi:**
- Korisnik ima validan nalog

**Glavni tok:**
1. Korisnik otvara login stranicu  
2. Unosi email i lozinku  
3. Sistem validira podatke  
4. Sistem kreira sesiju i prijavljuje korisnika  

**Alternativni tokovi:**
- 3a. Pogrešni podaci → sistem prikazuje grešku  

**Ishod:**
- Korisnik je prijavljen u sistem  

---

## UC02 – Kreiranje korisnika

**Akter:** Administrator  
**Opis:** Administrator kreira novi korisnički nalog.  

**Preduslovi:**
- Administrator je prijavljen  

**Glavni tok:**
1. Administrator otvara formu za kreiranje korisnika  
2. Unosi podatke (ime, email, uloga)  
3. Sistem validira podatke  
4. Sistem kreira korisnika u bazi  

**Alternativni tokovi:**
- 3a. Nevalidni podaci → sistem prikazuje grešku  

**Ishod:**
- Novi korisnik je kreiran  

---

## UC03 – Upravljanje kabinetima

**Akter:** Administrator  
**Opis:** Administrator dodaje ili uređuje kabinete.  

**Preduslovi:**
- Administrator je prijavljen  

**Glavni tok:**
1. Administrator otvara listu kabineta  
2. Dodaje ili uređuje kabinet  
3. Sistem sprema promjene  

**Alternativni tokovi:**
- 2a. Nevalidni podaci → sistem prikazuje grešku  

**Ishod:**
- Kabinet je uspješno dodan ili izmijenjen  

---

## UC04 – Upravljanje opremom

**Akter:** Tehničar / Administrator  
**Opis:** Dodavanje, izmjena i brisanje opreme.  

**Preduslovi:**
- Korisnik ima odgovarajuću ulogu  

**Glavni tok:**
1. Otvara listu opreme  
2. Dodaje/uređuje opremu  
3. Sistem validira podatke  
4. Sistem sprema promjene  

**Alternativni tokovi:**
- 3a. Nevalidni podaci  

**Ishod:**
- Oprema je ažurirana u sistemu  

---

## UC05 – Definisanje termina i radnog vremena

**Akter:** Administrator / Tehničar  
**Opis:** Definisanje radnog vremena i termina.  

**Preduslovi:**
- Postoji kabinet  

**Glavni tok:**
1. Otvara postavke termina  
2. Unosi radno vrijeme  
3. Definiše termine  
4. Sistem generiše raspored  

**Alternativni tokovi:**
- 2a. Nevalidan unos  

**Ishod:**
- Termini su dostupni korisnicima  

---

## UC06 – Pregled slobodnih termina i opreme

**Akter:** Student  
**Opis:** Student pregleda dostupne termine i opremu.  

**Preduslovi:**
- Student je prijavljen  
- Termini postoje  

**Glavni tok:**
1. Student otvara pregled termina  
2. Sistem prikazuje slobodne termine  
3. Student filtrira rezultate  

**Alternativni tokovi:**
- 2a. Nema dostupnih termina  

**Ishod:**
- Student vidi dostupne opcije  

---

## UC07 – Podnošenje zahtjeva za rezervaciju

**Akter:** Student  
**Opis:** Student rezerviše termin i opremu.  

**Preduslovi:**
- Student je prijavljen  
- Termin je slobodan  

**Glavni tok:**
1. Student bira termin  
2. Sistem prikazuje dostupnu opremu  
3. Student bira opremu  
4. Potvrđuje rezervaciju  
5. Sistem validira podatke  
6. Sistem kreira zahtjev (Pending)  
7. Zahtjev se šalje na odobrenje  

**Alternativni tokovi:**
- 5a. Termin zauzet → greška  
- 5b. Prekoračen limit rezervacija  

**Ishod:**
- Zahtjev za rezervaciju je kreiran  

---

## UC08 – Odobravanje rezervacije

**Akter:** Profesor / Asistent  
**Opis:** Odobravanje ili odbijanje zahtjeva.  

**Preduslovi:**
- Postoji zahtjev  

**Glavni tok:**
1. Otvara listu zahtjeva  
2. Pregledava zahtjev  
3. Odabire approve/reject  
4. Sistem ažurira status  

**Alternativni tokovi:**
- 3a. Odbijanje uz komentar  

**Ishod:**
- Status rezervacije je ažuriran  

---

## UC09 – Pregled i otkazivanje rezervacija

**Akter:** Student  
**Opis:** Student vidi i otkazuje rezervacije.  

**Preduslovi:**
- Student ima aktivne rezervacije  

**Glavni tok:**
1. Otvara listu rezervacija  
2. Bira rezervaciju  
3. Klikne otkaži  
4. Sistem briše rezervaciju  

**Alternativni tokovi:**
- 3a. Termin već počeo → nije moguće otkazati  

**Ishod:**
- Termin je oslobođen  

---

## UC10 – Prijava kvara

**Akter:** Student / Profesor  
**Opis:** Prijava neispravne opreme.  

**Preduslovi:**
- Oprema postoji  

**Glavni tok:**
1. Otvara formu za kvar  
2. Unosi opis  
3. Sistem bilježi kvar  
4. Status opreme se mijenja  

**Alternativni tokovi:**
- 2a. Nevalidan unos  

**Ishod:**
- Kvar je evidentiran  

---

## UC11 – Obrada kvara

**Akter:** Tehničar  
**Opis:** Obrada prijavljenog kvara.  

**Preduslovi:**
- Postoji prijava kvara  

**Glavni tok:**
1. Tehničar pregledava kvar  
2. Ažurira status opreme  
3. Sistem evidentira promjene  

**Alternativni tokovi:**
- 2a. Kvar nije moguće odmah riješiti  

**Ishod:**
- Status opreme je ažuriran  

---
