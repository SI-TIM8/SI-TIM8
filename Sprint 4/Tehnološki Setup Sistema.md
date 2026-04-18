**Tehnički Setup Sistema** 

## **1\. Osnovni Tehnološki Stack**

| Kategorija | Tehnologija | Opis / Uloga |
| :---- | :---- | :---- |
| **Backend Framework** | **ASP.NET Core 8.0** | Primarni framework za razvoj REST API-ja. |
| **Frontend Library** | **React.js** | Klijentski interfejs zasnovan na komponentama. |
| **Baza podataka** | **PostgreSQL (Docker)** | Relaciona baza unutar izolovanog kontejnera. |
| **Kontejnerizacija** | **Docker & Compose** | Standardizacija razvojnog i produkcionog okruženja. |
| **Database Tool** | **DBeaver** | Univerzalni alat za timski pristup i upravljanje bazom. |

---

## **2\. Docker Konfiguracija i PostgreSQL**

Umjesto klasične instalacije, baza "živi" unutar Docker kontejnera. To omogućava da cijeli tim ima identičnu verziju baze bez konflikata.

### **Docker Compose Setup:**

* Image: postgres:latest.  
* Portovi: Mapiranje 5432:5432 (omogućava vanjski pristup za DBeaver).  
* Volumes: Podaci se čuvaju na disku domaćina (npr. db-data:/var/lib/postgresql/data) kako bi se sačuvali i nakon gašenja kontejnera.  
* Environment: Sigurnosne varijable za POSTGRES\_USER i POSTGRES\_PASSWORD.

---

## **3\. Zajednički pristup bazi putem DBeaver-a**

Jedna od ključnih prednosti ovog setupa je što svaki član tima može pratiti stanje podataka u realnom vremenu.

**Kako se povezati:**

1. Host: IP adresa servera (ili localhost ako radite lokalno).  
2. Port: 5432\.  
3. Database: LabManagerDB (ili ime koje definišete).  
4. Credentials: Koristite zajednički Username i Password definisan u Docker environment fajlu.  
5. Prednost: DBeaver vam omogućava vizuelni prikaz tabela, ER dijagrame i direktno izvršavanje SQL upita bez pisanja koda u C\#-u.

---

## **4\. Deployment i Hosting (Free Tier)**

Pošto koristimo Docker, aplikacija je spremna za moderne Cloud platforme koje nude besplatne resurse za razvojne projekte.

* Backend & Baza: Sandbox. Ovaj service podržava Docker i automatski će podići PostgreSQL kontejner zajedno sa našim API-jem.  
* Frontend: Vercel. Najbrži način za serviranje React aplikacije; automatski se ažurira svaki put kada se uradi git push.  
* Networking: API će biti dostupan putem javnog URL-a (npr. lab-api.render.com), koji će se unijeti u DBeaver kao Host za daljinski pristup.

---

## **5\. Tehnološki moduli i sigurnost**

* ORM: EF Core sa Npgsql driverom (omogućava pisanje C\# koda koji se automatski pretvara u PostgreSQL upite).  
* Autentifikacija: JWT (JSON Web Token) – klijent dobije token pri loginu i šalje ga nazad za svaku zaštićenu akciju.  
* Heširanje lozinki: BCrypt.Net u Aplikacijskom sloju (Lozinke u bazi su nečitljive čak i administratorima koji gledaju preko DBeaver-a).  
* Komunikacija: Axios (React) komunicira sa Kestrel serverom (.NET) putem asinhronih poziva.

---

## **6\. Sažetak Deployment Sheme**

| Komponenta | Tehnologija | Okruženje |
| :---- | :---- | :---- |
| **API Server** | .NET 8 (Dockerized) | Cloud (Sandbox) |
| **Baza podataka** | PostgreSQL (Docker) | Cloud (Pristup putem DBeaver-a) |
| **Frontend UI** | React.js (Build) | Vercel  |
| **Upravljanje** | SQL / GUI | DBeaver Community Edition |

