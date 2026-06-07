# Deployment procedura

## Naziv aplikacije i opis arhitekture

Naziv aplikacije: **LABsistem**

LABsistem je web aplikacija za upravljanje laboratorijskom opremom, kabinetima, terminima, rezervacijama, zahtjevima, korisnicima i obavijestima. Sistem je organizovan kao višeslojna aplikacija:

- **Frontend**: React/Vite single-page aplikacija.
- **Backend API**: ASP.NET Core Web API.
- **Application/API sloj**: servisi, validatori, DTO modeli i poslovna logika.
- **Domain sloj**: domenski entiteti i enumeracije.
- **DAL sloj**: Entity Framework Core `DbContext`, repozitoriji, konfiguracije, migracije i seed podaci.
- **Baza podataka**: PostgreSQL 16.

U Docker deploymentu frontend se servira preko Nginx kontejnera. Nginx prosljeđuje zahtjeve koji počinju sa `/api/` prema backend kontejneru `labsistem.api:8080`. Backend komunicira sa PostgreSQL bazom kroz internu Docker mrežu.

## Tehnologije koje se koriste

- ASP.NET Core / .NET 10 za backend API.
- Entity Framework Core 10 za ORM, migracije i seed podataka.
- PostgreSQL 16 za relacijsku bazu podataka.
- React 18 za frontend.
- Vite 5 za frontend razvojni server i build.
- Axios za HTTP komunikaciju frontenda sa backend API-jem.
- Docker i Docker Compose za lokalno i produkcijsko pokretanje servisa.
- Nginx za serviranje produkcijskog frontend builda i proxy prema backendu.
- GitHub Actions za CI/CD pipeline.
- DigitalOcean Droplet kao produkcijsko okruženje.
- Resend za slanje email notifikacija.
- xUnit za backend unit/integration testove.
- Cypress i Playwright za frontend/E2E testove.

## Potrebni alati i verzije

Za lokalno pokretanje potrebno je instalirati:

- .NET SDK 10.x
- Node.js 20.x ili noviji kompatibilan sa Vite 5
- npm
- Docker Desktop ili Docker Engine
- Docker Compose
- Git
- PostgreSQL klijent po potrebi, npr. DBeaver ili `psql`

Za produkcijski deployment potrebno je:

- GitHub repozitorij sa podešenim GitHub Actions secrets.
- DigitalOcean Droplet sa instaliranim Dockerom i Docker Compose pluginom.
- SSH pristup Dropletu.
- Podešene environment varijable na serveru.
- Podešen Resend API key i verified sender email.

## Environment varijable

Backend, baza i deployment koriste sljedeće environment varijable:

| Varijabla                | Namjena                                                                                                                       |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------------------- |
| `POSTGRES_DB`            | Naziv PostgreSQL baze.                                                                                                        |
| `POSTGRES_USER`          | PostgreSQL korisnik.                                                                                                          |
| `POSTGRES_PASSWORD`      | Lozinka PostgreSQL korisnika.                                                                                                 |
| `CONNECTION_STRING`      | Connection string koji backend koristi za spajanje na PostgreSQL. U Docker mreži host treba biti naziv DB servisa, npr. `db`. |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core okruženje, npr. `Production` ili `Development`.                                                                  |
| `ASPNETCORE_URLS`        | URL/port na kojem backend sluša unutar kontejnera, npr. `http://+:8080`.                                                      |
| `FRONTEND_BASE_URL`      | Javna adresa frontenda, koristi se za email linkove za verifikaciju i reset lozinke.                                          |
| `RESEND_API_KEY`         | API key za Resend email servis.                                                                                               |
| `FROM_EMAIL`             | Email adresa pošiljaoca za Resend.                                                                                            |
| `VITE_API_BASE_URL`      | Frontend API base URL za lokalni/dev build. Ako nije postavljen, frontend koristi `/api`.                                     |

GitHub Actions deployment koristi sljedeće secrets:

