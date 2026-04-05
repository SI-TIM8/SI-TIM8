# Nefunkcionalni zahtjevi
---
| ID | Kategorija | Opis zahtjeva | Način provjere | Prioritet | Napomena |
|----|-----------|--------------|--------------------|-----------|----------|
| NFR-01 | Upotrebljivost | Sistem mora omogućiti studentima intuitivan pregled slobodnih termina i jednostavnu rezervaciju opreme bez tehničke obuke. | Testiranje sa korisnicima (studentima) i mjerenje broja koraka potrebnih za rezervaciju. | High | Fokus na jednostavnost |
| NFR-02 | Upotrebljivost | Profesori i asistenti moraju imati jasan pregled zahtjeva sa opcijama za odobravanje ili odbijanje rezervacija. | UI testiranje i provjera funkcionalnosti odobravanja. | High | - |
| NFR-03 | Upotrebljivost | Tehničari moraju moći ažurirati status opreme kroz minimalan broj koraka. | Mjerenje vremena i broja klikova za promjenu statusa. | Medium | - |
| NFR-04 | Upotrebljivost | Korisnički interfejs mora biti responzivan i prilagođen različitim veličinama ekrana. | Testiranje na različitim uređajima i rezolucijama. | Medium | Web aplikacija |
| NFR-05 | Sigurnost | Pristup sistemu mora biti omogućen samo autentifikovanim korisnicima. | Testiranje login sistema i pokušaji neovlaštenog pristupa. | High | - |
| NFR-06 | Sigurnost | Sistem mora implementirati role-based pristup kako bi korisnici imali pristup samo dozvoljenim funkcijama. | Testiranje različitih korisničkih uloga. | High | JWT token |
| NFR-07 | Sigurnost | Lozinke korisnika moraju biti sigurno pohranjene. | Provjera hashiranja lozinki u bazi. | High | - |
| NFR-08 | Privatnost | Neovlašteni korisnici ne smiju imati pristup podacima o rezervacijama i opremi. | Testiranje pristupa podacima bez autentifikacije. | High | - |
| NFR-09 | Dostupnost | Sistem mora biti dostupan najmanje 99.9% vremena tokom radne sedmice. | Monitoring uptime-a sistema. | Medium | - |
| NFR-10 | Kompatibilnost | Sistem mora raditi u modernim web preglednicima (Chrome, Firefox, Edge). | Testiranje u različitim browserima. | Medium | - |
