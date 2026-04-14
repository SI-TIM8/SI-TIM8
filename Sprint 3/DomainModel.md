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

Ovi entiteti pokrivaju osnovne funkcionalnosti sistema, uključujući upravljanje korisnicima, kabinetima, laboratorijskom opremom, terminima, rezervacijama, evidencijom stanja opreme, kao i organizaciju prostora kroz objekte i kabinete.

---

## Ključni atributi

### 1. Korisnik  
Predstavlja korisnika sistema (student, profesor, asistent ili tehničar).

Ključni atributi:
- ImePrezime  
- Email  
- Username  
- Password  
- Uloga  

---

### 2. Kabinet  
Predstavlja laboratorijski prostor u kojem se nalazi oprema i u kojem se odvijaju termini.

Ključni atributi:
- Naziv  

---

### 3. Oprema  
Predstavlja laboratorijske uređaje i alate dostupne za korištenje unutar kabineta.

Ključni atributi:
- Naziv  
- SerijskiBroj  

---

### 4. Termin  
Predstavlja vremenski period rezervisan za korištenje kabineta.

Ključni atributi:
- VrijemePocetka  
- VrijemeKraja  
- Datum  

---

### 5. Zahtjev  
Predstavlja zahtjev korisnika za pristup terminu i korištenje opreme.

Ključni atributi:
- Status  
- Komentar  

---

### 6. Evidencija  
Predstavlja zapis o stanju ili aktivnosti vezanoj za opremu.

Ključni atributi:
- Status  
- Komentar  

---

### 7. Objekat  
Predstavlja fizičku lokaciju (npr. zgradu fakulteta) u kojoj se nalaze kabineti.

Ključni atributi:
- Lokacija  
- RadnoVrijeme  

---

## Veze između entiteta

### Korisnik i Kabinet  
Korisnik može biti povezan sa više kabineta u kojima ima određene ovlasti ili pristup.  

Veza: 1 : N  

---

### Korisnik i Oprema  
Ovlašteni korisnik može dodavati i upravljati opremom u sistemu.  

Veza: 1 : N  

---

### Korisnik i Evidencija  
Ovlašteni korisnik može voditi evidenciju o stanju i aktivnostima opreme.  

Veza: 1 : N  

---

### Korisnik i Objekat  
Korisnici mogu imati pristup više objekata, dok jedan objekat može biti dostupan više korisnika.  
Veza se realizuje preko posredne tabele Korisnik_Objekat.  

Veza: M : N  

---

### Objekat i Kabinet  
Jedan objekat sadrži više kabineta, dok svaki kabinet pripada tačno jednom objektu.  

Veza: 1 : N  

---

### Kabinet i Oprema  
Jedan kabinet sadrži više komada opreme, dok svaki komad opreme pripada tačno jednom kabinetu.  

Veza: 1 : N  

---

### Evidencija i Oprema  
Jedan komad opreme može imati više zapisa evidencije, dok svaki zapis pripada tačno jednoj opremi.  

Veza: 1 : N  

---

### Korisnik i Termin  
Ovlašteni korisnik može upravljati terminima u sistemu.  

Veza: 1 : N  

---

### Kabinet i Termin  
Jedan kabinet može imati više termina, dok svaki termin pripada tačno jednom kabinetu.  

Veza: 1 : N  

---

### Kabinet i Zahtjev  
Jedan kabinet može imati više zahtjeva za rezervaciju, dok svaki zahtjev pripada tačno jednom kabinetu.  

Veza: 1 : N  

---

### Termin i Zahtjev  
Jedan termin može imati više zahtjeva za rezervaciju, dok svaki zahtjev pripada tačno jednom terminu.  

Veza: 1 : N  

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
