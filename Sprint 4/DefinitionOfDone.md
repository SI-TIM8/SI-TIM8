# Definition of Done
## Sistem za upravljanje laboratorijskom opremom

Definition of Done predstavlja zajednički dogovor tima o tome koje uslove mora zadovoljiti svaka funkcionalnost, user story, tehnička stavka ili projektni artefakt prije nego što se označi kao završen.

---

## Funkcionalni kriteriji

| Oblast | Kriterij |
|---|---|
| **Zahtjevi** | Stavka je realizovana ili dokumentovana u skladu sa dogovorenim zahtjevima i scope-om sprinta |
| **Acceptance kriteriji** | Svi acceptance kriteriji povezani sa stavkom su provjereni i ispunjeni |
| **Proces sistema** | Funkcionalnost podržava odgovarajući tok sistema, kao što su prijava korisnika, upravljanje opremom, rezervacije, odobravanje termina ili prijava kvara |
| **Poslovna pravila** | Implementacija poštuje definisana pravila korištenja opreme, termina i kabineta |
| **Uloge korisnika** | Prava pristupa su usklađena sa ulogama u sistemu: student, profesor/asistent, laboratorijski tehničar i administrator |
| **Pregled tima** | Rješenje je pregledano unutar tima i usklađeno sa ostatkom projekta |
| **Pull request** | Pull request je pregledan, komentari su riješeni i promjene su spremne za spajanje |

---

## Testiranje i kvalitet

| Oblast | Kriterij | Ko provjerava |
|---|---|---|
| **Osnovno testiranje** | Provjereni su glavni scenariji korištenja i očekivano ponašanje funkcionalnosti | Član tima zadužen za implementaciju |
| **Negativni scenariji** | Testirani su neispravni unosi, prazna polja, nedozvoljene akcije i pokušaji pristupa bez odgovarajuće uloge | Član tima zadužen za testiranje |
| **Unit testovi** | Unit testovi su dodani gdje je primjenjivo i prolaze bez grešaka | Developer koji radi na funkcionalnosti |
| **Integracijski testovi** | Ključni tokovi između backend servisa, repozitorija i baze provjereni su kroz integracijsko testiranje kada je potrebno | Backend developer |
| **UI provjera** | Korisnički interfejs je provjeren za forme, poruke grešaka, navigaciju i prikaz podataka | Frontend developer ili član tima za QA |
| **Evidencija testova** | Rezultati testiranja su evidentirani u relevantnom dokumentu ili dogovorenom obliku | Član tima koji je izvršio testiranje |
| **Bugovi** | Nema otvorenih kritičnih ili visokoprioritetnih grešaka za stavku koja se isporučuje | Tim prije sprint review-a |
| **Poznati problemi** | Eventualna ograničenja ili poznati problemi su jasno dokumentovani | Tim i Product Owner |

---

## Tehnička ispravnost

| Oblast | Kriterij |
|---|---|
| **Backend** | API endpointi rade u skladu sa očekivanim zahtjevima i vraćaju odgovarajuće statuse i podatke |
| **Validacija** | Validacija podataka je implementirana na backendu i ne oslanja se samo na frontend |
| **Baza podataka** | Podaci o korisnicima, opremi, kabinetima, terminima i rezervacijama se ispravno čuvaju, mijenjaju i čitaju |
| **Autentifikacija** | Prijava, odjava i zaštita ruta funkcionišu prema definisanom modelu pristupa |
| **Autorizacija** | Korisnik može izvršiti samo akcije koje odgovaraju njegovoj ulozi |
| **Kod** | Kod je razumljiv, konzistentan sa postojećom strukturom projekta i komentarisan samo tamo gdje je potrebno |
| **Integracija** | Nova ili izmijenjena funkcionalnost je povezana sa relevantnim dijelovima frontend i backend sistema |
| **Build** | Projekat se builda bez grešaka i bez novih upozorenja koja utiču na rad sistema |
| **Repozitorij** | Promjene su commitovane i pushane na odgovarajuću granu repozitorija |
| **Performanse** | Nisu uočeni očigledni problemi sa brzinom rada, učitavanjem podataka ili nepotrebnim opterećenjem sistema |
| **Pokretanje sistema** | Sistem se može pokrenuti u dogovorenom razvojnom okruženju bez dodatnih neplaniranih koraka |

---

## Frontend i korisničko iskustvo

| Oblast | Kriterij |
|---|---|
| **UI** | Interfejs je usklađen sa funkcionalnim zahtjevima i postojećim stilom aplikacije |
| **Forme** | Validacije na formama rade za obavezna polja, format email adrese, lozinku i druge relevantne unose |
| **Poruke korisniku** | Greške, potvrde i statusi akcija prikazuju se jasno i razumljivo |
| **Navigacija** | Korisnik može jednostavno doći do funkcionalnosti koja odgovara njegovoj ulozi |
| **Upotrebljivost** | Osnovni tokovi korištenja su logični, pregledni i mogu se demonstrirati bez dodatnih objašnjenja |

---

## Dokumentacija i projektni artefakti

| Oblast | Kriterij |
|---|---|
| **Artefakti** | Relevantni projektni artefakti su ažurirani ako je stavka uticala na zahtjeve, arhitekturu, testiranje ili plan isporuke |
| **Backlog** | Product Backlog ili Sprint Backlog odražava trenutno stanje rada |
| **Dokumentacija** | Tehnička i korisnička dokumentacija je dopunjena gdje je to potrebno |
| **Decision Log** | Važne tehničke, arhitekturalne ili dizajnerske odluke su evidentirane |
| **AI Usage Log** | Korištenje AI alata je zabilježeno u AI Usage Log-u kada je primjenjivo |

---

## Spremnost za isporuku

| Oblast | Kriterij |
|---|---|
| **Demo** | Stavka je spremna za demonstraciju na sprint review-u |
| **Stabilnost demo scenarija** | Funkcionalnost se može prikazati kroz dogovoreni demo tok bez dodatnih izmjena neposredno prije prezentacije |
| **Okruženje** | Funkcionalnost je spremna za pokretanje u predviđenom lokalnom ili testnom okruženju |
| **Prihvatanje** | Product Owner ili dogovoreni predstavnik tima potvrđuje da stavka zadovoljava očekivanja |

---
