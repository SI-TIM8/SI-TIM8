# **Test Strategy**

***Sistem za upravljanje laboratorijskom opremom***

## **Cilj testiranja**

| Testni cilj i referenca | Obuhvat testiranja | Očekivani rezultat / Uslov za prolaz |
| :---- | :---- | :---- |
| **Provjera autentifikacije i autorizacije (RBAC)** (US29, US30, NFR-06) | Prijava putem JWT tokena, automatski istek sesije i ograničenje pristupa za uloge: Administrator, Tehničar, Profesor, Student | Korisnik sa ulogom "Student" dobija 403 Forbidden pri pokušaju pristupa admin rutama; lozinke su ispravno hesirane (NFR-07) |
| **Validacija procesa rezervacije bez konflikata** (US11, US26, NFR-16) | Kreiranje rezervacije termina i specifične opreme; provjera Database Locks mehanizma | Sistem fizički onemogućava dvije rezervacije za isti ID opreme u istom terminu; student dobija jasnu poruku o zauzetosti |
| **Upravljanje statusom opreme i prijava kvarova** (US08, US09, US28) | Promjena statusa opreme (dostupno/neispravno); automatsko otkazivanje budućih rezervacija nakon prijave kvara | Prijava kvara traje \< 10s (NFR-03); pogođeni korisnici automatski primaju notifikaciju o otkazivanju |
| **Provjera performansi i responzivnosti** (NFR-02, NFR-04, NFR-13) | Učitavanje profesorskog dashboarda; rad sistema pod opterećenjem od 50 istovremenih korisnika | Dashboard se učitava za \< 1s; UI je funkcionalan na rezolucijama od 360px do 1920px |
| **Integritet podataka i backup** (NFR-17, NFR-19) | ACID transakcije pri upisu; automatski incremental backup baze podataka | U slučaju pada, sistem se vraća u stabilno stanje u roku od 500ms; backup se izvršava svaka 24h u 03:00 AM |

## **Nivoi testiranja**

| Faza testiranja | Predmet provjere | Zaduženi tim | Kriterijum završetka |
| :---- | :---- | :---- | :---- |
| **Unit testiranje** | Validacija poslovne logike: provjera formata emaila, validacija dužine lozinke, logika računanja slobodnih termina i provjera limita rezervacija (US16) | Dev tim | Minimalno 80% pokrivenosti kritične logike rezervacija (NFR-12) |
| **Integracijsko testiranje** | Komunikacija API-ja sa bazom (PostgreSQL/MySQL); provjera JWT tokena u Headeru; automatska promjena statusa opreme u bazi nakon US09 | Dev \+ QA | API endpointi vraćaju 401 Unauthorized bez validnog tokena u roku od 100ms (NFR-05) |
| **Sistemsko testiranje** | E2E tokovi: Student rezerviše \-\> Profesor odobrava \-\> Tehničar prijavljuje kvar \-\> Sistem otkazuje rezervaciju | QA tim | Svi prioritetni "High" storyji prolaze bez blokera; poslovna pravila (1-24) su ispoštovana |
| **UI testiranje** | Provjera "Calendar View" prikaza (US24); filteri opreme (US10); responzivnost elemenata na mobilnim uređajima | QA tim | Rezervacija završena u max 4 klika (NFR-01); vizuelni indikatori za blokirane termine su jasni |
| **Sigurnosno testiranje** | RBAC autorizacija; zaštita od SQL injection-a; enkripcija osjetljivih podataka studenata (JMBG) (NFR-08) | Security tim | Student ne vidi lične podatke drugih studenata; administrativne rute su nedostupne bez Admin uloge |
| **Performansno testiranje** | Load testing sa 50 concurrent korisnika; mjerenje latencije upisa u bazu (\< 300ms) | QA \+ DevOps | Sistem održava odziv ispod 1s pri maksimalnom definisanom opterećenju (NFR-13) |
| **Testiranje prihvatljivosti** | Provjera usklađenosti sa Product Vision-om i MVP opsegom; validacija na BHS jezicima (NFR-18) | Product Owner | Svi kriteriji iz "Definition of Done" su ispunjeni; korisnici (studenti/profesori) potvrđuju upotrebljivost |

## **Šta se testira u kojem nivou**

| Grupa funkcionalnosti (Referenca) | Unit test. | Integracioni nivo | Sistemski E2E | UI/UX provjera | Sec/RBAC | Load/Perf | UAT |
| :---- | :---- | :---- | :---- | :---- | :---- | :---- | :---- |
| **Upravljanje pristupom** (US01-US05, US29-US32) | DA | DA | DA | DA | DA | DA | DA |
| **Rezervisanje opreme i termina** (US11, US25, US26) | DA | DA | DA | DA | DA | DA | DA |
| **Prijava i obrada kvarova** (US09, US28) | DA | DA | DA | DA | NE | NE | DA |
| **Upravljanje kabinetima i resursima** (US22, US23) | DA | DA | DA | DA | DA | NE | DA |
| **Kalendarski prikaz i filtriranje** (US10, US24) | NE | DA | DA | DA | NE | DA | DA |
| **Odobravanje/Odbijanje zahtjeva** (US15, US27) | DA | DA | DA | DA | DA | NE | DA |
| **Lokalizacija (BHS jezici)** (NFR-18) | NE | NE | DA | DA | NE | NE | DA |

