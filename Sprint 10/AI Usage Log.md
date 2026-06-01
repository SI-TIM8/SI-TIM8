# AI Usage Log – Sprint 9

## Unos #1

| Polje                                      | Detalji                                                                                                               |
| ------------------------------------------ | --------------------------------------------------------------------------------------------------------------------- |
| Datum                                      | 24.04.2026                                                                                                            |
| Alat koji je korišten                      | Lovable AI                                                                                                            |
| Svrha korištenja                           | Pomoć pri implementaciji login UI-a i validacije forme                                                                |
| Kratak opis zadatka ili upita              | Izrada login forme sa validacijom unosa (email i password).                                                           |
| Šta je AI predložio ili generisao          | AI je predložio strukturu login komponente, validaciju forme (required polja, email format) i prikaz poruka o grešci. |
| Šta je tim prihvatio                       | Većinu validacione logike i osnovnu strukturu forme.                                                                  |
| Šta je tim izmijenio                       | Izgled forme i stilove kako bi se uklopili u dizajn aplikacije.                                                       |
| Šta je tim odbacio                         | Automatsko generisane stilove koji nisu odgovarali dizajnu projekta.                                                  |
| Rizici, problemi ili greške koje su uočene | Potreba za dodatnom provjerom validacije kako bi se izbjegle sigurnosne greške.                                       |
| Ko je koristio alat                        | Aner Atović                                                                                                           |

## Unos #2

| Polje                                      | Detalji                                                                                   |
| ------------------------------------------ | ----------------------------------------------------------------------------------------- |
| Datum                                      | 25.04.2026                                                                                |
| Alat koji je korišten                      | Github Copilot                                                                            |
| Svrha korištenja                           | Pomoć pri implementaciji JWT autentifikacije                                              |
| Kratak opis zadatka ili upita              | Generisanje i verifikacija JWT tokena na backendu.                                        |
| Šta je AI predložio ili generisao          | AI je predložio primjer koda za generisanje JWT tokena i middleware za validaciju tokena. |
| Šta je tim prihvatio                       | Osnovnu logiku za generisanje i verifikaciju tokena.                                      |
| Šta je tim izmijenio                       | Podešavanja trajanja tokena i sadržaj JWT claims-a.                                       |
| Šta je tim odbacio                         | Predložene default tajne ključeve (zamijenjeni sigurnijim vrijednostima).                 |
| Rizici, problemi ili greške koje su uočene | Rizik korištenja nesigurnih ključeva ako se preuzmu direktno iz AI primjera.              |
| Ko je koristio alat                        | Haris Sadiković                                                                           |

## Unos #3

| Polje                                      | Detalji                                                              |
| ------------------------------------------ | -------------------------------------------------------------------- |
| Datum                                      | 25.04.2026                                                           |
| Alat koji je korišten                      | Claude Sonnet                                                        |
| Svrha korištenja                           | Implementacija RBAC logike i zaštite ruta                            |
| Kratak opis zadatka ili upita              | Definisanje korisničkih uloga i zaštita ruta na osnovu uloge.        |
| Šta je AI predložio ili generisao          | AI je predložio primjer middleware-a za provjeru korisničkih uloga.  |
| Šta je tim prihvatio                       | Logiku provjere role prije pristupa određenim rutama.                |
| Šta je tim izmijenio                       | Nazive uloga i strukturu permisija prema potrebama sistema.          |
| Šta je tim odbacio                         | Previše generičku strukturu permisija.                               |
| Rizici, problemi ili greške koje su uočene | Mogućnost pogrešne konfiguracije uloga ako se ne testira svaka ruta. |
| Ko je koristio alat                        | Emina Hamamdžić                                                      |

## Unos #4

| Polje                                      | Detalji                                                                   |
| ------------------------------------------ | ------------------------------------------------------------------------- |
| Datum                                      | 25.04.2026                                                                |
| Alat koji je korišten                      | Github Copilot                                                            |
| Svrha korištenja                           | Implementacija logout funkcionalnosti                                     |
| Kratak opis zadatka ili upita              | Sigurno uklanjanje tokena i redirect korisnika na login stranicu.         |
| Šta je AI predložio ili generisao          | AI je predložio funkciju za brisanje tokena iz localStorage-a i redirect. |
| Šta je tim prihvatio                       | Logiku za uklanjanje tokena i redirect.                                   |
| Šta je tim izmijenio                       | Dodali dodatnu provjeru stanja autentifikacije.                           |
| Šta je tim odbacio                         | Automatsko osvježavanje stranice nakon logout-a.                          |
| Rizici, problemi ili greške koje su uočene | Rizik da token ostane u memoriji ako logout nije pravilno implementiran.  |
| Ko je koristio alat                        | Hamza Hadžić                                                              |

