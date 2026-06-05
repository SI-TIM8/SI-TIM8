## Final AI Usage Summary

---

### 1. Za šta je AI korišten 
*AI alati (Claude/Claude Sonnet, GitHub Copilot, ChatGPT, Gemini, Lovable AI) korišteni su kroz cijeli razvojni ciklus tokom sprinta za:*

* **Frontend razvoj:** Izrada korisničkih interfejsa (Login UI sa validacijom, kalendarski prikaz rezervacija sa color-coding sistemom, ekrani za profile, promjenu lozinke, "Moje rezervacije" s tabovima, vizualizacija stanja opreme kroz grafikone, te Dark/Light mod).
* **Backend logika i sigurnost:** Implementacija JWT autentifikacije, RBAC logike (kontrola pristupa na osnovu uloga), logout mehanizama, automatskog isteka sesije, te logike za verifikaciju email adresa i obavezne promjene lozinke pri prvom loginu.
* **Core biznis logika (CRUD operacije):** Razvoj sistema za upravljanje korisnicima (CRUD sa soft-delete), opremom (CRUD sa statusima, arhiviranjem i uploadom PDF dokumentacije/URL-ova), kabinetima, te upravljanje terminima i rezervacijama (sa provjerom konflikata i preklapanja).
* **Automatizacija, testiranje i DevOps:** Pisanje unit, integracijskih i E2E testova, konfiguracija Docker-a i CI/CD pipeline-a, dokumentovanje API endpointa, te generisanje skripti za load testiranje (k6) i provjeru NFR zahtjeva (NFR-13, NFR-17, NFR-19).
* **Upravljanje kvarovima i obavijestima:** Implementacija toka prijave kvara od strane profesora s automatskom promjenom statusa opreme, logike za automatsko otkazivanje rezervacija pogođenih kvarom, te sistema masovnih in-app obavijesti (automatskih i ručnih) vidljivih kroz zvono za obavijesti.
* **Napredne korisničke funkcionalnosti:** Implementacija ekrana "Moje rezervacije" s tabovima za aktivne rezervacije i zahtjeve, logike za otkazivanje rezervacija i povlačenje zahtjeva na čekanju, ograničenja broja aktivnih zahtjeva po studentu, arhiviranja opreme (soft delete) s filterima i vraćanjem iz arhive, te uploada PDF dokumentacije i URL linkova uz opremu.
* **Korisnički profil i email infrastruktura:** Razvoj toka verifikacije email adrese, sistema podsjetnika prije termina (konfigurabilni intervali, razlikovanje verificiranih i neverificiranih korisnika), stranice profila s prikazom i uređivanjem podataka, prikaza nedavnih aktivnosti, sigurnosnih email alertova pri promjeni osjetljivih podataka, oporavka lozinke putem email linka, filtriranja rezervacija po datumu i kabinetu s exportom u CSV i PDF, dark/light moda i vizualizacije stanja opreme kroz prstenasti grafikon.

---

### 2. Šta je prihvaćeno
*Tim je od AI-ja usvojio isključivo stabilne strukturalne i arhitekturalne temelje:*

* **Arhitektura koda i strukture:** Osnovne strukture komponenti, API endpointa, DTO modela, tablica u bazi podataka i JOIN logika za agregiranje podataka (npr. detalji kabineta s opremom i odgovornim profesorom u jednom pozivu).
* **Standardni algoritmi:** Bazični algoritmi za provjeru vremenskih intervala (konflikti termina), logika verifikacije tokena i query-ji za prebrojavanje zahtjeva - uz napomenu da se provjera limita mora izvoditi unutar izolovane transakcije radi sprječavanja race conditiona.
* **Testni okviri:** Osnovni testni scenariji, strukture testnih slučajeva (unit, integracijski i k6 load testovi).
* **Dizajnerski koncepti:** Osnovni raspored kalendara, logika za color-coding statusa, koncept trajnog pamćenja korisničkih preferenci (teme) i mehanizam soft-delete arhiviranja putem zastavice `archived`/`archived_at`.

---

