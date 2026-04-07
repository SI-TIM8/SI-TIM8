
## 1. Upravljanje pristupom

### USER STORY – Kreiranje korisnika
**ID storyja:** US01  
**Naziv storyja:** Kreiranje korisnika  
**Opis:** Kao administrator, želim kreirati korisnike, kako bi se prijavili na sistem.  
**Poslovna vrijednost:** Omogućava pristup sistemu novim korisnicima.  
**Prioritet:** High  
**Pretpostavke:** Administrator je prijavljen. Postoji forma za unos korisnika.  
**Veze:** -  
**Acceptance Criteria:** 1. Forma za unos je dostupna administratoru.
2. Validacija obaveznih podataka.
3. Korisnik je uspješno kreiran u bazi.
**Sprint:** 6  
**Veza sa Product Backlogom:** 21

---

### USER STORY – Brisanje korisnika
**ID storyja:** US02  
**Naziv storyja:** Brisanje korisnika  
**Opis:** Kao administrator, želim da brišem korisnike, kako bi se smanjio broj korisnika koji ne koriste sistem.  
**Poslovna vrijednost:** Održavanje ažurirane i sigurne baze korisnika.  
**Prioritet:** High  
**Veze:** US01  
**Acceptance Criteria:** 1. Prikaz liste svih korisnika.
2. Dugme za brisanje korisnika pored svakog unosa.
3. Potvrda brisanja (Confirmation dialog).
4. Korisnik je trajno uklonjen iz sistema.  
**Sprint:** 6   
**Veza sa Product Backlogom:** 21

---

### USER STORY – Prijava na sistem
**ID storyja:** US03  
**Naziv storyja:** Prijava na sistem  
**Opis:** Kao registrovani korisnik, želim da se prijavim na sistem, kako bih pristupio resursima.  
**Poslovna vrijednost:** Siguran pristup sistemu.  
**Prioritet:** High  
**Pretpostavke:** Korisnik ima validan nalog.  
**Veze:** US01  
**Acceptance Criteria:** 1. Polja za unos emaila i lozinke.
2. Validacija unesenih kredencijala.
3. Uspješna prijava ili prikaz jasne poruke o grešci.
**Sprint:** 5   
**Veza sa Product Backlogom:** 18 - 20

---

### USER STORY – Promjena lozinke
**ID storyja:** US04  
**Naziv storyja:** Promjena lozinke  
**Opis:** Kao korisnik, želim promijeniti lozinku, kako bih osigurao svoj profil.  
**Poslovna vrijednost:** Povećanje sigurnosti korisničkog naloga.  
**Prioritet:** Medium  
**Pretpostavke:** Korisnik je prijavljen.  
**Veze:** US03  
**Acceptance Criteria:** 1. Unos stare i nove lozinke.
2. Validacija podataka (npr. minimalna dužina).
3. Lozinka je uspješno promijenjena.  
**Sprint:** 10  
**Veza sa Product Backlogom:** 40

---

### USER STORY – Uređivanje profila
**ID storyja:** US05  
**Naziv storyja:** Uređivanje profila  
**Opis:** Kao korisnik, želim izmijeniti svoje podatke kako bih održavao tačne informacije.  
**Poslovna vrijednost:** Tačnost korisničkih podataka.  
**Prioritet:** Medium  
**Pretpostavke:** Korisnik je prijavljen.  
**Veze:** US03  
**Acceptance Criteria:** 1. Prikaz trenutnih podataka korisnika.
2. Mogućnost izmjene podataka.
3. Spremanje izmjena i prikaz ažuriranih informacija.       
**Sprint:** 10  
**Veza sa Product Backlogom:** 41

---

## 2. Upravljanje opremom

### USER STORY – Kreiranje nove opreme
**ID storyja:** US06  
**Naziv storyja:** Kreiranje nove opreme  
**Opis:** Kao laboratorijski tehničar, želim unijeti novu opremu u bazu, kako bi bila dostupna za pregled i rezervaciju.  
**Poslovna vrijednost:** Omogućava korištenje nove opreme u sistemu.  
**Prioritet:** High  
**Pretpostavke:** Tehničar je prijavljen. Postoji forma za unos opreme.  
**Acceptance Criteria:** 1. Forma za unos opreme je funkcionalna.
2. Unos svih potrebnih podataka (naziv, opis, status).
3. Validacija unosa podataka.
4. Oprema uspješno dodana u bazu.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 23

---

### USER STORY – Brisanje opreme
**ID storyja:** US07  
**Naziv storyja:** Brisanje opreme  
**Opis:** Kao laboratorijski tehničar, želim ukloniti opremu koja se više ne koristi.  
**Poslovna vrijednost:** Održavanje tačne evidencije opreme.  
**Prioritet:** Medium  
**Pretpostavke:** Oprema postoji u sistemu.  
**Veze:** US06  
**Acceptance Criteria:** 1. Prikaz liste opreme.
2. Dugme za brisanje opreme.
3. Potvrda brisanja.
4. Oprema trajno uklonjena iz sistema.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 23

