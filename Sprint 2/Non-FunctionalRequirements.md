# Nefunkcionalni zahtjevi

| ID | Kategorija | Opis zahtjeva | Način provjere | Prioritet | Napomena |
|:---|:---|:---|:---|:---|:---|
| **NFR-01** | Upotrebljivost | Student mora biti u mogućnosti da završi proces rezervacije slobodnog termina u **maksimalno 4 klika** od trenutka prijave. | Brojanje interakcija u testnom scenariju. | High | Eliminacija subjektivne "intuitivnosti" |
| **NFR-02** | Upotrebljivost | Profesori moraju imati dashboard koji se učitava za **< 1 sekundu** i nudi akcije odobravanja/odbijanja na jedan klik. | Mjerenje vremena učitavanja (Lighthouse). | High | Brz pregled zahtjeva |
| **NFR-03** | Upotrebljivost | Tehničar mora moći promijeniti status opreme (ispravno/kvar) za manje od **10 sekundi** ukupnog rada. | Time-to-task mjerenje. | Medium | Efikasnost tehničara |
| **NFR-04** | Upotrebljivost | UI mora biti prilagođen rezolucijama od 360px (mobilni) do 1920px (desktop) bez gubitka funkcionalnosti elemenata. | Manualni test na različitim uređajima. | Medium | Responzivni dizajn |
| **NFR-05** | Sigurnost | Svaki API zahtjev prema zaštićenim resursima bez validnog Bearer tokena mora vratiti **401 Unauthorized** u roku od 100ms. | Automatski sigurnosni testovi (Postman). | High | Obavezna autentifikacija |
| **NFR-06** | Sigurnost | Sistem mora onemogućiti pristup administrativnim rutama korisniku sa ulogom "Student" (vraćanje **403 Forbidden**). | Testiranje autorizacije po ulogama. | High | RBAC implementacija |
| **NFR-07** | Sigurnost | Lozinke moraju biti hesirane koristeći **Argon2 ili BCrypt** algoritam sa minimalnim salt-om od 16 bajta. | Inspekcija koda i baze podataka. | High | Sigurnost baze |
| **NFR-08** | Privatnost | Lični podaci studenata (JMBG, broj telefona) ne smiju biti vidljivi asistentima osim ako je zahtjev u statusu "Odobreno". | Inspekcija polja u UI-u i API odzivu. | High | Zaštita podataka |
| **NFR-09** | Dostupnost | Sistem mora imati mjerljivi uptime od **99.9%** (maksimalno 43 minute zastoja mjesečno). | Monitoring alati (UptimeRobot). | Medium | Visoka dostupnost |
| **NFR-10** | Kompatibilnost | Sistem mora postizati 100% funkcionalnosti na Chrome v110+, Firefox v105+ i Safari v15+. | Cross-browser testiranje. | Medium | Moderni preglednici |
| **NFR-11** | Održivost | Logika aplikacije mora biti razdvojena tako da izmjena u bazi podataka ne zahtijeva promjene u UI komponentama. | Arhitektonska revizija koda. | Medium | Razdvajanje slojeva |
| **NFR-12** | Održivost | Svaki novi Pull Request mora imati minimalno **80% pokrivenosti unit testovima** za kritičnu logiku rezervacija. | CI/CD izvještaj (Jest/Istanbul). | High | Kvalitet koda |
| **NFR-13** | Skalabilnost | Sistem mora podržati **50 istovremenih korisnika** (concurrent users) uz zadržavanje vremena odziva ispod 1 sekunde. | Load testing (k6 ili JMeter). | Medium | Kapacitet sistema |
| **NFR-14** | Performanse | API endpoint za listu dostupne opreme mora vratiti podatke u roku od **500ms** pri bazi od 1000 stavki. | Mjerenje latencije servera. | High | Brzina pretrage |
| **NFR-15** | Performanse | Procesiranje transakcije rezervacije (upis u bazu) mora biti završeno unutar **300ms**. | Profilisanje baze podataka. | High | Brzina upisa |
| **NFR-16** | Pouzdanost | Sistem mora koristiti **Database Locks** kako bi fizički onemogućio upis dvije rezervacije za isti ID opreme u istom terminu. | Testiranje konkurentnosti. | High | Integritet rezervacija |
| **NFR-17** | Pouzdanost | U slučaju pada sistema, integritet podataka se mora vratiti na zadnje stabilno stanje u roku od **500ms** putem ACID transakcija. | Testiranje oporavka od greške. | High | Konzistentnost podataka |
| **NFR-18** | Lokalizacija | Svi labeli, poruke o greškama i uputstva moraju biti na **BHS jezicima** (Bosanski/Hrvatski/Srpski). | Provjera i18n datoteka. | Medium | Jezička podrška |
| **NFR-19** | Backup | Sistem mora vršiti automatski **incremental backup** baze podataka svakih 24 sata (u 03:00 AM). | Provjera automatizovanih skripti. | High | Sigurnost od gubitka |
| **NFR-20** | Arhiviranje | Historija rezervacija starija od 2 godine mora biti automatski arhivirana u sekundarnu bazu radi očuvanja performansi. | Provjera scheduler zadataka. | Low | Optimizacija baze |
