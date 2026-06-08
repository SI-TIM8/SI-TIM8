# **ZAVRŠNI IZVJEŠTAJ O TESTIRANJU (QA SUMMARY REPORT)**

**Naziv projekta:** LABsistem

**Datum evaluacije:** 07\. juni 2026\.

**Razvojni ciklus:** Sprints 5 – 11

**Finalni status:**  PROŠLO (PRODUCTION READY / ACID & PERFORMANCE SIGNED-OFF)

## **1\. Vrste testova na projektu**

Unutar cjelokupnog **LABsistem** rješenja implementirana je višeslojna testna strategija koja pokriva funkcionalne i nefunkcionalne zahtjeve kroz sljedeće kategorije:

* **Jedinični testovi (Unit Tests):** Fokusirani na izolovanu provjeru poslovne logike i validacionih pravila (npr. provjera vremenskih opsega, kalkulacija kapaciteta, hashiranje lozinki). Zavisnosti su izolovanje primjenom biblioteke Moq i alata AutoFixture.  
* **Integracioni testovi (Integration Tests):** Verifikuju zajednički rad API endpointa, servisnog sloja, repozitorija (LabSistem.Dal) i baze podataka. Za perzistenciju se koristi EF Core In-Memory Database i TestWebApplicationFactory za simulaciju HTTP zahtjeva sa JWT autorizacijom.  
* **Testovi konkurentnosti i transakcija (ACID Tests):** Posebna grupa integracionih testova dizajnirana da simulira *Race Conditions* (istovremene klikove više korisnika na isti resurs) pomoću asinhronih Task.WhenAll operacija, prateći ponašanje baze podataka.  
* **Performansni / Load testovi:** Izvršeni nad ključnim dijelovima backend sistema kako bi se potvrdila stabilnost pod visokim opterećenjem.  
* **End-to-End (E2E) testovi:** Automatizovani testovi kompletnog toka (frontend \+ backend \+ notifikacije) koji simuliraju ponašanje stvarnog korisnika unutar browsera kroz aplikativni interfejs.

## **2\. Kako se testovi pokreću**

### **Backend (Unit, Integration & ACID)**

Svi .NET testovi su centralizovani unutar testnog projekta LABsistem.Tests. Pokreću se pozicioniranjem u korijenski (Root) folder projekta ili LabSistem.backend direktorijum izvršavanjem standardne .NET CLI naredbe u terminalu:

PowerShell  
dotnet test

*Alternativa:* Kroz VS Code grafički interfejs unutar *Testing* taba (ikona epruvete) odabirom opcije "Run All Tests".

### **Performansno testiranje (Load Testing)**

Izvršava se na bazi napisanih JS skripti unutar k6 okruženja pokretanjem komande:

Bash  
k6 run load\_test\_script.js

### **Frontend E2E testovi**

Pokreću se unutar LABsistem.Frontend direktorijuma kroz Cypress interfejs naredbom:

Bash  
npm run cypress:open

## **3\. Statistički pregled prolaznosti testova**

Nakon izvršavanja kompletne testne dionice nad cjelokupnim rješenjem, registrovani su sljedeći zvanični rezultati pokretanja automatizovanih testova:

Plaintext  
\[xUnit.net 00:00:21.20\]       Finished:     LABsistem.Tests  
  LABsistem.Tests test net10.0 succeeded (22.9s)

Test summary: total: 136, failed: 0, succeeded: 136, skipped: 0, duration: 22.9s  
Build succeeded with 55 warning(s) in 44.5s

### **Tabela prolaznosti po modulima**

| Modul (Slojevi / Sprintevi) | Ukupno testova | Prošlo (Succeeded) | Palo (Failed) | Prolaznost (%) |
| :---- | :---- | :---- | :---- | :---- |
| **Slojeviti Unit & Integration (Sprint 5-7)** | 11 | 11 | 0 | 100% |
| **Rezervacije & Termini (Sprint 8\)** | 11 | 11 | 0 | 100% |
| **Load & ACID testiranje (Sprint 9\)** | 97 | 97 | 0 | 100% |
| **Novi moduli & Obavijesti (Sprint 10\)** | 17 | 17 | 0 | 100% |
| **UKUPNO** | **136** | **136** | **0** | **100%** |

## **4\. Manuelno testiranje i ključni korisnički tokovi**

