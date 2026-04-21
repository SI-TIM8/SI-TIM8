

|ID|Opis rizika|Uzrok|Vjerovatnoća|Uticaj|Prioritet rizika|Plan mitigacije|Odgovorna osoba /uloga|Status|
|-|-|-|-|-|-|-|-|-|
|001|Neautoriziran pristup admin panelu|Loše implementirane sesije|Niska|Visok|Visok|Redovni sigurnosni testovi|Backend tim|Otvoren|
|002|Pad sistema|Preopterećenje servera|Niska|Visok|Visok|Backup sistem|DevOps tim|Otvoren|
|003|Brisanje podataka|Napad na sistem; ljudska greška;   Kvar na hardveru|Niska|Visok|Visok|Korištenje cloud servisa|Security tim|Otvoren|
|004|Rezervacija u isto vrijeme|Dva studenta u istoj sekundi zatraže  isti termin kada je dostupno još jedno mjesto|Srednja|Nizak|Srednji|Brojevni prikaz koliko je termina zauzeto u datom trenutku; Pri podnošenju zahtjeva student dobija notifikaciju da je termin skoro pun i da odabere drugi.|Backend tim|Identifikovan|
|005|Prelazak limita|Logička greška u kodu|Srednja|Srednji|Srednji|Provjera broja termina se vrši direktno u bazi prije potvrde rezervacije|Backend tim|Otvoren|
|006|Pogrešna evidencija kvara|Pogrešno prijavljen kvar|Visoka|Srednji|Srednji|Omogućiti studentu da prijavi kvar direktno kroz aplikaciju što se odmah prijavljuje tehničaru i blokira opremu od potencijalnog narednog rezervisanja|Laboratorijski tehničar / Backend tim|Otvoren|
|007|Pogrešan unos uloge korisnika|Admin prijavi studenta kao profesora ili obratno|Niska|Visok|Visok|Validacija svih korisnika; Evidencija svih promjena u sistemu|Backend tim|Otvoren|
|008|Brisanje rezervisane opreme|Laboratorijski tehničar obriše opremu koja je rezervisana|Srednja|Srednji|Srednji|Postavljen status opreme: dostupna, nedostupna, rezervisana i onemogućavanje brisanje rezervisane opreme|Backend tim|Otvoren|
|009|Nedostupnost developera|Developer koji je zadužen da odradi određeni dio nije dostupan.|Srednja|Visok|Visok|Korištenje GitHub-a tako da svi developeri imaju uvid u rad; redovni sastanci; održavanje dokumentacije|Project Manager|Identifikovan|
|010|Visoka stopa frikcije u korisničkom toku|Korisnici ne žele da koriste sistem zbog pretjerane složenosti; Ne žele mijenjanje inicijalnog sistema|Srednja|Visok|Srednji|Jednostavan i intuitivan dizajn aplikacije bez nepotrebnih animacija|Frontend tim|Identifikovan|
|011|Iznenadno dodavanje novih zahtijeva na kraju razvoja|Nedostatak jasnog dogovora na početku|Visoka|Srednji|Srednji|Precizno dogovoren zadatak, potpisan ugovor; Svaki novi zahtjev dovodi do produženja roka |Project Manager|Identifikovan|
|012|Curenje ličnih podataka studenata|Pristup podacima neovlaštenim osobama|Srednja|Srednji|Visok|Samo su dostupni nužni podaci; Enkripcija lozinki i drugih osjetljivih podataka|Security tim|Otvoren|
|013|Nepostojanje offline-first arhitekture i mehanizma za oporavak od katastrofe|Nestanak struje; Hardverski kvar|Visoka|Visok|Visok|Backup sistem|Security tim|Identifikovan|
|014|Korištenje nepravilne opreme|Nije evidentiran kvar opreme / ljudska pogreška|Srednja|Nizak|Nizak|Omogućavanje studenata da prijave kvar što automatski označava opremu kao nedostupnu|Laboratorijski tehničar|Otvoren|
|015|Nedostatak validacijskih pravila u modulu za upravljanje terminima|Student kasno otkaže rezervaciju pa termin ostane slobodan|Niska|Nizak|Nizak|Uvođenje da se ne može otkazati u 24h prije termina|Asistent / Profesor|Identifikovan|
|016|Nedostatak automatizovanog event-driven sistema notifikacija|Student se ne pojavi na rezervisanom terminu|Srednja|Nizak|Nizak|Sistem obavještava studente o zakazanom terminu; Oduzima studentu od njegovog limita za iznajmljivanje; Asistent / profesor evidentira njegovo odsustvo u sistem|Asistent / Profesor|Identifikovan|
|017|Suboptimalan sistem za upravljanje redovima čekanja i eskalaciju|Profesor ne odobri na vrijeme zatražene zahtjeve|Srednja|Nizak|Nizak|Sistem obavještava profesora o pristiglim zahtjevima|Asistent / Profesor|Identifikovan|
|018|Nekompatibilnost preglednika|Sistem se ne može pokrenuti na nekim preglednicima|Srednja|Srednji|Visok|Korištenje standardnih web tehnologija; testiranje na različitim preglednicima|Backend tim|Identifikovan|
|019|Korisnici ne mogu da se prijave u sistem|Pogrešno uneseni podaci|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|020|Student ne može da zatraži zahtjev za rezervaciju|Logička greška u kodu; bug|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|021|Profesor ne može da prihvati / odbije zahtjev|Logička greška u kodu; bug|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|022|Laboratorijski tehničar ne može da unese / pristupi opremi|Loše implementirane sesije; Bug|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|023|Student ne može da otkaže rezervisani termin|Logička greška u kodu; bug|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|024|Korisnici ne mogu da pristupe vlastitim podacima|Logička greška u kodu; bug|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|025|Onemogućeno pretraživanje opreme|Logička greška u kodu; Nije implementirana funkcionalnost|Niska|Srednji|Srednji|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|026|Profesor ne može da pregleda historiju zauzeća|Logička greška u kodu; Nije implementirana funkcionalnost|Niska|Visok|Visok|Testiranje funkcionalnosti prije objavljivanja|Backend tim|Otvoren|
|027|Neslaganje u timu|Nedovoljno dogovoren rad na sistemu|Srednja|Visok|Visok|Održavanje redovnih sastanaka|Project Manager|Identifikovan|
|028|Developerski tim nema potrebna znanja da funkcionalno odradi dio potrebnih zahtijeva|Zadužen nedovoljno iskusan tim; Koriste se nepoznate tehnologije|Srednja|Visok|Visok|Održavanje sastanaka; redovna komunikacija o iskustvu|Project Manager|Identifikovan|
|029|Ne postojanje načina za vraćanje izgubljene lozinke|Gubitak pristupa|Srednja|Visok|Visok|Mogućnost povratka pristupa preko fakultetskog maila|Security tim|Otvoren|
|030|Neispravan prikaz dostupnosti opreme|Status opreme se ne mijenja u realnom vremenu|Niska|Srednji|Srednji|Uvođenje automatskog osvježavanja stanice |Backend tim|Otvoren|
|031|Preopterećenost tima|Previše zadataka za riješiti u kratkom vremenskom periodu |Srednja|Visok|Visok|Jasna podjela oblasti po članu|Project Manager|Identifikovan|



