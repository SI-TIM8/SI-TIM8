# Nefunkcionalni zahtjevi

## 1.1 Upotrebljivost sistema

Sistem će biti dizajniran s primarnim fokusom na jednostavnost i brzinu pristupa za sve četiri identifikovane grupe korisnika.

- **Za studente:** Interfejs mora omogućiti intuitivan pregled slobodnih termina i brzu rezervaciju opreme bez potrebe za tehničkom obukom.
- **Za profesore i asistente:** Omogućit će se pregledan uvid u sve pristigle zahtjeve sa jasnim opcijama za odobravanje ili odbijanje rezervacija.
- **Za tehničare:** Fokus će biti na efikasnom ažuriranju statusa opreme (ispravna / neispravna) kroz minimalan broj interakcija.
- **Responzivnost:** Budući da je primarna platforma web preglednik, korisnički interfejs mora biti responzivan kako bi osigurao konzistentno iskustvo na različitim veličinama ekrana.
---
## 1.2 Sigurnost sistema

Sigurnost sistema osigurat će se kroz strogu kontrolu pristupa i zaštitu integriteta podataka.

- **Autentifikacija i autorizacija:** Pristup sistemu bit će omogućen isključivo putem unaprijed kreiranih korisničkih računa. Implementirat će se role-based access control (RBAC) koristeći JWT tokene, čime se osigurava da korisnici vide samo funkcionalnosti koje odgovaraju njihovoj ulozi.
- **Zaštita lozinki:** Sve korisničke lozinke moraju biti sigurno pohranjene korištenjem savremenih algoritama za enkripciju.
- **Privatnost podataka:** Neovlašteni korisnici neće imati uvid u detalje rezervacija niti u bazu laboratorijske opreme.