| Secret           | Namjena                                                                       |
| ---------------- | ----------------------------------------------------------------------------- |
| `SERVER_HOST`    | IP adresa ili DNS naziv DigitalOcean Dropleta.                                |
| `SERVER_USER`    | SSH korisnik na Dropletu.                                                     |
| `SERVER_SSH_KEY` | Privatni SSH ključ za pristup Dropletu.                                       |
| `APP_DIR`        | Direktorij na Dropletu u koji se kopiraju Docker image arhive i compose fajl. |

## Lokalno pokretanje baze

Baza se može pokrenuti kroz Docker Compose iz backend direktorija:

```bash
cd Projekat/LabSistem.backend
docker compose up -d db
```

Ova komanda pokreće PostgreSQL 16 kontejner `labsistem-db`, kreira Docker volume `labsistem_pgdata` i izlaže bazu na portu `5432`.

Prije pokretanja potrebno je imati `.env` fajl ili postavljene environment varijable:

```env
POSTGRES_DB=labsistem
POSTGRES_USER=labsistem
POSTGRES_PASSWORD=labsistem
CONNECTION_STRING=Host=db;Port=5432;Database=labsistem;Username=labsistem;Password=labsistem
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
FRONTEND_BASE_URL=http://localhost:3001
RESEND_API_KEY=<resend-api-key>
FROM_EMAIL=<verified-sender-email>
```

Za lokalno pokretanje backend aplikacije van Docker kontejnera connection string iz `appsettings.json` koristi `Host=localhost;Port=5432`.

## Lokalno pokretanje backend-a

Backend se može pokrenuti direktno preko .NET CLI-ja:

```bash
cd Projekat/LabSistem.backend
dotnet restore LabSistem.slnx
dotnet run --project LABsistem.Presentation/LABsistem.Presentation.csproj
```

Prema `launchSettings.json`, backend u development modu sluša na:

- `http://localhost:5222`
- `https://localhost:7116`

Ako se backend pokreće u Docker Compose okruženju, koristi se port `8080`.

Kompletan lokalni Docker stack, uključujući bazu, backend i frontend, pokreće se komandom:

```bash
cd Projekat/LabSistem.backend
docker compose up --build
```

U tom režimu dostupni su:

- Backend API: `http://localhost:8080`
- Frontend preko Nginx-a: `http://localhost:3001`
- PostgreSQL: `localhost:5432`

## Lokalno pokretanje frontend-a

Frontend se pokreće iz frontend direktorija:

```bash
cd Projekat/LABsistem.Frontend
npm install
npm run dev
```

Frontend development server je dostupan na:

```text
http://localhost:5173
```

Frontend koristi `VITE_API_BASE_URL` ako je postavljen. Ako nije postavljen, koristi `/api`. U Docker/Nginx deploymentu `/api` zahtjevi se proxy prosljeđuju prema backend servisu.

Produkcijski frontend build se kreira komandom:

```bash
cd Projekat/LABsistem.Frontend
npm run build
```

Lokalna provjera builda:

```bash
npm run preview
```

## Migracije i seed podaci

Migracije se nalaze u:

```text
Projekat/LabSistem.backend/LabSistem.Dal/Migrations
```

Backend automatski primjenjuje migracije pri startupu aplikacije pozivom `context.Database.Migrate()`, osim u `Testing` okruženju. Nakon migracija automatski se izvršava seed:

- default korisnici,
- default objekti,
- default kabineti.

Seed korisnici:

| Uloga    | Username   | Lozinka       |
| -------- | ---------- | ------------- |
| Admin    | `admin`    | `admin123`    |
| Profesor | `profesor` | `profesor123` |
| Student  | `student`  | `student123`  |
| Tehničar | `tehnicar` | `tehnicar123` |

Uspješan završetak migracija na backend startupu se vidi u logu:

```text
Migracije su uspjesno provjerene/primijenjene.
```

Ako migracija ne uspije, backend log ispisuje:

```text
Greska pri migraciji:
```

## Pokretanje testova

Backend testovi se pokreću komandom:

```bash
dotnet test Projekat/LABsistem.Tests/LABsistem.Tests.csproj --logger "console;verbosity=normal"
```

CI workflow za pull requestove nalazi se u:

```text
.github/workflows/build-script.yml
```

Taj workflow radi:

1. checkout repozitorija,
2. setup .NET SDK 10.x,
3. `dotnet restore`,
4. `dotnet build`,
5. `dotnet test`.

