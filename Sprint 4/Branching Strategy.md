## **Branching strategy**

Naš tim je za branching strategy odabrao jednostavni i efikasni **GitHub Flow**. 
GitHub Flow je jednostavan i lagan proces rada (workflow) zasnovan na granama (branches) koji omogućava timovima da redovno i sigurno isporučuju promjene u kodu.

## **Osnovni procesi**

* **Main grana spremna za objavu:** Main (ili master) grana uvijek sadrži kod koji je spreman za produkciju.  
* **Kratkotrajne feature grane:** Za svaki novi zadatak \- bilo da je riječ o novoj funkcionalnosti, ispravci greške ili eksperimentu kreira se nova grana koja se odvaja od main grane.   
* **Kontinuirana isporuka:** Promjene se obično spajaju (*merge*) i objavljuju odmah nakon što su spremne i pregledane.

## **Ključni koraci GitHub Flow procesa**

1. **Kreiranje grane:** Pokreće se nova grana iz main grane kako bi se rad izolovao.  
2. **Pravljenje izmjena:** Developeri rade na svojoj grani i redovno šalju (push) promjene.  
3. **Kreiranje Pull Request-a:** Kada je posao završen (ili čak i ranije, za feedback), otvara se Pull Request kako bi se tim obavijestio da su promjene spremne za pregled ili kako bi se dobile rane povratne informacije.  
4. **Pregled i diskusija:** Tim pregleda kod, diskutuje o rješenjima i vrši testiranja.  
5. **Testiranje i objava:** Tim objavljuju promjene iz feature grane u testno (*staging*) ili produkcijsko okruženje radi konačne provjere prije spajanja.  
6. **Spajanje i brisanje grane:** Nakon odobrenja, grana se spaja u main granu, a zatim briše kako bi repozitorij ostao uredan.

## **Imenovanje grana**

Za lakšu organizaciju i preglednost repozitorija, koristi se standardizovani način imenovanja grana. Svaka grana počinje prefiksom koji označava tip zadatka:

* feature/naziv-funkcionalnosti (npr. feature/login-page)  
* bugfix/opis-greske (npr. bugfix/payment-error)  
* hotfix/opis-kriticnog-problema (npr. hotfix/crash-on-start)

## **Kada je kod spreman za produkciju**

Kod se smatra spremnim za produkciju tek kada su ispunjeni sljedeći uslovi:

* svi unit testovi uspješno prolaze  
* kod je pregledan i odobren od najmanje 2 člana tima  
* funkcionalnost je ručno testirana na staging okruženju  
* nema poznatih kritičnih grešaka

Pregledom koda se podrazumijeva provjera čitljivosti, organizacije koda, ispravnosti implementacije zahtjeva te uočavanje potencijalnih grešaka i sigurnosnih problema.

## **Automatizacija testiranja i objave**

Kako bi se osigurala stabilnost koda, koristi se automatizovani CI/CD proces. Prilikom svakog push-a ili kreiranja Pull Request-a:

* automatski se pokreću unit testovi  
* provjerava se ispravnost build-a

Nakon što se promjene spoje u main granu aplikacija se automatski deploy-a na staging ili produkcijsko okruženje.

## **Staging okruženje**

Prije konačnog spajanja u main, promjene se deploy-aju na staging okruženje gdje se vrši dodatna provjera:

* testiranje funkcionalnosti u realnim uslovima  
* provjera integracije sa drugim dijelovima sistema  
* osnovno korisničko testiranje

## **Rad sa kritičnim greškama**

Za hitne probleme u produkciji koristi se poseban proces:

* kreira se hotfix grana direktno iz main  
* problem se odmah ispravlja  
* nakon odobrenja, promjena se direktno spaja u main i deploy-a

## **Rollback strategija**

U slučaju da se nakon objave pojavi problem vrši se:

* vraćanje na prethodnu stabilnu verziju (git revert)  
* brzo kreiranje hotfix grane za ispravku problema

## **Verzionisanje i release-ovi**

Za praćenje verzija aplikacije koristi se verzionisanje putem tagova (npr. v1.0, v1.1, v2.0). 
Svaka stabilna verzija koja se objavljuje u produkciji dobija svoj tag, što omogućava lakše praćenje promjena i eventualni povratak na starije verzije.

## **Benfiti korištenja**

* Najpogodniji za projekte koji imaju jednu produkcijsku verziju i česta ažuriranja.  
* GitHub Flow prisiljava tim na Pull Requestove.   
* Smanjuje složenost upravljanja u poređenju sa složenijim modelima poput Git Flow-a.  
* Brza ispravka grešaka: proces ispravke je identičan procesu razvoja gdje je potrebno kreiranje posebne grane, ispravka i spajanje.  
* Idealan za okruženja u kojima kod treba stići do korisnika što je brže moguće.

## **Potencijalni problemi i rizici**

Iako jednostavan, GitHub Flow nosi određene izazove:

* Rizik od spajanja nedovoljno testiranog koda u **main** granu.  
* Velika ovisnost o kvalitetnim automatiziranim testovima.  
* Ograničena pogodnost za projekte koji zahtijevaju održavanje više verzija aplikacije.  
* Mogućnost pojave konflikata prilikom spajanja grana (*merge conflicts*).  
* Potreba za visokim nivoom discipline i koordinacije unutar tima.  
* Rizik od prečestih objava bez dovoljno detaljnog testiranja.

## **Zaključak za naš tim**

GitHub Flow se zasniva na jednoj stabilnoj grani (**main**) i kratkotrajnim feature granama, što olakšava upravljanje konfliktima pri spajanju koda. 
Ovakav pristup omogućava timu da se fokusira na razvoj funkcionalnosti umjesto na upravljanje granama, uz zadržavanje dobrih razvojnih praksi poput obaveznog pregleda koda putem Pull Requestova prije svakog spajanja u **main**. 
S obzirom na obim i trajanje projekta, ova strategija pruža dovoljan nivo kontrole bez nepotrebne složenosti.