---

### USER STORY – Uređivanje statusa opreme
**ID storyja:** US08  
**Naziv storyja:** Uređivanje statusa opreme  
**Opis:** Kao laboratorijski tehničar, želim promijeniti detalje opreme (dostupnost, ispravnost), kako bi podaci bili ispravni.  
**Poslovna vrijednost:** Osigurava tačnost podataka o dostupnosti i ispravnosti.  
**Prioritet:** Medium  
**Pretpostavke:** Oprema postoji u sistemu.  
**Veze:** US06  
**Acceptance Criteria:** 1. Prikaz trenutnog statusa opreme.
2. Mogućnost izmjene statusa (dostupno/neispravno).
3. Spremanje promjena i ažuriranje u realnom vremenu.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 36

---

### USER STORY – Prijava kvara
**ID storyja:** US09  
**Naziv storyja:** Prijava kvara  
**Opis:** Kao profesor, želim prijaviti kvar opreme, kako bi imali preciznu evidenciju ispravnosti.  
**Poslovna vrijednost:** Omogućava evidenciju kvarova i bolju kontrolu.  
**Prioritet:** Medium  
**Pretpostavke:** Oprema postoji. Profesor je prijavljen.  
**Veze:** US08  
**Acceptance Criteria:** 1. Forma za prijavu kvara je dostupna.
2. Unos detaljnog opisa problema.
3. Evidentiranje kvara u sistemu.
4. Automatska promjena statusa opreme na "neispravna".  
**Sprint:** 10  
**Veza sa Product Backlogom:** 43, 44

---

### USER STORY – Filtriranje opreme
**ID storyja:** US10  
**Naziv storyja:** Filtriranje opreme  
**Opis:** Kao korisnik sistema, želim filtrirati opremu po kategoriji, dostupnosti ili nazivu, kako bih brzo pronašao potrebnu opremu.  
**Poslovna vrijednost:** Omogućava efikasan pregled opreme.  
**Prioritet:** Medium  
**Veze:** US08  
**Acceptance Criteria:** 1. Filtriranje po kategoriji.
2. Filtriranje po dostupnosti (dostupna / zauzeta).
3. Mogućnost istovremenog korištenja više filtera.
4. Prikaz samo odgovarajućih rezultata.
5. Opcija za resetovanje svih filtera.  
**Sprint:** 10  
**Veza sa Product Backlogom:** 40

---

## 3. Rezervisanje

### USER STORY – Kreiranje rezervacije
**ID storyja:** US11  
**Naziv storyja:** Kreiranje rezervacije  
**Opis:** Kao student, želim rezervisati slobodan termin i opremu, kako bih obavio vježbe.  
**Poslovna vrijednost:** Transparentan i brz proces rezervacije resursa.  
**Prioritet:** High  
**Pretpostavke:** Student je registrovan.  
**Veze:** US03  
**Acceptance Criteria:** 1. Prikaz isključivo slobodnih termina i opreme.
2. Onemogućeno preklapanje termina.
3. Sistem sprečava rezervaciju termina u prošlosti.
4. Slanje zahtjeva na odobrenje asistentu.  
**Sprint:** 7  
**Veza sa Product Backlogom:** 31, 32

---

### USER STORY – Pregled vlastitih zahtjeva
**ID storyja:** US12  
**Naziv storyja:** Pregled vlastitih zahtjeva  
**Opis:** Kao student, želim pregledati svoje odobrene i podnesene zahtjeve.  
**Poslovna vrijednost:** Transparentan uvid u vlastiti raspored.  
**Prioritet:** Medium  
**Veze:** US03, US11  
**Acceptance Criteria:** 1. Prikaz liste svih zahtjeva/termina specifičnog korisnika.  
**Sprint:** 9  
**Veza sa Product Backlogom:** 39

---

### USER STORY – Otkazivanje rezervacije
**ID storyja:** US13  
**Naziv storyja:** Otkazivanje rezervacije  
**Opis:** Kao student, želim otkazati rezervisani termin, kako bih oslobodio resurse drugima.  
**Poslovna vrijednost:** Automatsko oslobađanje termina bez posrednika.  
**Prioritet:** Medium  
**Pretpostavke:** Student je prijavljen i ima aktivan zakazan termin.  
**Veze:** US03, US11, US12  
**Acceptance Criteria:** 1. Prikaz liste aktivnih termina.
2. Mogućnost otkazivanja pored svakog termina.
3. Trenutno oslobađanje termina u sistemu.  
**Sprint:** 9  
**Veza sa Product Backlogom:** 39

---

### USER STORY – Pregled svih rezervacija
**ID storyja:** US14  
**Naziv storyja:** Pregled rezervacija  
**Opis:** Kao asistent/profesor, želim pregledati zahtjeve, kako bih imao uvid o održavanju vježbi.  
**Poslovna vrijednost:** Centralizovan uvid u korištenje laboratorije.  
**Prioritet:** High  
**Pretpostavke:** Osoblje je prijavljeno.  
**Veze:** US11, US13  
**Acceptance Criteria:** 1. Lista svih zauzetih i slobodnih termina/opreme.
2. Istorija svih prijašnjih zauzeća.
3. Opcija za eksport podataka.
**Sprint:** 8  
**Veza sa Product Backlogom:** 34

