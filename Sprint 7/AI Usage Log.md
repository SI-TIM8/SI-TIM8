# AI Usage Log – Sprint 7

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
