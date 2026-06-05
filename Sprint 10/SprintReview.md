### Sprint Review Summary - Sprint 10

#### Planirani sprint goal
Fokus sprinta bio je na zaokruživanju korisničkog profila i pouzdanosti email komunikacije kroz verifikaciju email adrese, podsjetnike prije termina, uređivanje profila i promjenu lozinke, uz isporuku preostalih sekundarnih funkcionalnosti — filtriranja i exporta rezervacija, dark/light moda i vizualizacije zdravlja opreme, te testiranje prethodno preskočenih scenarija.

#### Šta je završeno
Završeni su tok verifikacije email adrese sa statusom na profilu, podsjetnici prije termina (in-app i email), prikaz i uređivanje profila, promjena lozinke iz profila, prikaz nedavnih aktivnosti, sigurnosni email alert za promjene profila, filtriranje i export rezervacija u CSV i PDF, dark/light mode, prsten zdravlja opreme te dodatno testiranje prethodno preskočenih funkcionalnosti.

#### Demonstrirane funkcionalnosti ili artefakti
Demonstrirani su tok verifikacije emaila s ponovnim slanjem linka i prikazom statusa na profilu, slanje podsjetnika 24h i 1h prije termina s razlikovanjem kanala po statusu verifikacije, stranica profila s uređivanjem podataka i historijom aktivnosti, sigurnosni email alert pri promjeni osjetljivih podataka, filtriranje rezervacija po datumu i kabinetu s exportom u CSV (s bosanskim karakterima) i PDF, prebacivanje teme uz trajno pamćenje postavke, te prstenasti grafikon zdravlja opreme na dashboardu.

#### Glavni problemi i blokeri
Najveći izazov bila je ispravna podrška bosanskim karakterima u CSV eksportu, riješena postavljanjem UTF-8 BOM kodiranja. Scheduler za podsjetnike inicijalno nije uzimao u obzir razliku između serverskog i lokalnog vremena, što je riješeno standardizacijom na UTC timestamps. Uočeni su i problemi s kontrastom pojedinih UI komponenti u dark modu, koji su ručno prilagođeni.

#### Ključne odluke donesene u sprintu
Odlučeno je da se email alertovi i podsjetnici šalju isključivo korisnicima s verificiranom email adresom. Export u CSV i PDF uvijek obuhvata samo trenutno filtrirane podatke, a ne cijelu listu. Odbačen je prijedlog za automatski export pri promjeni filtera, kao i automatsko prebacivanje teme prema sistemskim postavkama OS-a.

#### Povratna informacija Product Ownera
Product Owner je istakao da sprint zaokružuje sistem kao cjelinu i da su email verifikacija, sigurnosni alertovi i filtriranje rezervacija praktično korisni dodaci. Prsten zdravlja opreme pohvaljen je kao vrijedan vizualni uvid koji do sad nije bio dostupan na jednom mjestu. Kao otvorene stavke evidentirane su četiri Deferred funkcionalnosti (audit log, To-Do sekcija za tehničara, ocjenjivanje studenata, recenzije opreme) za eventualnu buduću iteraciju.

#### Zaključak za naredni sprint
Sprint 10 je krajnji sprint implementacije projekta. Za posljednji sprint, sprint 11, fokus će biti na finalizaciji čitavog projekta, pregled i uvid u rad kroz čitav razvojni proces.