## Unos #5

| Polje                                      | Detalji                                                                              |
| ------------------------------------------ | ------------------------------------------------------------------------------------ |
| Datum                                      | 25.04.2026                                                                           |
| Alat koji je korišten                      | Claude Sonnet                                                                        |
| Svrha korištenja                           | Implementacija automatskog isteka sesije                                             |
| Kratak opis zadatka ili upita              | Implementacija logike za automatsko odjavljivanje korisnika nakon isteka JWT tokena. |
| Šta je AI predložio ili generisao          | AI je predložio korištenje timeout funkcije i provjeru vremena isteka tokena.        |
| Šta je tim prihvatio                       | Logiku za provjeru expiration vremena tokena.                                        |
| Šta je tim izmijenio                       | Način prikaza poruke korisniku prije isteka sesije.                                  |
| Šta je tim odbacio                         | Predloženo prečesto provjeravanje tokena zbog performansi.                           |
| Rizici, problemi ili greške koje su uočene | Mogućnost nepravilnog logout-a ako vrijeme isteka nije pravilno očitano.             |
| Ko je koristio alat                        | Alma Jusufbegović                                                                    |

## Unos #6

| Polje                                      | Detalji                                                                  |
| ------------------------------------------ | ------------------------------------------------------------------------ |
| Datum                                      | 27.04.2026                                                               |
| Alat koji je korišten                      | Claude Sonnet                                                            |
| Svrha korištenja                           | Pomoć pri pisanju testova                                                |
| Kratak opis zadatka ili upita              | Kreiranje unit i integration testova za autentifikaciju.                 |
| Šta je AI predložio ili generisao          | AI je predložio primjere testova za login i validaciju tokena.           |
| Šta je tim prihvatio                       | Osnovne testne scenarije.                                                |
| Šta je tim izmijenio                       | Testne podatke i strukturu testova.                                      |
| Šta je tim odbacio                         | Automatski generisane testove koji nisu odgovarali arhitekturi projekta. |
| Rizici, problemi ili greške koje su uočene | Testovi mogu biti nepotpuni ako se slijepo prihvate AI prijedlozi.       |
| Ko je koristio alat                        | Merima Glušac                                                            |

## Unos #7

| Polje                                      | Detalji                                                                             |
| ------------------------------------------ | ----------------------------------------------------------------------------------- |
| Datum                                      | 27.04.2026                                                                          |
| Alat koji je korišten                      | Claude Sonnet                                                                       |
| Svrha korištenja                           | Konfiguracija CI/CD pipeline-a                                                      |
| Kratak opis zadatka ili upita              | Kreiranje Docker konfiguracije i CI/CD pipeline-a.                                  |
| Šta je AI predložio ili generisao          | AI je generisao primjer Dockerfile-a i osnovni CI/CD workflow.                      |
| Šta je tim prihvatio                       | Osnovnu strukturu Docker konfiguracije.                                             |
| Šta je tim izmijenio                       | Varijable okruženja i build korake.                                                 |
| Šta je tim odbacio                         | Predložene default konfiguracije koje nisu odgovarale projektu.                     |
| Rizici, problemi ili greške koje su uočene | Rizik pogrešne konfiguracije pipeline-a koji može uzrokovati neuspješan deployment. |
| Ko je koristio alat                        | Haris Macić                                                                         |

## Unos #8

| Polje                                      | Detalji                                               |
| ------------------------------------------ | ----------------------------------------------------- |
| Datum                                      | 27.04.2026                                            |
| Alat koji je korišten                      | ChatGPT                                               |
| Svrha korištenja                           | Dokumentacija API endpointa                           |
| Kratak opis zadatka ili upita              | Kreiranje dokumentacije za API rute.                  |
| Šta je AI predložio ili generisao          | AI je generisao primjer dokumentacije endpointa.      |
| Šta je tim prihvatio                       | Strukturu dokumentacije.                              |
| Šta je tim izmijenio                       | Opise parametara i odgovora.                          |
| Šta je tim odbacio                         | Previše generičke opise endpointa.                    |
| Rizici, problemi ili greške koje su uočene | Rizik netačne dokumentacije ako se ne provjeri ručno. |
| Ko je koristio alat                        | Refik Mujčinović                                      |

## Unos #9