Frontend Cypress testovi se nalaze u:

```text
Projekat/LABsistem.Frontend/cypress/e2e
```

Pokretanje Cypress testova:

```bash
cd Projekat/LABsistem.Frontend
npm install
npx cypress run
```

Playwright E2E testovi se nalaze u:

```text
Projekat/LABsistem.E2E/tests
```

Pokretanje Playwright testova:

```bash
cd Projekat/LABsistem.E2E
npm install
npx playwright install
npx playwright test
```

Playwright po defaultu koristi:

```text
http://localhost:3001
```

Drugi URL se može zadati varijablom `PLAYWRIGHT_BASE_URL`.

## Produkcijski deployment

Produkcijski deployment se izvršava preko GitHub Actions workflowa:

```text
.github/workflows/deployment-script.yml
```

Deployment se automatski pokreće na svaki push na branch:

```text
main
```

Pipeline deploya kompletan sistem:

- backend Docker image `labsistem-api:latest`,
- frontend Docker image `labsistem-frontend:latest`,
- PostgreSQL bazu kroz Docker Compose,
- migracije i seed podatke kroz startup backend aplikacije,
- frontend Nginx servis koji proxy prosljeđuje `/api` zahtjeve backendu.

Deployment tok:

1. GitHub Actions radi checkout koda.
2. Builda backend Docker image iz `Projekat/LabSistem.backend/LABsistem.Presentation/Dockerfile`.
3. Builda frontend Docker image iz `Projekat/LABsistem.Frontend/Dockerfile`.
4. Snima image-e u `labsistem-api.tar` i `labsistem-frontend.tar`.
5. Kopira image-e i `docker-compose.production.yml` na DigitalOcean Droplet preko SSH/SCP.
6. Na Dropletu učitava Docker image-e pomoću `docker load`.
7. Kopira produkcijski compose fajl u `docker-compose.prod.yml`.
8. Pokreće ili ponovo koristi PostgreSQL kontejner `labsistem-db`.
9. Čeka da baza bude `healthy`.
10. Rekreira backend kontejner `labsistem-api`.
11. Čeka backend log koji potvrđuje uspješne migracije.
12. Rekreira frontend kontejner `labsistem-frontend`.
13. Čisti nekorištene Docker image-e pomoću `docker image prune -f`.

Produkcijski Docker Compose fajl nalazi se u:

```text
Projekat/LabSistem.backend/docker-compose.production.yml
```

Produkcijski servisi:

| Servis      | Kontejner            | Port                   |
| ----------- | -------------------- | ---------------------- |
| PostgreSQL  | `labsistem-db`       | interno u Docker mreži |
| Backend API | `labsistem-api`      | `8080:8080`            |
| Frontend    | `labsistem-frontend` | `3000:80`              |

Frontend i backend su povezani preko Nginx konfiguracije:

```text
Projekat/LABsistem.Frontend/nginx.conf
```

Nginx pravilo:

```text
/api/ -> http://labsistem.api:8080
```

Zbog toga frontend u produkciji može koristiti relativni API path `/api`, a backend nije potrebno direktno izlagati korisniku ako se pristup organizuje kroz frontend/Nginx.

## Provjera deploymenta

Nakon deploymenta provjerava se da su kontejneri podignuti na DigitalOcean Dropletu:

```bash
docker ps
```

Očekivani kontejneri:

```text
labsistem-db
labsistem-api
labsistem-frontend
```

Provjera health statusa baze:

```bash
docker inspect -f '{{if .State.Health}}{{.State.Health.Status}}{{else}}no-healthcheck{{end}}' labsistem-db
```

Očekivani rezultat:

```text
healthy
```

Provjera backend migracija:

```bash
docker logs labsistem-api
```

U logu treba postojati:

```text
Migracije su uspjesno provjerene/primijenjene.
```

Provjera frontenda:

```bash
curl http://localhost:3000
```

Ako je port `3000` javno otvoren ili mapiran kroz reverse proxy, aplikacija se provjerava i kroz browser preko javne adrese DigitalOcean Dropleta.

## Link na deployment

Produkcijsko okruženje je DigitalOcean Droplet.

