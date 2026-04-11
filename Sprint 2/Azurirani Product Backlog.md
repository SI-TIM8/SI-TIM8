# Azurirani Product Backlog

| ID | Naziv stavke | Kratak opis | Tip stavke | Prioritet | Procjena (Story Points) | Status | Sprint/Release | Napomena |
|----|--------------|-------------|------------|-----------|------------------------|--------|----------------|----------|
| 01 | Definisati problem i ciljeve sistema | Razraditi problem koji sistem rješava i osnovne ciljeve proizvoda | research | High | 3 | Done | Sprint 1 | |
| 02 | Izraditi Product Vision | Definisati viziju sistema i vrijednost za korisnike | documentation | High | 5 | Done | Sprint 1 | |
| 03 | Identifikovati stakeholdere sistema | Prepoznati sve ključne stakeholder grupe | research | High | 3 | Done | Sprint 1 | |
| 04 | Izraditi Stakeholder Map | Vizualizacija stakeholdera i njihovih interesa | documentation | High | 3 | Done | Sprint 1 | |
| 05 | Napraviti početni Product Backlog | Izraditi početnu verziju backlog-a | documentation | High | 5 | Done | Sprint 1 | |
| 06 | Razraditi prioritetne user storyje | Definisati ključne user storyje za MVP | documentation | High | 5 | To Do | Sprint 2 | |
| 07 | Definisati acceptance criteria | Napisati mjerljive kriterije za storyje | documentation | High | 5 | To Do | Sprint 2 | |
| 08 | Uraditi prioritizaciju backloga | Sortirati backlog po poslovnoj vrijednosti | research | High | 3 | To Do | Sprint 2 | |
| 09 | Definisati početne NFR zahtjeve | Identifikovati sigurnosne, performansne i UX zahtjeve | documentation | Medium | 3 | To Do | Sprint 2 | |
| 10 | Izraditi Risk Register | Identifikovati rizike i plan mitigacije | documentation | Medium | 3 | To Do | Sprint 3 | |
| 11 | Napraviti Domain Model / Use Case Model | Modelovati entitete i odnose sistema | documentation | High | 5 | To Do | Sprint 3 | |
| 12 | Izraditi Architecture Overview | Definisati arhitekturu sistema | documentation | High | 5 | To Do | Sprint 3 | |
| 13 | Pripremiti Test Strategy | Definisati pristup testiranju | documentation | Medium | 3 | To Do | Sprint 3 | |
| 14 | Definisati Definition of Done | Dogovoriti kriterije završetka taska | documentation | Medium | 2 | To Do | Sprint 4 | |
| 15 | Napraviti Initial Release Plan | Planirati inkremente i MVP | documentation | Medium | 3 | To Do | Sprint 4 | |
| 16 | Razrada modela podataka sistema | Definisati strukturu baze i relacije između entiteta | documentation | Medium | 3 | To Do | Sprint 3 | |
| 17 | Postavka tehničkog skeletona sistema | Inicijalizacija projekta, repozitorija i osnovne strukture sistema | technical task | High | 5 | To Do | Sprint 4 | |
| 18 | Autentifikacija – Login i tokeni | Implementirati login formu i JWT token autentifikaciju | feature | High | 3 | To Do | Sprint 5 | |
| 19 | Autentifikacija – Role based | Implementirati role-based access control (RBAC) za sve uloge | feature | High | 3 | To Do | Sprint 5 | |
| 20 | Autentifikacija – Logout i sesija | Implementirati odjavu i upravljanje sesijom | feature | High | 2 | To Do | Sprint 5 | |
| 21 | Upravljanje korisnicima | Administrator kreira i upravlja korisnicima | feature | High | 5 | To Do | Sprint 6 | Neophodno da bi sistem imao korisnike |
| 22 | Upravljanje kabinetima | Administrator vrši dodavanje i upravljanje kabinetima | feature | High | 3 | To Do | Sprint 6 | |
| 23 | Upravljanje inventarom – CRUD opreme | Admin dodaje, uređuje i briše laboratorijsku opremu; vezuje opremu za kabinet | feature | High | 5 | To Do | Sprint 6 | |
| 24 | Upravljanje inventarom – status dostupnosti | Administrator upravlja statusom dostupnosti laboratorijske opreme | feature | High | 4 | To Do | Sprint 6 | |
| 25 | Definisanje radnog vremena kabineta | Admin definiše radno vrijeme i trajanje termina | feature | High | 5 | To Do | Sprint 6 | Na osnovu radnog vremena kabineta i trajanja termina sistem pravi raspored termina |
| 26 | Upravljanje terminima | Tehničar definiše, mijenja i briše termine za pojedinačne laboratorije | feature | Medium | 3 | To Do | Sprint 6 | |
| 27 | Blokiranje kabineta | Administrator blokira korištenje kabineta u određenim periodima (praznici, čišćenje...) | feature | Medium | 3 | To Do | Sprint 6 | |
| 28 | Pregled termina – Kalendar po kabinetu | Prikaz kalendara slobodnih termina po kabinetu | feature | High | 3 | To Do | Sprint 7 | |
| 29 | Pregled termina – Dostupnost opreme | Prikaz dostupnosti opreme u realnom vremenu | feature | High | 3 | To Do | Sprint 7 | |
| 30 | Pregled termina – Kombinovani prikaz | UI za kombinirani prikaz termina i dostupnosti opreme | feature | High | 2 | To Do | Sprint 7 | |
| 31 | Odabir termina i podnošenje rezervacije | Korisnik bira kabinet, slobodan termin i podnosi zahtjev za rezervaciju | feature | High | 3 | To Do | Sprint 7 | Oprema se bira unutar jednog kabineta |
| 32 | Odabir opreme unutar rezervacije | Korisnik odabire željenu opremu iz kabineta kao dio zahtjeva za rezervaciju | feature | Medium | 2 | To Do | Sprint 7 | |
| 33 | Validacija konflikta termina | Sprječavanje rezervacija koje se vremenski preklapaju | technical task | High | 5 | To Do | Sprint 8 | |
| 34 | Pregled zahtjeva za rezervaciju (profesor/asistent) | Profesor/asistent vidi listu pristiglih zahtjeva s detaljima (termin, korisnik, oprema) | feature | High | 2 | To Do | Sprint 8 | Status rezervacije se može mijenjati do početka termina |
| 35 | Odobravanje / odbijanje rezervacije | Profesor/asistent odobrava ili odbija zahtjev; status se mijenja do početka termina | feature | High | 3 | To Do | Sprint 8 | |
| 36 | Ažuriranje statusa opreme (tehničar) | Tehničar ažurira status opreme | feature | High | 3 | To Do | Sprint 9 | |
| 37 | Automatska promjena statusa opreme po rezervaciji | Sistem automatski označava opremu kao zauzetu tokom potvrđene rezervacije i oslobađa je po završetku | technical task | Medium | 2 | To Do | Sprint 9 | |
| 38 | Ograničenja broja rezervacija | Profesor/asistent ograničava broj termina koji student može zauzeti | Medium | 2 | To Do | Sprint 9 | |
| 39 | Pregled i otkazivanje vlastitih rezervacija | Student može pregledati i otkazati svoje rezervacije i zahtjeve za rezervaciju | feature | Medium | 3 | To Do | Sprint 9 | Status rezervacije ili zahtjeva se može mijenjati do početka termina |
| 40 | Filtriranje i pretraga opreme | Pretraga opreme po nazivu i kabinetu | feature | Medium | 3 | To Do | Sprint 10 | |
| 41 | Upravljanje korisničkim profilom | Korisnik može promijeniti lozinku i podatke | feature | Low | 3 | To Do | Sprint 10 | Korisnik inicijalno koristi lozinku dodijeljenu od administratora |
| 42 | Historija korištenja opreme | Tehničar i administrator imaju uvid u historiju korištenja opreme | feature | Low | 3 | To Do | Sprint 10 | Nije u MVP |
| 43 | Prijava kvara | Korisnik (student ili profesor/asistent) može prijaviti kvar tehničaru | feature | Medium | 3 | To Do | Sprint 10 | |
| 44 | Potvrda i obrada kvara | Tehničar obrađuje prijavu kvara | feature | Medium | 3 | To Do | Sprint 10 | Povezano sa statusom opreme |