| Polje                                      | Detalji                                                                                                            |
| ------------------------------------------ | ------------------------------------------------------------------------------------------------------------------ |
| Datum                                      | 30.04.2026                                                                                                         |
| Alat koji je korišten                      | Gemini                                                                                                             |
| Svrha korištenja                           | Implementacija korisničkog CRUD-a                                                                                  |
| Kratak opis zadatka ili upita              | Kreiranje CRUD operacija za korisnike (frontend, backend i baza), uključujući aktivaciju i deaktivaciju korisnika. |
| Šta je AI predložio ili generisao          | AI je predložio strukturu API endpointa, primjer kontrolera i modela za korisnika, kao i validaciju podataka.      |
| Šta je tim prihvatio                       | Osnovnu strukturu CRUD operacija i validaciju inputa.                                                              |
| Šta je tim izmijenio                       | Način deaktivacije korisnika (soft delete umjesto brisanja).                                                       |
| Šta je tim odbacio                         | Direktno brisanje korisnika iz baze.                                                                               |
| Rizici, problemi ili greške koje su uočene | Potencijalna sigurnosna ranjivost ako se ne validiraju svi input podaci.                                           |
| Ko je koristio alat                        | Hamza Hadžić                                                                                                       |

## Unos #10

| Polje                                      | Detalji                                                                               |
| ------------------------------------------ | ------------------------------------------------------------------------------------- |
| Datum                                      | 02.05.2026                                                                            |
| Alat koji je korišten                      | Claude Sonnet                                                                         |
| Svrha korištenja                           | Implementacija CRUD operacija za opremu                                               |
| Kratak opis zadatka ili upita              | Kreiranje CRUD funkcionalnosti za upravljanje opremom i logike za status opreme.      |
| Šta je AI predložio ili generisao          | AI je predložio strukturu backend servisa i frontend komponenti za prikaz opreme.     |
| Šta je tim prihvatio                       | Osnovnu logiku CRUD operacija i prikaz liste opreme.                                  |
| Šta je tim izmijenio                       | Logiku statusa opreme (dodane dodatne vrijednosti poput "u kvaru", "ispravno").       |
| Šta je tim odbacio                         | Previše pojednostavljenu logiku statusa (npr. samo aktivno/neaktivno).                |
| Rizici, problemi ili greške koje su uočene | Mogućnost nekonzistentnog stanja opreme ako se status ne ažurira pravilno.            |
| Ko je koristio alat                        | Aner Atović                                                                           |

## Unos #11

| Polje                                      | Detalji                                                                                             |
| ------------------------------------------ | --------------------------------------------------------------------------------------------------- |
| Datum                                      | 04.05.2026                                                                                          |
| Alat koji je korišten                      | Claude Sonnet                                                                                       |
| Svrha korištenja                           | Implementacija termina i radnog vremena                                                             |
| Kratak opis zadatka ili upita              | Definisanje radnog vremena laboratorija, upravljanje kabinetima i kreiranje termina sa validacijom. |
| Šta je AI predložio ili generisao          | AI je predložio logiku za provjeru preklapanja termina i validaciju radnog vremena.                 |
| Šta je tim prihvatio                       | Osnovni algoritam za provjeru konflikta termina.                                                    |
| Šta je tim izmijenio                       | Pravila za blokiranje perioda i ograničenja termina.                                                |
| Šta je tim odbacio                         | Generički pristup bez uzimanja u obzir radnog vremena laboratorije.                                 |
| Rizici, problemi ili greške koje su uočene | Kompleksnost logike može dovesti do bugova ako se ne testira dovoljno.                              |
| Ko je koristio alat                        | Refik Mujčinović                                                                                    |

## Unos #12

| Polje                                      | Detalji                                                                        |
| ------------------------------------------ | ------------------------------------------------------------------------------ |
| Datum                                      | 04.05.2026                                                                     |
| Alat koji je korišten                      | Claude Sonnet                                                                  |
| Svrha korištenja                           | Pisanje integracijskih testova                                                 |
| Kratak opis zadatka ili upita              | Kreiranje integracijskih testova za sve CRUD operacije u sistemu.              |
| Šta je AI predložio ili generisao          | AI je generisao primjere test slučajeva i strukturu testova za API.            |
| Šta je tim prihvatio                       | Osnovne test scenarije za CRUD operacije.                                      |
| Šta je tim izmijenio                       | Testne podatke i način inicijalizacije test okruženja.                         |
| Šta je tim odbacio                         | Testove koji nisu pokrivali specifične edge-case scenarije.                    |
| Rizici, problemi ili greške koje su uočene | Mogućnost nedovoljne pokrivenosti testovima ako se koriste generički primjeri. |
| Ko je koristio alat                        | Emina Hamamdžić, Alma Jusufbegović                                             |

## Unos #13

