**Osnovni tehnički setup**

## **1\. Osnovni tehnološki stack**

| Kategorija | Tehnologija | Opis / Uloga |
| :---- | :---- | :---- |
| **Backend Framework** | **ASP.NET Core (.NET 10)** | Web API host je projekat LABsistem.Presentation (TargetFramework: net10.0). |
| **Arhitektura slojeva** | **Presentation / Api / Dal / Domain** | Skelet je podijeljen na više projekata unutar jednog solution-a. |
| **Baza podataka** | **PostgreSQL 16 (Docker)** | Relaciona baza pokrenuta kao `db` servis u docker-compose konfiguraciji. |
| **Kontejnerizacija** | **Docker & Docker Compose** | Lokalno pokretanje API-ja i baze kroz dvije usluge (`api` i `db`). |
| **Database Tool** | **DBeaver (opcionalno)** | Vanjski alat za pregled i izvršavanje SQL upita nad PostgreSQL bazom. |
| **Frontend Library** | **React.js (Vite)** | Frontend skeleton je dodat u LABsistem.Frontend projektu. |

---

## **2\. Docker konfiguracija i PostgreSQL**

PostgreSQL i API su definisani u docker-compose konfiguraciji tako da se mogu pokrenuti zajedno u lokalnom razvojnom okruženju.

### **Docker Compose setup (stvarno stanje):**

* **db image:** `postgres:16`  
* **db portovi:** `5432:5432`  
* **db volume:** `labsistem_pgdata:/var/lib/postgresql/data`  
* **db environment:** `POSTGRES_DB=labsistem`, `POSTGRES_USER=labsistem`, `POSTGRES_PASSWORD=labsistem`  
* **healthcheck:** `pg_isready -U labsistem -d labsistem`  
* **api build:** Dockerfile iz `LABsistem.Presentation/Dockerfile`  
* **api portovi:** `8080:8080`  
* **api connection string (u kontejneru):** host `db`, baza `labsistem`

---

## **3\. Pristup bazi putem DBeaver-a**

Za lokalni rad (kada je docker-compose podignut na istoj mašini):

1. Host: `localhost`  
2. Port: `5432`  
3. Database: `labsistem`  
4. Username: `labsistem`  
5. Password: `labsistem`

Ako se povezivanje radi sa druge mašine, Host je IP adresa mašine na kojoj je pokrenut Docker.

---

## **4\. Deployment i hosting**

Trenutno podrzan i vidljiv setup je:

* Lokalni deployment backend-a i baze putem Docker Compose-a (`db` + `api`).
* API je dostupan na portu `8080` nakon pokretanja kontejnera.
* Baza je dostupna na portu `5432` za alate poput DBeaver-a.
* Frontend (React/Vite) se pokrece lokalno kroz Node.js (`npm run dev`) na portu `5173`.

Za cloud varijantu frontend je spreman za staticki deployment (npr. Vercel), dok backend i baza ostaju odvojeni servisi.

---

## **5\. Tehnološki moduli i sigurnost (trenutno implementirano)**

* U API host projektu je uključena OpenAPI podrška (`Microsoft.AspNetCore.OpenApi`).  
* Definisan je osnovni ASP.NET Core pipeline (`AddControllers`, `AddOpenApi`).  
* Connection string `Default` postoji i koristi PostgreSQL parametre za lokalno okruženje.  
* U DAL sloju postoji skelet klase `LabSistemDbContext`, ali bez pune ORM konfiguracije.  
* Frontend skeleton koristi Axios klijent i `VITE_API_BASE_URL` varijablu za komunikaciju sa API-jem.

**Napomena o razlikama u odnosu na plan:**

* JWT autentifikacija trenutno nije vidljiva u konfiguraciji/pipeline-u.  
* BCrypt i EF Core + Npgsql paketi trenutno nisu vidljivi u .csproj zavisnostima skeleta.

---

## **6\. Sažetak trenutne deployment sheme**

| Komponenta | Tehnologija | Okruženje |
| :---- | :---- | :---- |
| **API Server** | ASP.NET Core (.NET 10), Dockerized | Lokalno (Docker Compose, servis `api`) |
| **Baza podataka** | PostgreSQL 16 (Docker) | Lokalno (Docker Compose, servis `db`) |
| **Frontend UI** | React.js (Vite) + Axios | Lokalno (Node.js), spremno za static hosting |
| **Upravljanje bazom** | SQL / GUI (DBeaver) | Lokalno ili daljinski pristup Docker hostu |

