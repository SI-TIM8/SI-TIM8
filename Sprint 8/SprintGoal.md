### Sprint Goal - Sprint 8

Glavni fokus osmog sprinta je na postizanju potpune automatizacije i pouzdanosti sistema rezervacija kroz implementaciju naprednih mehanizama validacije i komunikacije. Ključni prioritet tima je eliminacija ljudske greške uvođenjem automatske validacije konflikata, čime će se spriječiti preklapanje termina i osigurati da ista oprema ne bude rezervisana dva puta u istom periodu. Uporedo s tim, radit će se na jačanju interakcije s korisnicima putem integrisanog sistema notifikacija unutar aplikacije i putem e-maila, čime će studenti i osoblje biti trenutno obaviješteni o svakoj promjeni statusa njihovih zahtjeva.

Pored komunikacijskog aspekta, sprint obuhvata i unapređenje dinamičkog upravljanja resursima, gdje će sistem samostalno ažurirati dostupnost opreme na osnovu potvrđenih rezervacija. Kako bi se osigurala stabilnost cijelog ekosistema, finalni dio sprinta biće posvećen rigoroznom E2E i integracijskom testiranju kompletnog workflowa. Time ćemo potvrditi da svi razvijeni moduli – od inicijalne prijave studenta, preko validacije i notifikacija, do automatske promjene statusa opreme – funkcionišu kao neraskidiva i sigurna cjelina spremna za krajnje korisnike.

#### Očekivani deliverables:
* Algoritam za automatsku validaciju konflikata koji u realnom vremenu blokira pokušaje duplih rezervacija iste opreme.
* Integrisani notifikacijski servis koji šalje automatske e-mailove i in-app obavijesti studentima i osoblju.
* Modul za dinamičko upravljanje resursima koji automatski oslobađa ili zauzima opremu na osnovu životnog ciklusa rezervacije.
* Sveobuhvatni E2E (End-to-End) testni izvještaj koji potvrđuje da čitav proces, od prve prijave do finalnog razduživanja opreme.