| Polje                                      | Detalji                                                                                                                 |
| ------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------- |
| Datum                                      | 06.05.2026                                                                                                              |
| Alat koji je korišten                      | Gemini                                                                                                                  |
| Svrha korištenja                           | Implementacija kalendar UI prikaza                                                                                      |
| Kratak opis zadatka ili upita              | Izrada kalendarskog prikaza rezervacija i color-coding sistema za različite statuse termina.                            |
| Šta je AI predložio ili generisao          | AI je predložio strukturu React komponenti za kalendar, način prikaza događaja i mapiranje boja za statuse rezervacija. |
| Šta je tim prihvatio                       | Osnovni raspored kalendara i logiku za color-coding termina.                                                            |
| Šta je tim izmijenio                       | Prikaz detalja termina i boje statusa radi bolje preglednosti korisnicima.                                              |
| Šta je tim odbacio                         | Previše kompleksan drag-and-drop pristup za upravljanje terminima.                                                      |
| Rizici, problemi ili greške koje su uočene | Moguć problem sa preglednošću pri velikom broju termina u jednom danu.                                                  |
| Ko je koristio alat                        | Merima Glušac                                                                                                           |

## Unos #14

| Polje                                      | Detalji                                                                                                         |
| ------------------------------------------ | --------------------------------------------------------------------------------------------------------------- |
| Datum                                      | 08.05.2026                                                                                                      |
| Alat koji je korišten                      | Claude Sonnet                                                                                                   |
| Svrha korištenja                           | Implementacija prikaza slobodnih termina                                                                        |
| Kratak opis zadatka ili upita              | Kreiranje logike za dohvat i prikaz slobodnih termina laboratorija.                                             |
| Šta je AI predložio ili generisao          | AI je predložio backend filtriranje dostupnih termina i frontend prikaz liste sa filtrima po datumu i kabinetu. |
| Šta je tim prihvatio                       | Logiku filtriranja i osnovni prikaz slobodnih termina.                                                          |
| Šta je tim izmijenio                       | Dodane dodatne validacije za preklapanje termina i radno vrijeme laboratorije.                                  |
| Šta je tim odbacio                         | Prikaz svih termina bez paginacije ili filtriranja.                                                             |
| Rizici, problemi ili greške koje su uočene | Rizik od prikaza zastarjelih termina pri konkurentnim rezervacijama.                                            |
| Ko je koristio alat                        | Emina Hamamdžić                                                                                                 |

## Unos #15

| Polje                                      | Detalji                                                                                 |
| ------------------------------------------ | --------------------------------------------------------------------------------------- |
| Datum                                      | 08.05.2026                                                                              |
| Alat koji je korišten                      | ChatGPT                                                                                 |
| Svrha korištenja                           | Implementacija CRUD operacija za rezervacije                                            |
| Kratak opis zadatka ili upita              | Razvoj frontend i backend funkcionalnosti za kreiranje, izmjenu i brisanje rezervacija. |
| Šta je AI predložio ili generisao          | AI je generisao prijedlog API endpointa, DTO modela i validacije rezervacija.           |
| Šta je tim prihvatio                       | Osnovnu strukturu CRUD operacija i validaciju zahtjeva.                                 |
| Šta je tim izmijenio                       | Poslovna pravila za rezervacije i način provjere konflikta termina.                     |
| Šta je tim odbacio                         | Direktno brisanje rezervacija bez evidencije statusa zahtjeva.                          |
| Rizici, problemi ili greške koje su uočene | Mogućnost race-condition problema kod paralelnih rezervacija.                           |
| Ko je koristio alat                        | Refik Mujčinović                                                                        |

## Unos #16

| Polje                                      | Detalji                                                                                |
| ------------------------------------------ | -------------------------------------------------------------------------------------- |
| Datum                                      | 09.05.2026                                                                             |
| Alat koji je korišten                      | Claude Sonnet                                                                          |
| Svrha korištenja                           | Implementacija odobravanja i odbijanja zahtjeva                                        |
| Kratak opis zadatka ili upita              | Kreiranje funkcionalnosti za pregled, odobravanje i odbijanje zahtjeva za rezervacije. |
| Šta je AI predložio ili generisao          | AI je predložio statusni model rezervacija i tok obrade zahtjeva.                      |
| Šta je tim prihvatio                       | Statusni workflow (na čekanju, odobreno, odbijeno).                                    |
| Šta je tim izmijenio                       | Dodane validacije profesorovih privilegija.                                            |
| Šta je tim odbacio                         | Automatsko odobravanje svih zahtjeva bez ručne provjere.                               |
| Rizici, problemi ili greške koje su uočene | Rizik nekonzistentnog statusa rezervacije pri simultanim zahtjevima.                   |
| Ko je koristio alat                        | Alma Jusufbegović                                                                      |

## Unos #17

