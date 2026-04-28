# AI Usage Log – Sprint 5

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

