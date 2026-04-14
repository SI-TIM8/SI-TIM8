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