| Polje                                      | Detalji                                                                                            |
| ------------------------------------------ | -------------------------------------------------------------------------------------------------- |
| Datum                                      | 10.05.2026                                                                                         |
| Alat koji je korišten                      | Github Copilot                                                                                     |
| Svrha korištenja                           | Testiranje kalendara i rezervacija                                                                 |
| Kratak opis zadatka ili upita              | Pisanje unit i integracijskih testova za funkcionalnosti kalendara i rezervacija.                  |
| Šta je AI predložio ili generisao          | AI je generisao primjere test scenarija za rezervacije, validaciju termina i API endpoint testove. |
| Šta je tim prihvatio                       | Većinu osnovnih test scenarija za rezervacije i validaciju konflikta termina.                      |
| Šta je tim izmijenio                       | Testne podatke i edge-case scenarije za preklapanje rezervacija.                                   |
| Šta je tim odbacio                         | Generičke testove bez provjere poslovnih pravila sistema.                                          |
| Rizici, problemi ili greške koje su uočene | Nedovoljna pokrivenost concurrency scenarija može dovesti do neotkrivenih bugova.                  |
| Ko je koristio alat                        | Aner Atović, Haris Sadiković                                                                       |

## Unos #18

| Polje                                      | Detalji                                                                                                     |
| ------------------------------------------ | ----------------------------------------------------------------------------------------------------------- |
| Datum                                      | 13.05.2026                                                                                                  |
| Alat koji je korišten                      | Github Copilot                                                                                              |
| Svrha korištenja                           | Implementacija validacije konflikta termina i opreme                                                        |
| Kratak opis zadatka ili upita              | Razvoj logike za automatsku provjeru preklapanja rezervacija laboratorija i zauzetosti opreme.              |
| Šta je AI predložio ili generisao          | AI je predložio backend validaciju konflikta termina pomoću provjere vremenskih intervala i statusa opreme. |
| Šta je tim prihvatio                       | Većinu logike za validaciju preklapanja termina i rezervacija opreme.                                       |
| Šta je tim izmijenio                       | Dodane dodatne provjere za specifične statuse rezervacija i laboratorija.                                   |
| Šta je tim odbacio                         | Isključivo frontend validaciju bez backend provjere.                                                        |
| Rizici, problemi ili greške koje su uočene | Potencijalni race-condition problemi kod paralelnih rezervacija.                                            |
| Ko je koristio alat                        | Aner Atović, Haris Sadiković                                                                                |

## Unos #19

| Polje                                      | Detalji                                                                                |
| ------------------------------------------ | -------------------------------------------------------------------------------------- |
| Datum                                      | 13.05.2026                                                                             |
| Alat koji je korišten                      | Gemini                                                                                 |
| Svrha korištenja                           | Implementacija email i in-app notifikacija                                             |
| Kratak opis zadatka ili upita              | Kreiranje sistema obavijesti za promjene statusa rezervacija.                          |
| Šta je AI predložio ili generisao          | AI je predložio event-based sistem notifikacija i predloške email poruka za korisnike. |
| Šta je tim prihvatio                       | Osnovni tok slanja email notifikacija i prikaz in-app obavijesti.                      |
| Šta je tim izmijenio                       | Dizajn i sadržaj notifikacija prilagođen potrebama projekta.                           |
| Šta je tim odbacio                         | Push notifikacije preko eksternih servisa.                                             |
| Rizici, problemi ili greške koje su uočene | Mogućnost kašnjenja email notifikacija zbog SMTP konfiguracije.                        |
| Ko je koristio alat                        | Emina Hamamdžić, Alma Jusufbegović                                                     |

## Unos #20

| Polje                                      | Detalji                                                                                       |
| ------------------------------------------ | --------------------------------------------------------------------------------------------- |
| Datum                                      | 14.05.2026                                                                                    |
| Alat koji je korišten                      | ChatGPT                                                                                       |
| Svrha korištenja                           | Automatska promjena statusa opreme                                                            |
| Kratak opis zadatka ili upita              | Implementacija logike za zauzimanje i oslobađanje opreme nakon rezervacije.                   |
| Šta je AI predložio ili generisao          | AI je predložio statusni model opreme i automatsko ažuriranje stanja pri potvrdi rezervacije. |
| Šta je tim prihvatio                       | Automatsku promjenu statusa opreme tokom rezervacijskog workflowa.                            |
| Šta je tim izmijenio                       | Dodane dodatne provjere dostupnosti opreme pri izmjeni rezervacija.                           |
| Šta je tim odbacio                         | Ručno upravljanje statusima opreme od strane administratora za svaku rezervaciju.             |
| Rizici, problemi ili greške koje su uočene | Rizik nekonzistentnog statusa opreme pri neuspješnom rollback-u rezervacije.                  |
| Ko je koristio alat                        | Hamza Hadžić, Merima Glušac                                                                   |

## Unos #21