---

### USER STORY – Odobravanje/otkazivanje zahtjeva
**ID storyja:** US15  
**Naziv storyja:** Odobravanje zahtjeva  
**Opis:** Kao asistent/profesor, želim pregledati pristignute zahtjeve, kako bih potvrdio/odbio rezervaciju.  
**Poslovna vrijednost:** Kontrola pristupa i povećanje sigurnosti.  
**Prioritet:** High  
**Pretpostavke:** Student je podnio zahtjev. Osoblje je prijavljeno.  
**Veze:** US11, US14  
**Acceptance Criteria:** 1. Lista svih zahtjeva na čekanju (Pending).
2. Opcije "Approve" (Odobri) ili "Reject" (Odbij) uz komentar.
3. Ažuriranje statusa u realnom vremenu.  
**Sprint:** 8  
**Veza sa Product Backlogom:** 35

---

### USER STORY – Ograničenje broja aktivnih rezervacija
**ID storyja:** US16
**Naziv storyja:** Limitiranje rezervacija po korisniku  
**Opis:** Kao profesor, želim postaviti limit na maksimalan broj aktivnih rezervacija po studentu.  
**Poslovna vrijednost:** Pravedna raspodjela termina.  
**Prioritet:** Medium  
**Veze:** US11  
**Acceptance Criteria:** 1. Sistem onemogućava novu rezervaciju ako je student dosegao definirani limit (npr. 3 aktivna termina).
2. Prikaz poruke upozorenja o dosegnutom limitu.  
**Sprint:** 9  
**Veza sa Product Backlogom:** 38

---

## 4. Upravljanje terminima

### USER STORY – Kreiranje radnog vremena
**ID storyja:** US17 
**Naziv storyja:** Kreiranje radnog vremena  
**Opis:** Kao administrator, želim definisati radno vrijeme laboratorije.  
**Poslovna vrijednost:** Definisanje vremenskog okvira za korištenje sistema.  
**Prioritet:** High  
**Pretpostavke:** Radno vrijeme se definiše za specifičnu prostoriju.  
**Veze:** -  
**Acceptance Criteria:** 1. Forma za unos radnog vremena.
2. Unos svih potrebnih parametara.
3. Prikaz radnog vremena korisnicima u realnom vremenu.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 25, 27

---

### USER STORY – Definisanje termina
**ID storyja:** US18 
**Naziv storyja:** Definisanje termina  
**Opis:** Kao laboratorijski tehničar, želim definisati novi termin, kako bi ga korisnici mogli vidjeti.  
**Poslovna vrijednost:** Kreiranje konkretnih slotova za rad.  
**Prioritet:** High  
**Pretpostavke:** Postoji definisano radno vrijeme i prostorija.  
**Veze:** US17  
**Acceptance Criteria:** 1. Forma za unos novog termina.
2. Unos trajanja i opreme.
3. Novi termin vidljiv svim korisnicima odmah.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 26

---

### USER STORY – Mijenjanje termina
**ID storyja:** US19 
**Naziv storyja:** Mijenjanje termina  
**Opis:** Kao laboratorijski tehničar, želim promijeniti postojeći termin.  
**Poslovna vrijednost:** Fleksibilnost u upravljanju rasporedom.  
**Prioritet:** High  
**Pretpostavke:** Termin već postoji.  
**Veze:** US18  
**Acceptance Criteria:** 1. Prikaz forme za postojeći termin.
2. Mogućnost izmjene svih podataka.
3. Ažuriranje termina u sistemu.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 26

---

### USER STORY – Brisanje termina
**ID storyja:** US20  
**Naziv storyja:** Brisanje termina  
**Opis:** Kao laboratorijski tehničar, želim ukloniti postojeći termin korištenja laboratorije.  
**Poslovna vrijednost:** Jednostavno prilagođavanje novim potrebama.  
**Prioritet:** High  
**Pretpostavke:** Termin postoji u sistemu.  
**Veze:** US18, US19  
**Acceptance Criteria:** 1. Mogućnost brisanja odabranog termina.
2. Brisanje se reflektuje u kalendaru odmah.  
**Sprint:** 6  
**Veza sa Product Backlogom:** 26

---

### USER STORY – Pregled slobodnih termina
**ID storyja:** US21  
**Naziv storyja:** Pregled termina  
**Opis:** Kao student, želim pregledati postavljene termine, kako bih znao dostupnost laboratorije.  
**Poslovna vrijednost:** Transparentan uvid u raspored za korisnike.  
**Prioritet:** Medium  
**Pretpostavke:** Korisnik je registrovan. Termini su definisani.  
**Veze:** US03, US11, US18  
**Acceptance Criteria:** 1. Prikaz pregledne liste svih dostupnih slobodnih termina.  
**Sprint:** 7  
**Veza sa Product Backlogom:** 28-30