### 3. Šta je izmijenjeno 
*Kako bi kod odgovarao specifičnim poslovnim pravilima i dizajnu projekta, tim je intervenisao u sljedećim segmentima:*

* **Vizuelni identitet:** Kompletan stil i izgled formi, raspored elemenata na stranicama i kontrast boja (Dark/Light mod) prilagođeni su UI/UX dizajnu aplikacije; posebno su korigovani problemi čitljivosti određenih komponenti u dark modu.
* **Biznis pravila i validacije:** Precizirane su definicije "aktivnih zahtjeva" (eksplicitno samo statusi *Na čekanju* i *Odobren*), dodani su specifični statusi opreme ("u kvaru", "ispravno", "servisirano"), dodane su dodatne backend i frontend validacije (za URL-ove, komentare, preklapanje termina i privilegije profesora), a lista opreme u prikazu detalja kabineta filtrirana je da prikazuje samo dostupnu opremu (ne arhiviranu niti neispravnu).
* **Sigurnosni parametri:** Promijenjena su defaultna podešavanja trajanja JWT tokena, prošireni su JWT claims-i, definisana strožija pravila složenosti lozinke i izuzeti su seedovani korisnici iz mehanizma obavezne promjene lozinke. Pored toga, osigurano je da token sesije ne daje pune privilegije dok korisnik ne postavi novu lozinku pri prvom loginu.
* **Podaci i performanse:** Testni podaci su prilagođeni arhitekturi, ograničen je broj aktivnosti koje se prikazuju na profilu, filtrirane su liste kabineta kako bi prikazivale samo dostupnu opremu, a za soft-delete arhiviranje dodano je polje `archived_at` za praćenje vremena arhiviranja uz čuvanje historije kvarova.

---

### 4. Šta je odbačeno 
*Tim je prepoznao i eliminisao nefunkcionalne, nesigurne ili previše kompleksne prijedloge:*

* **Nesigurne prakse:** Generički "default" tajni ključevi za JWT, automatska prijava nakon promjene lozinke (bez ponovne autentifikacije), slanje email podsjetnika neverifikovanim korisnicima, te slanje privremene lozinke emailom pri kreiranju naloga.
* **Destruktivne akcije u bazi:** Direktno (trajno) brisanje korisnika i opreme iz baze podataka (zamijenjeno sa soft-delete, odnosno arhiviranjem), te automatsko brisanje naloga bez prethodne verifikacije email adrese.
* **Prekomjerna kompleksnost:** Drag-and-drop pristup za kalendar, WebSocket realtime push za obavijesti (zamijenjen pollingom), slanje privremenih lozinki mailom, kontinuirani monitoring (Prometheus/Grafana), indeksiranje teksta iz PDF-a, konfigurabilni limit zahtjeva putem admin panela (limit je hardkodiran kao konstanta), paginacija unutar tabova "Moje rezervacije" (zamijenjena scrollable listom), prikaz historije rezervacija kabineta na stranici detalja kabineta (ostavljeno za buduću iteraciju), te odloženo ažuriranje statusa opreme putem background job-a pri prijavi kvara (zamijenjeno sinhronim ažuriranjem).
* **Loša optimizacija i UX:** Prečesto provjeravanje tokena (štetno za performanse), automatsko osvježavanje stranice nakon logout-a, automatski eksport nakon promjene filtera, automatsko arhiviranje opreme na osnovu neaktivnosti, automatsko prebacivanje teme bez korisničkog izbora, te prikaz aktivnosti i rezervacija drugih korisnika na profilu.

---

### 5. Koje greške je AI napravio
*Kritički osvrt na generisani kod otkriva nekoliko ključnih propusta AI alata koji su mogli ugroziti sistem:*