| Polje                                      | Detalji                                                                                                          |
| ------------------------------------------ | ---------------------------------------------------------------------------------------------------------------- |
| Datum                                      | 08.05.2026                                                                                                       |
| Alat koji je korišten                      | Gemini                                                                                                           |
| Svrha korištenja                           | E2E i integracijsko testiranje rezervacijskog workflowa                                                          |
| Kratak opis zadatka ili upita              | Kreiranje scenarija za testiranje kompletnog toka rezervacije laboratorija i opreme.                             |
| Šta je AI predložio ili generisao          | AI je generisao primjere E2E scenarija za rezervaciju, odobravanje, notifikacije i validaciju konflikta termina. |
| Šta je tim prihvatio                       | Većinu osnovnih test scenarija i API integracijskih testova.                                                     |
| Šta je tim izmijenio                       | Testni podaci i edge-case scenariji prilagođeni poslovnim pravilima sistema.                                     |
| Šta je tim odbacio                         | Isključivo manuelno testiranje bez automatizacije.                                                               |
| Rizici, problemi ili greške koje su uočene | Veća kompleksnost održavanja E2E testova pri promjenama frontend interfejsa.                                     |
| Ko je koristio alat                        | Refik Mujčinović, Haris Macić                                                                                    |

## Unos #22

| Polje | Detalji |
|---|---|
| Datum | 22.05.2026 |
| Alat koji je korišten | GitHub Copilot |
| Svrha korištenja | Implementacija toka prijave kvara opreme i automatske promjene statusa |
| Kratak opis zadatka ili upita | Razvoj backend logike za prijavu kvara od strane profesora, uključujući validaciju forme, unos komentara i automatsku promjenu statusa opreme u "neispravna". |
| Šta je AI predložio ili generisao | AI je predložio strukturu endpoint-a za prijavu kvara, validaciju obaveznih polja forme i logiku automatskog ažuriranja statusa opreme nakon evidentiranja kvara. |
| Šta je tim prihvatio | Strukturu API endpoint-a i logiku automatske promjene statusa opreme na "neispravna". |
| Šta je tim izmijenio | Dodate su dodatne provjere validacije unosa komentara i prilagođene poruke greške prema specifičnostima sistema. |
| Šta je tim odbacio | Prijedlog za odloženo ažuriranje statusa putem background job-a – odlučeno je za sinhrono ažuriranje. |
| Rizici, problemi ili greške koje su uočene | Mogući problem sa race condition-om ako više korisnika istovremeno prijavi kvar iste opreme. |
| Ko je koristio alat | Emina Hamamdžić |

## Unos #23

| Polje | Detalji |
|---|---|
| Datum | 21.05.2026 |
| Alat koji je korišten | GitHub Copilot |
| Svrha korištenja | Implementacija sistemskog limita aktivnih zahtjeva po studentu uz validaciju i poruku upozorenja |
| Kratak opis zadatka ili upita | Razvoj logike koja onemogućava slanje novog zahtjeva kada student dostigne maksimalan broj aktivnih zahtjeva (statusi "Na čekanju" i "Odobren"). |
| Šta je AI predložio ili generisao | AI je predložio query za prebrojavanje aktivnih zahtjeva po studentu, uvjet provjere limita prije kreiranja novog zahtjeva i strukturu poruke upozorenja. |
| Šta je tim prihvatio | Query logiku za prebrojavanje i provjeru limita, te strukturu poruke upozorenja prema korisniku. |
| Šta je tim izmijenio | Precizirana definicija "aktivnih zahtjeva" – eksplicitno uključeni samo statusi "Na čekanju" i "Odobren" kako bi bila usklađena sa poslovnim pravilima. |
| Šta je tim odbacio | Prijedlog za konfigurabilni limit putem admin panela – limit je za sada hardkodiran kao konstanta. |
| Rizici, problemi ili greške koje su uočene | Potrebno je osigurati da se limit provjerava unutar transakcije kako bi se izbjeglo zaobilaženje kroz paralelne zahtjeve. |
| Ko je koristio alat | Refik Mujčinović |

## Unos #24

| Polje | Detalji |
|---|---|
| Datum | 22.05.2026 |
| Alat koji je korišten | Claude Sonnet |
| Svrha korištenja | Implementacija obavezne promjene lozinke pri prvom loginu za novokreirane korisnike |
| Kratak opis zadatka ili upita | Razvoj mehanizma koji detektuje prvi login korisnika i preusmjerava ga na ekran za promjenu lozinke, blokirajući pristup ostatku aplikacije dok nova lozinka ne bude postavljena. |
| Šta je AI predložio ili generisao | AI je predložio middleware logiku za detekciju zastavice "first_login", redirect na ekran za promjenu lozinke i validaciju nove lozinke prema sigurnosnim pravilima. |
| Šta je tim prihvatio | Middleware pristup za blokiranje pristupa i logiku provjere zastavice "first_login" u bazi podataka. |
| Šta je tim izmijenio | Izuzeti seedovani korisnici iz ovog toka – zastavica se ne postavlja za korisnike kreirane putem seed skripti. |
| Šta je tim odbacio | Prijedlog za slanje e-mail obavijesti sa privremenom lozinkom – implementacija e-mail servisa izvan opsega sprinta. |
| Rizici, problemi ili greške koje su uočene | Potrebno paziti da token sesije ne daje pune privilegije sve dok korisnik ne promijeni lozinku. |
| Ko je koristio alat | Hamza Hadžić |

