# Domain Model

## Glavni entiteti

Sistem za upravljanje laboratorijskom opremom zasniva se na sljedećim glavnim entitetima:

- Korisnik  
- Kabinet  
- Oprema  
- Termin  
- Zahtjev  
- Evidencija  
- Objekat  
- Historija  
- Historija_Termin  
- Obavijest  
- Recenzija  
- Oprema_Recenzija  
- Korisnik_Objekat  

Ovi entiteti pokrivaju upravljanje korisnicima, kabinetima, opremom, terminima, rezervacijama, evidencijom aktivnosti, organizacijom prostora, kao i dodatnim funkcionalnostima poput obavještenja, historije i recenzija opreme.

---

## Ključni atributi

### 1. Korisnik  
Predstavlja korisnika sistema.

Ključni atributi:
- ImePrezime  
- Email  
- Username  
- Password  

---

### 2. Kabinet  
Predstavlja laboratorijski prostor.

Ključni atributi:
- Naziv  

---

### 3. Oprema  
Predstavlja laboratorijske uređaje i alate.

Ključni atributi:
- Naziv  
- SerijskiBroj  
- tipOpreme  

---

### 4. Termin  
Predstavlja vremenski period korištenja kabineta.

Ključni atributi:
- VrijemePocetka  
- VrijemeKraja  
- Datum  

---

### 5. Zahtjev  
Predstavlja zahtjev za rezervaciju.

Ključni atributi:
- Status  
- Komentar  

---

### 6. Evidencija  
Predstavlja zapis o stanju ili aktivnosti opreme.

Ključni atributi:
- Status  
- Komentar  

---

### 7. Objekat  
Predstavlja fizičku lokaciju.

Ključni atributi:
- Lokacija  
- RadnoVrijeme  

---

### 8. Historija  
Predstavlja zapis promjena ili aktivnosti.

Ključni atributi:
- Datum  

---

### 9. Historija_Termin  
Predstavlja vezu između historije i termina.

Ključni atributi:
- HistorijaID  
- TerminID  

---

### 10. Obavijest  
Predstavlja obavještenja vezana za termine.

Ključni atributi:
- Novosti  
- Dostupnost  

---

### 11. Recenzija  
Predstavlja ocjenu i komentar.

Ključni atributi:
- Komentar  
- Ocjena  

---

### 12. Oprema_Recenzija  
Veza između opreme i recenzije.

Ključni atributi:
- OpremaID  
- RecenzijaID  

---

### 13. Korisnik_Objekat  
Veza između korisnika i objekta.

Ključni atributi:
- KorisnikID  
- ObjekatID  

---

## Veze između entiteta

### Korisnik i Kabinet  
Korisnik upravlja kabinetima.  
**Veza:** 1 : N  

---

### Korisnik i Oprema  
Korisnik dodaje i upravlja opremom.  
**Veza:** 1 : N  

---

### Korisnik i Evidencija  
Korisnik vodi evidenciju.  
**Veza:** 1 : N  

---

### Korisnik i Termin  
Korisnik kreira termine.  
**Veza:** 1 : N  

---

### Korisnik i Objekat  
Veza se ostvaruje preko `Korisnik_Objekat`.  
**Veza:** M : N  

---

### Objekat i Kabinet  
Jedan objekat sadrži više kabineta.  
**Veza:** 1 : N  

---

### Kabinet i Oprema  
Jedan kabinet sadrži više komada opreme.  
**Veza:** 1 : N  

---

### Kabinet i Termin  
Jedan kabinet ima više termina.  
**Veza:** 1 : N  

---

### Termin i Zahtjev  
Jedan termin ima više zahtjeva.  
**Veza:** 1 : N  

---

### Kabinet i Zahtjev  
Jedan kabinet ima više zahtjeva.  
**Veza:** 1 : N  

---

### Evidencija i Oprema  
Jedan komad opreme ima više evidencija.  
**Veza:** 1 : N  

---

### Historija i Termin  
Veza se ostvaruje preko `Historija_Termin`.  
**Veza:** M : N  

---

### Termin i Obavijest  
Jedan termin može imati više obavijesti.  
**Veza:** 1 : N  

---

### Oprema i Recenzija  
Veza se ostvaruje preko `Oprema_Recenzija`.  
**Veza:** M : N  

---

## Poslovna pravila važna za model

1. Svaki korisnik mora imati tačno jednu ulogu u sistemu.  
2. Korisničke račune kreira administrator, a pristup sistemu je moguć samo registrovanim korisnicima.  
3. Jedan kabinet sadrži više komada opreme i više termina.  
4. Svaki kabinet mora pripadati tačno jednom objektu.  
5. Svaki komad opreme mora pripadati tačno jednom kabinetu.  
6. Termin može biti definisan samo unutar radnog vremena kabineta.  
7. Termin može biti blokiran, a za takav termin nije dozvoljeno slanje zahtjeva za rezervaciju.  
8. Zahtjev za rezervaciju može se kreirati samo za termin čije vrijeme početka nije u prošlosti.  
9. Svaki zahtjev mora biti vezan za tačno jednog studenta, tačno jedan termin i tačno jedan komad opreme.  
10. Student može pregledati i otkazati samo vlastite zahtjeve i rezervacije.  
11. Broj odobrenih zahtjeva za jedan termin u okviru kabineta ne smije preći maksimalan dozvoljeni broj studenata definisan za taj kabinet.  
12. Profesor ili asistent može odobriti ili odbiti samo pristigle zahtjeve za rezervaciju.  
13. Zahtjev za rezervaciju može imati status `Pending`, `Approved` ili `Rejected`.  
14. Status zahtjeva se može mijenjati samo do početka termina.  
15. Profesor ili asistent može definisati maksimalan broj aktivnih rezervacija po studentu, a sistem mora spriječiti prekoračenje tog limita.  
16. Oprema koja je označena kao neispravna ne može biti predmet nove rezervacije.  
17. Tehničar može mijenjati status opreme i obrađivati prijave kvarova.  
18. Prijava kvara mora biti vezana za tačno jedan komad opreme.  
19. Kada se prijavi kvar, status opreme se automatski mijenja u `neispravna`.  
20. Jedan komad opreme može imati više prijavljenih kvarova kroz vrijeme.  
21. Tehničar može biti zadužen za obradu više tiketa, ali svaki tiket mora imati tačno jednog odgovornog tehničara.  
22. Administrator upravlja korisnicima, kabinetima, opremom i pravilima korištenja sistema.  
23. Sistem mora spriječiti vremensko preklapanje rezervacija za istu opremu.  
24. Sistem mora voditi računa o dostupnosti opreme u realnom vremenu prilikom kreiranja i odobravanja zahtjeva.