* **Sigurnosni propusti:** Predlaganje nesigurnih defaultnih ključeva, nedostatak validacije svih input podataka na backendu (potencijalne ranjivosti), generisanje tokena sesije koji bi dali pune privilegije prije promjene inicijalne lozinke, te predlaganje manipulacije ID-evima u zahtjevima bez provjere vlasništva nad resursom (npr. student koji pristupa tuđim rezervacijama).
* **Generičnost i nedostatak konteksta:** Predložene konfiguracije (Docker, CI/CD, arhitektura testova) i opisi API-ja bili su previše generički i neusklađeni sa stvarnom arhitekturom projekta. AI je često ignorisao specifična pravila sistema (npr. radno vrijeme laboratorije, specifične edge-case scenarije kod preklapanja, razliku između seedovanih i administratorski kreiranih korisnika).
* **Ignorisanje performansi i lokalizacije:** Predlagano je prečesto provjeravanje tokena, neučinkovit prikaz velike količine podataka bez paginacije/filtriranja, te masovni insert obavijesti umjesto batch obrade. Također, AI nije predvidio probleme sa prikazom bosanskih karaktera (Č, Ć, Š, Ž, Đ) u PDF/CSV eksportu, niti probleme čitljivosti određenih UI komponenti u dark modu.
* **Problemi sa concurrency (paralelnošću):** AI nije obezbijedio mehanizme za sprečavanje race-condition problema - ni kod simultanih rezervacija istog termina/opreme, ni kod paralelnih prijava kvara iste opreme od strane više korisnika, ni kod provjere limita aktivnih zahtjeva pri paralelnom slanju. U svim slučajevima tim je morao eksplicitno dodati transakcijsku izolaciju.
* **Nekonzistentnost pri rollbacku:** Kod automatske promjene statusa opreme pri rezervaciji, AI nije predvidio scenarij gdje rollback rezervacije ostavi opremu u pogrešnom statusu - potrebna je bila eksplicitna logika vraćanja statusa u slučaju neuspješne transakcije.

---

### 6. Dijelovi sistema razvijani uz AI pomoć
*Pregled funkcionalnosti i koncepata čija je izrada bila olakšana korištenjem AI alata:*

* **Validacija konflikata i preklapanja termina:** Kako tačno algoritam provjerava vremenske intervale na backendu i sprečava da se laboratorija ili oprema rezervišu u isto vrijeme.
* **JWT Middleware i RBAC zaštita ruta:** Kako funkcioniše middleware koji presreće zahtjeve, verifikuje JWT token, čita korisničke uloge (roles) i dozvoljava/odbija pristup određenim rutama.
* **Mehanizam "First Login" i sigurnosna stanja:** Kako middleware detektuje zastavicu `first_login` i na koji način blokira pristup ostatku aplikacije dok se ne postavi nova lozinka (uz objašnjenje zašto su izuzeti seedovani korisnici).
* **Soft Delete / Arhiviranje podataka:** Kako je modifikovan query za dobavljanje podataka (korištenje `archived` i `archived_at` zastavica) kako bi se osiguralo da se podaci ne brišu trajno, ali da se ne prikazuju u aktivnim listama, uz čuvanje historije kvarova i rezervacija za arhiviranu opremu.
* **Transakcijska provjera limita zahtjeva:** Objašnjenje query-ja koji broji aktivne zahtjeve studenta i zašto se ta provjera mora izvršiti unutar baze podataka kroz izolovanu transakciju (kako bi se spriječio race condition pri paralelnom slanju zahtjeva).
* **E2E i Load testiranje (k6):** Razumijevanje k6 skripti koje simuliraju 50 concurrent (paralelnih) korisnika i kako su testirana ACID svojstva baze podataka prilikom simulacije grešaka.
* **Ekran "Moje rezervacije" s tabovima:** Kako je strukturirana tabbed React komponenta s odvojenim tabovima za aktivne rezervacije i zahtjeve na čekanju, uključujući logiku zaštite koja sprječava studenta od manipulacije tuđim ID-evima i pristupa tuđim podacima.
* **Scheduler za podsjetnike prije termina:** Kako je implementiran mehanizam zakazivanja obavijesti (konfigurabilni intervali 24h i 1h) s razlikovanjem kanala isporuke - in-app za sve korisnike, email isključivo za one s verificiranom adresom.
* **Upload i prikaz dokumentacije uz opremu:** Kako je implementiran multipart/form-data upload za PDF fajlove s validacijom veličine (max 10MB) i tipa, URL validacijom regex pravilima na backendu i frontendU, te prikazom i preuzimanjem dokumentacije na stranici detalja opreme.

---