## Unos #25

| Polje | Detalji |
|---|---|
| Datum | 22.05.2026 |
| Alat koji je korišten | GitHub Copilot |
| Svrha korištenja | Implementacija ekrana "Moje rezervacije" s tabovima za pregled i upravljanje aktivnim rezervacijama i zahtjevima |
| Kratak opis zadatka ili upita | Razvoj frontend komponente sa dva taba (Aktivne rezervacije i Moji zahtjevi) i backend logike za otkazivanje rezervacija i poništavanje zahtjeva na čekanju. |
| Šta je AI predložio ili generisao | AI je predložio strukturu tabbed komponente u Reactu, API endpoint-e za dohvatanje rezervacija i zahtjeva po studentu, te logiku za promjenu statusa na "Otkazan". |
| Šta je tim prihvatio | Strukturu tabbed prikaza i logiku filtriranja rezervacija i zahtjeva prema studentu koji je prijavljen. |
| Šta je tim izmijenio | Dodate su provjere koje onemogućavaju otkazivanje već završenih ili odbijenih stavki i termina koji su već počeli. |
| Šta je tim odbacio | Prijedlog za paginaciju unutar tabova – odlučeno je za scrollable listu s obzirom na očekivani broj stavki. |
| Rizici, problemi ili greške koje su uočene | Potrebno osigurati da student ne može manipulisati ID-evima u zahtjevu i pristupiti tuđim rezervacijama. |
| Ko je koristio alat | Hamza Hadžić |

## Unos #26

| Polje | Detalji |
|---|---|
| Datum | 22.05.2026 |
| Alat koji je korišten | ChatGPT |
| Svrha korištenja | Zamjena funkcionalnosti trajnog brisanja opreme arhiviranjem, uz filtere i mogućnost vraćanja iz arhive |
| Kratak opis zadatka ili upita | Refaktoring logike brisanja opreme u arhiviranje (soft delete), dodavanje filtera za prikaz arhivirane opreme i implementacija akcije vraćanja opreme iz arhive. |
| Šta je AI predložio ili generisao | AI je predložio dodavanje boolean polja "archived" u model opreme, modifikaciju postojećih query-ja za isključivanje arhivirane opreme iz aktivnih lista i endpoint za arhiviranje/vraćanje. |
| Šta je tim prihvatio | Pristup soft delete-a putem "archived" zastavice i modifikaciju svih postojećih query-ja za filtriranje arhivirane opreme. |
| Šta je tim izmijenio | Dodano polje "archived_at" za praćenje vremena arhiviranja, kao i čuvanje historije kvarova za arhiviranu opremu. |
| Šta je tim odbacio | Prijedlog za automatsko arhiviranje opreme nakon određenog perioda neaktivnosti – nije u skladu sa poslovnim zahtjevima. |
| Rizici, problemi ili greške koje su uočene | Potrebno provjeriti da sve buduće funkcionalnosti koje dohvataju opremu koriste filter za isključivanje arhivirane opreme. |
| Ko je koristio alat | Merima Glušac |

## Unos #27

| Polje | Detalji |
|---|---|
| Datum | 24.05.2026 |
| Alat koji je korišten | GitHub Copilot |
| Svrha korištenja | Implementacija uploada PDF uputstava i unosa URL linkova uz opremu |
| Kratak opis zadatka ili upita | Razvoj funkcionalnosti za prilaganje PDF fajlova (do 10MB) i eksternih URL linkova pri kreiranju ili uređivanju opreme, te prikaz i preuzimanje dokumentacije na stranici detalja opreme. |
| Šta je AI predložio ili generisao | AI je predložio multipart/form-data upload endpoint, validaciju veličine i tipa fajla na backendu, regex validaciju URL formata i strukturu prikaza dokumentacije na frontend-u. |
| Šta je tim prihvatio | Backend validaciju PDF fajlova (veličina i tip), URL format validaciju i strukturu prikaza dokumentacije. |
| Šta je tim izmijenio | Dodata je validacija URL formata i na frontend-u radi boljeg korisničkog iskustva prije slanja zahtjeva. |
| Šta je tim odbacio | Prijedlog za ekstrakciju i indeksiranje teksta iz PDF-a za pretragu – izvan opsega trenutne iteracije. |
| Rizici, problemi ili greške koje su uočene | Potrebno osigurati sigurno čuvanje uploadovanih fajlova i spriječiti upload potencijalno malicioznih fajlova. |
| Ko je koristio alat | Haris Sadiković |

