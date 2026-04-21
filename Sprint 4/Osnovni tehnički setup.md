## **Tehnološki Setup**

#### **Frontend**

| Biblioteka / Alat | Verzija | Namjena i razlog izbora |
| :---- | :---- | :---- |
| **React** | 18.x | Omogućava izgradnju brzog i reaktivnog korisničkog interfejsa kroz komponentni model. |
| **Vite** | 5.x | Moderan alat za build i razvoj koji drastično ubrzava proces rada. Nudi "Hot Module Replacement" (HMR) koji trenutno reflektuje izmjene u kodu bez osvježavanja cijele stranice. |
| **Axios** | 1.x | Robustan HTTP klijent za komunikaciju s backendom. Izabran zbog ugrađene podrške za presretanje zahtjeva (interceptors), automatsku transformaciju JSON podataka i bolju obradu grešaka u odnosu na standardni Fetch API. |
| **Environment Vars** | \- | Korištenje .env datoteka i VITE\_API\_BASE\_URL varijable omogućava laku promjenu endpointa bez zadiranja u izvorni kod, što je ključno za prelazak s lokalnog na produkcijsko okruženje. |

---

#### **Backend**

| Biblioteka / Alat | Verzija | Namjena i razlog izbora |
| :---- | :---- | :---- |
| **ASP.NET Core** | **10.0** | Najnovija verzija visokoperformansnog, cross-platform frameworka. Izabrana zbog izuzetne brzine, sigurnosti i nativne podrške za razvoj skalabilnih Web API-ja. |
| **Clean Architecture** | \- | Razdvajanje sistema na Presentation, API, DAL i Domain projekte. Ovakav pristup osigurava da poslovna logika ne ovisi o vanjskim alatima ili bazi podataka, čineći sistem lakšim za testiranje i održavanje. |
| **Entity Framework Core** | 10.x | Snažan ORM (Object-Relational Mapper) koji omogućava rad s bazom podataka koristeći C\# objekte. Olakšava rad s migracijama i smanjuje potrebu za pisanjem manuelnih SQL upita. |
| **OpenAPI (Swagger)** | 1.x | Integrisana Microsoft podrška za automatsko generisanje interaktivne API dokumentacije. Omogućava frontend timu testiranje endpointa u realnom vremenu bez potrebe za čitanjem backend koda. |
| **Npgsql** | \- | Open-source ADO.NET provider za PostgreSQL. Omogućava EF Core-u da efikasno komunicira s PostgreSQL bazom koristeći sve njene napredne mogućnosti. |

---

#### **Baza podataka**

**PostgreSQL 16**

Relaciona baza podataka poznata po svojoj stabilnosti i usklađenosti s ACID standardima. PostgreSQL je izabran jer savršeno podržava kompleksne relacije unutar sistema. U razvojnom okruženju se pokreće unutar Docker kontejnera, što osigurava da svaki član tima radi na identičnoj verziji baze bez potrebe za lokalnom instalacijom softvera.

**LabSistemDbContext**

Centralno mjesto za konfiguraciju baze podataka unutar aplikacije. Putem ovog konteksta definišu se mapiranja entiteta, relacije (jedan-prema-više, više-prema-više) i pravila integriteta podataka, čime se osigurava konzistentnost na nivou koda.

---

#### **1.4 Infrastruktura i Kontejnerizacija**

Sistem koristi **Docker** za standardizaciju razvojnog i produkcijskog okruženja.

* **Docker Compose:** Omogućava podizanje kompletnog ekosistema (API server i PostgreSQL baza) jednom komandom. Definiše mrežnu izolaciju i volumene za trajno čuvanje podataka.  
* **Healthchecks:** Uvedeni unutar docker-compose konfiguracije kako bi se osiguralo da API servis ne pokuša uspostaviti vezu prije nego što je PostgreSQL baza potpuno spremna za rad.  
* **Networking:** Interna Docker mreža omogućava sigurnu komunikaciju između API-ja i baze, dok su prema vanjskom svijetu otvoreni samo portovi **8080** (API) i **5432** (Baza za administraciju).

---

#### **Development alati**

| Alat | Namjena |
| :---- | :---- |
| **DBeaver** | Univerzalni alat za upravljanje bazom podataka. Koristi se za vizualni pregled podataka, izvršavanje SQL skripti i analizu strukture tabela u Docker kontejneru. |
| **Docker Desktop** | Centralni dashboard za upravljanje kontejnerima, pregled logova servera u realnom vremenu i nadzor resursa koje troše API i baza. |
| **Node.js** | Neophodan za pokretanje React razvojnog servera i upravljanje frontend zavisnostima putem npm-a. |
| **Visual Studio / VS Code** | Primarna razvojna okruženja s ekstenzijama za C\# i React razvoj, omogućavaju integrisani debugging i rad s Dockerom. |

---

### **Sažetak Deployment Sheme**

| Komponenta | Tehnologija | Deployment Tip |
| :---- | :---- | :---- |
| **Frontend UI** | React (Vite) | Lokalni dev server / Spremno za Static Hosting (npr. Vercel) |
| **API Server** | ASP.NET Core (.NET 10\) | Kontejnerizovan (Docker) |
| **Baza podataka** | PostgreSQL 16 | Kontejnerizovan (Docker) |
| **API Gateway** | REST / OpenAPI | Dokumentovano putem Swagger-a |