## **Veza sa acceptance kriterijima**

| Povezani User Story/NFR | Opis AC-a | Faze u kojima se testira | Artefakt (Testni dokaz) |
| :---- | :---- | :---- | :---- |
| **US01, US30** | Administrator kreira korisnika; Student ne vidi meni "Upravljanje korisnicima" | Sistemsko, Sigurnosno | Izvještaj o testiranju uloga (Role-Permission Matrix); 403 status kod na admin rutama |
| **US11, US26** | Onemogućeno preklapanje termina i rezervacija opreme u prošlosti | Unit, Integracijsko, Sistemsko | Unit testovi za datumsku logiku; DB lock logovi pri pokušaju konkurentnog upisa |
| **US09, US28** | Automatska promjena statusa opreme na "neispravna" i otkazivanje rezervacija | Integracijsko, Sistemsko | Zapis u bazi nakon prijave kvara; logovi poslatih notifikacija pogođenim studentima |
| **NFR-01** | Rezervacija slobodnog termina u maksimalno 4 klika | UI, Sistemsko | Time-to-task video zapis ili report sa brojem klikova (Click-stream analysis) |
| **NFR-07** | Lozinke hesirane koristeći Argon2 ili BCrypt (min 16 byte salt) | Sigurnosno | Inspekcija baze podataka (hash string provjera); Code review kriptografskog modula |
| **US24** | Kalendarski prikaz sa različitim bojama za slobodne/zauzete/blokirane termine | UI | Vizuelna validacija (Screenshotovi) na različitim rezolucijama (360px-1920px) |

## **Način evidentiranja rezultata testiranja**

| Vrijeme | Tiket/Zahtjev | Predmet testa | Tip testa | Izvještaj/Dokaz | Status prolaznosti | Prijavljeni bug | Dodatni kontekst |
| :---- | :---- | :---- | :---- | :---- | :---- | :---- | :---- |
| 28-03-2026 | US26, NFR-16 | Spriječavanje duple rezervacije opreme | Integracijsko | JMeter log / SQL log | **FAIL** | BUG-28-03-2026-001 | Trka (Race condition) pri 5 paralelnih zahtjeva |
| 29-03-2026 | US30, NFR-06 | Zabrana pristupa studentu admin panelu | Sigurnosno | Postman Report | **PASS** | N/A | Vraćen 403 Forbidden za /api/admin/\* |

**Notacija defekata:** BUG-\[DD-MM-YYYY\]-\[sekvenca\].

**Severity skala:** S1 (Bloker), S2 (Kritičan), S3 (Visok), S4 (Srednji), S5 (Nizak).

**Alat:** GitHub Issues (labeli: bug, severity:SX, sprint:X).

## **Glavni rizici kvaliteta**

| Identifikovani rizik | Nivo ozbiljnosti | Mogućnost pojave | Strategija rješavanja (Mitigacija) |
| :---- | :---- | :---- | :---- |
| **R-01: Konflikt konkurentnih rezervacija** — Dva studenta u istoj sekundi rezervišu zadnji komad opreme (Rizik 004). | Visok | Srednja | Implementacija Database Locks (Pessimistic locking) i testiranje kroz konkurentne API skripte |
| **R-02: Neispravna autorizacija (Privilege Escalation)** — Student dobija pristup listi svih korisnika i njihovim JMBG podacima (NFR-08). | Visok | Niska | Striktna Middleware provjera JWT uloga na svakom endpointu; automatizovani "negative" sigurnosni testovi |
| **R-03: Gubitak podataka pri padu sistema** — Pad fakultetske mreže briše trenutne sesije ili zapise o rezervacijama (Rizik 013). | Visok | Visoka | Korištenje ACID transakcija; incremental backup svaka 24h; testiranje oporavka (Recovery Testing) unutar 500ms |
| **R-04: Loše korisničko iskustvo (Složenost)** — Korisnici odbijaju sistem jer je proces rezervacije predug (Rizik 010). | Srednji | Srednja | Pridržavanje NFR-01 (max 4 klika) i provođenje testiranja upotrebljivosti sa fokusnom grupom studenata |
| **R-05: Nekompatibilnost preglednika** — Sistem ne radi ispravno na starijim verzijama Safarija ili Firefoxa (NFR-10). | Srednji | Srednja | Cross-browser testiranje (Chrome v110+, Firefox v105+, Safari v15+) pomoću automatizovanih UI alata |