Link deploymenta:

```text
https://labsistem.serveblog.net/login
```

Ako je ispred aplikacije podešen reverse proxy, provjera se radi preko domene koju tim koristi za taj Droplet.

## Poznata ograničenja deploymenta

- Deployment zavisi od dostupnosti DigitalOcean Dropleta i SSH pristupa.
- GitHub Actions workflow očekuje da su `SERVER_HOST`, `SERVER_USER`, `SERVER_SSH_KEY` i `APP_DIR` secrets ispravno postavljeni.
- Environment varijable za bazu, backend, frontend URL i Resend moraju biti postavljene na serveru prije pokretanja produkcijskog compose fajla.
- Produkcijski workflow provjerava da je baza healthy i da backend log potvrdi migracije, ali ne radi zaseban HTTP health endpoint poziv iz GitHub Actions workflowa.
- `docker-compose.production.yml` koristi postojeći eksterni Docker volume `labsistembackend_labsistem_pgdata` i eksternu mrežu `labsistem-network`, pa oni moraju postojati ili biti kreirani prije prvog produkcijskog pokretanja.
- Email funkcionalnosti zavise od validnog `RESEND_API_KEY` i `FROM_EMAIL`.
- Ako se promijeni javna adresa frontenda, mora se ažurirati `FRONTEND_BASE_URL`, jer se koristi u email linkovima.

## Najčešći problemi i rješenja

### Baza nije healthy

Provjera:

```bash
docker logs labsistem-db
docker inspect -f '{{if .State.Health}}{{.State.Health.Status}}{{else}}no-healthcheck{{end}}' labsistem-db
```

Mogući uzroci:

- pogrešan `POSTGRES_USER`,
- pogrešan `POSTGRES_PASSWORD`,
- pogrešan `POSTGRES_DB`,
- postojeći volume sadrži staru bazu sa drugim kredencijalima.

Rješenje je uskladiti environment varijable sa postojećim volumeom ili kreirati novi volume ako se radi o čistom deploymentu.

### Backend ne može pristupiti bazi

Provjera:

```bash
docker logs labsistem-api
```

Mogući uzroci:

- `CONNECTION_STRING` ne koristi Docker service name `db`,
- baza nije healthy,
- backend i baza nisu na istoj Docker mreži.

U Docker deploymentu connection string treba koristiti host `db`, a ne `localhost`.

### Migracije nisu uspjele

Provjera:

```bash
docker logs labsistem-api
```

Ako log sadrži `Greska pri migraciji:`, potrebno je provjeriti connection string, stanje baze i zadnju EF migraciju.

### Frontend se otvara, ali API pozivi ne rade

Provjera:

```bash
docker logs labsistem-frontend
docker logs labsistem-api
```

Mogući uzroci:

- backend kontejner nije pokrenut,
- Nginx proxy ne može dohvatiti `labsistem.api:8080`,
- frontend i backend nisu na istoj Docker mreži,
- CORS nije usklađen za način pristupa aplikaciji.

U Docker produkciji frontend treba slati zahtjeve na `/api`, a Nginx ih prosljeđuje prema backendu.

### Emailovi se ne šalju

Provjera:

```bash
docker logs labsistem-api
```

Mogući uzroci:

- `RESEND_API_KEY` nije ispravan,
- `FROM_EMAIL` nije verifikovan u Resend-u,
- `FRONTEND_BASE_URL` nije ispravan.

### GitHub Actions deployment ne može pristupiti serveru

Provjeriti secrets:

- `SERVER_HOST`
- `SERVER_USER`
- `SERVER_SSH_KEY`
- `APP_DIR`

Također provjeriti da li SSH ključ ima pristup DigitalOcean Dropletu i da li korisnik ima dozvolu za Docker komande.

### Kontejneri se ne rekreiraju nakon novog deploymenta

Provjera:

```bash
docker ps -a
docker images
```

Workflow uklanja postojeće `labsistem-api` i `labsistem-frontend` kontejnere i zatim ih pokreće sa novim image-ima. Ako se promjene ne vide, provjeriti da li je GitHub Actions run uspješno završio i da li su novi image-i učitani preko `docker load`.