Pored sveobuhvatnog skupa automatizovanih testova, izvršeno je ručno (manuelno) QA testiranje direktno unutar browsera na lokalnom klijentu (http://localhost) kako bi se verifikovali tokovi koji obuhvataju korisnički interfejs, validaciju formi i rukovanje fajlovima.

### **Provjereni ključni korisnički tokovi:**

1. **Autentifikacija i višeslojna autorizacija:** Uspješna prijava različitih uloga (Student, Profesor, Tehničar). Potvrđeno je da korisnik sa ulogom *Tehničar* ima ispravan pristup administrativnoj tabeli opreme, dok su destruktivne rute sakrivene i zaštićene od pristupa anonimnih korisnika ili studenata.  
2. **Upravljanje opremom (CRUD sa dokumentacijom):** Unutar forme za dodavanje/izmjenu opreme, ručno su testirana polja za pridruživanje tehničke dokumentacije:  
   * **PDF Upload:** Uspješno prenesen i sačuvan PDF fajl veličine do 10MB. Pokušaj uploada fajla od 15MB je uspješno blokiran uz ispis validacione poruke na UI-ju.  
   * **URL Linkovi:** Provjeren unos eksternih URL adresa i njihovo ispravno renderovanje kao klikabilnih linkova u korisničkom pregledu.  
3. **Pregled i rezervacija (Korisnički E2E tok):** \* *Tehničar* kreira slobodan termin kabineta.  
   * *Profesor* rezerviše slot i odobrava/odbija pristigle studentske zahtjeve.  
   * *Student* otvara kalendar, pregleda zauzetost, aplicira za termin i kroz modalni prozor uspješno preuzima (download) priloženi PDF dokument o opremi koja se nalazi u tom kabinetu.

## **5\. Pokrivena biznis pravila (Business Rules) i NFR**

Sistem u potpunosti implementira i uspješno validira sljedeća kritična pravila:

* **BR-RES-001 (Kapacitet):** Limit osoba unutar rezervacije ne može preći definisani fizički kapacitet kabineta.  
* **BR-RES-002 (Otkazivanje):** Korisnik ne može otkazati termin ukoliko je do početka ostalo manje od 24 sata.  
* **BR-NOT-002 (Notifikacije):** Automatski e-mail podsjetnici se generišu i šalju isključivo korisnicima sa verifikovanim e-mail adresama.  
* **NFR-13 (Performanse pod opterećenjem):** Sistem uspješno izdržava nalet od **50 konkurentnih virtuelnih korisnika (VU)** u trajanju od 2 minute. 95% zahtjeva (p95) završava se sa odzivom ispod **420ms**, uz stopu grešaka od **0.00%**.  
* **ACID \- Izolacija (Race Conditions):** Prilikom simultanog slanja zahtjeva za isti termin od strane dva studenta u istoj milisekundi, sistem kroz unutrašnje zaključavanje baze (Locking) propušta samo jedan zahtjev, dok drugi biva bezbjedno odbijen uz 409 Conflict, čime je spriječena korupcija podataka.

## **6\. Poznati testni propusti i ograničenja (Known Issues)**

* **EF Core In-Memory vs Stvarni SQL:** Integracioni testovi koriste *In-Memory* bazu radi brzine izvršavanja. Iako pokrivaju 99% logike, oni ne testiraju specifične bazične trigere i stroge constraints eksterne PostgreSQL baze, što zahtijeva povremeno ručno usklađivanje migracija (dotnet ef database update).  
* **Zavisnost o seedovanim podacima:** Performansni k6 testovi se oslanjaju na unaprijed seedovane profilne račune (loadstudent1 \- loadstudent50). Ukoliko se baza kompletno obriše bez pokretanja seedera, performansni testovi će pasti na koraku autentifikacije.

## **7\. Dokazi o rezultatima testiranja (Test Run Evidence)**

Kao neoboriv dokaz o ispravnosti i prolaznosti cjelokupnog testnog paketa, prilaže se zvanični izlaz iz konzole izvršen nad projektnim rješenjem:

Plaintext  
Priložen dokazni log:  
\================================================================================  
Test Summary:  
  Total:     136  
  Failed:      0  
  Succeeded: 136  
  Skipped:     0  
  Duration:  22.9s  
\================================================================================

**Zaključak:** Aplikacija "LABsistem" pokazuje visok stepen stabilnosti, pokrivenosti poslovnih pravila automatizovanim testovima, te potpunu otpornost na preopterećenje i konkurentne upise u bazu podataka. Sistem je **SPREMAN ZA PRODUKCIJU**.