## Unos #28

| Polje | Detalji |
|---|---|
| Datum | 19.05.2026 |
| Alat koji je korišten | Claude Sonnet, Gemini |
| Svrha korištenja | Generisanje skripti za load testiranje i provjeru NFR zahtjeva (NFR-13, NFR-17, NFR-19) |
| Kratak opis zadatka ili upita | Pisanje test skripti za simulaciju 50 concurrent korisnika (NFR-13), provjera ACID svojstava baze podataka (NFR-17) i validacija backup i restore procedure (NFR-19). |
| Šta je AI predložio ili generisao | AI je predložio k6 skriptu za load testiranje sa 50 virtualnih korisnika, SQL transakcije za provjeru atomičnosti i konzistentnosti podataka, te proceduru za simulaciju greške i restore iz backupa. |
| Šta je tim prihvatio | Strukturu k6 load test skripte i SQL upite za provjeru ACID svojstava. |
| Šta je tim izmijenio | Prilagođeni su scenariji load testa stvarnim korisničkim tokovima sistema (prijava, pregled opreme, slanje zahtjeva). |
| Šta je tim odbacio | Prijedlog za kontinuirani monitoring putem Prometheus/Grafana stacka – van opsega hackathona. |
| Rizici, problemi ili greške koje su uočene | Pri velikom opterećenju uočena su kašnjenja u odzivima; identificiran potencijalni bottleneck na nivou connection pool-a baze podataka. |
| Ko je koristio alat | Alma Jusufbegović |

## Unos #29

| Polje | Detalji |
|---|---|
| Datum | 24.05.2026 |
| Alat koji je korišten | Claude |
| Svrha korištenja | Implementacija slanja općenitih obavijesti svim korisnicima i automatskih obavijesti o otkazivanju rezervacija |
| Kratak opis zadatka ili upita | Razvoj funkcionalnosti za kreiranje obavijesti vidljivih u zvonu za obavijesti – automatskih (otkazivanje rezervacije zbog kvara) i ručnih (općenita obavijest ovlaštenog korisnika svim aktivnim korisnicima). |
| Šta je AI predložio ili generisao | AI je predložio model tabele obavijesti s poljima za korisnika, sadržaj, status čitanja i timestamp, endpoint za masovno kreiranje obavijesti i realtime ažuriranje brojača nepročitanih. |
| Šta je tim prihvatio | Strukturu modela obavijesti i logiku masovnog kreiranja obavijesti za sve aktivne korisnike. |
| Šta je tim izmijenio | Dodata je provjera da pošiljalac dobije potvrdu o uspješnom slanju i ograničena dužina teksta obavijesti. |
| Šta je tim odbacio | Prijedlog za WebSocket realtime push – implementirano je polling s kraćim intervalom kao jednostavnija alternativa. |
| Rizici, problemi ili greške koje su uočene | Masovno kreiranje obavijesti za veliki broj korisnika može biti sporo – potrebno razmotriti batch insert. |
| Ko je koristio alat | Haris Macić |

## Unos #30

| Polje | Detalji |
|---|---|
| Datum | 24.05.2026 |
| Alat koji je korišten | ChatGPT |
| Svrha korištenja | Implementacija pregleda detalja o kabinetima s prikazom opreme, lokacije i odgovornog profesora |
| Kratak opis zadatka ili upita | Razvoj stranice detalja kabineta koja prikazuje naziv, lokaciju, odgovornog profesora, kapacitet i listu dostupne opreme unutar odabranog kabineta. |
| Šta je AI predložio ili generisao | AI je predložio API endpoint za dohvatanje detalja kabineta sa JOIN-om na tabelu opreme i profesora, te strukturu React komponente za prikaz informacija. |
| Šta je tim prihvatio | Strukturu endpoint-a i JOIN logiku za agregiranje podataka o kabinetima i opremi u jednom pozivu. |
| Šta je tim izmijenio | Filtrirana je lista opreme da prikazuje samo dostupnu (ne arhiviranu i ne neispravnu) opremu unutar kabineta. |
| Šta je tim odbacio | Prijedlog za prikaz historije rezervacija kabineta na istoj stranici – ostavljeno za buduću iteraciju. |
| Rizici, problemi ili greške koje su uočene | Potrebno paziti na autorizaciju – pristup detaljima kabineta treba biti ograničen na ovlaštene korisnike. |
| Ko je koristio alat | Aner Atović |