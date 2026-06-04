# Architecture & Technical Overview: LABsistem

Ovaj dokument pruža sveobuhvatan tehnički i arhitektonski pregled sistema **LABsistem**, softverskog rješenja za upravljanje laboratorijama, kabinetima, opremom i rezervacijama termina. 

- primarna tehnička referenca za razvoj, testiranje, deployment i održavanje sistema

---

## 1. Arhitektonski Obrazac i Pregled Sistema

**LABsistem** je dizajniran kao **Modularni Monolit**. Sistem je podijeljen na jasno definisane slojeve i tehničke cjeline unutar jednog repozitorijuma (*Mono-repo* pristup), što omogućava lakši razvoj, konzistentan deployment i visoke performanse, uz zadržavanje jasnih granica između biznis modula.

### Ključne komponente komunikacije:
* **Klijent -> Server:** Komunikacija se odvija asinhrono preko **REST API** protokola (`/api/*`). Frontend aplikacija šalje HTTP zahtjeve ka backendu.
* **Server -> Baza podataka:** Backend komunicira sa **PostgreSQL** bazom podataka isključivo putem **Entity Framework Core (EF Core)** ORM-a.
* **Server -> Eksterni servisi:** Za slanje email obavijesti i transakcijskih poruka, backend je integrisan sa eksternim provajderom **Resend API**.

---

## 2. Tehnološki Stack (Tech Stack)

Sistem je izgrađen korišćenjem modernih, stabilnih i široko prihvaćenih tehnologija koje osiguravaju skalabilnost i sigurnost.

| Sloj / Komponenta | Tehnologija | Verzija / Specifikacija | Uloga i Razlog Izbora |
| :--- | :--- | :--- | :--- |
| **Backend Framework** | ASP.NET Core (Web SDK) | `.NET 10.0` (Moderni .NET) | Visoke performanse, tipiziran kod, ugrađen DI kontejner i middleware podrška. |
| **Frontend Framework** | React | `v18 + React Router` | Komponentalna arhitektura, brz rendering i glatko SPA (*Single Page Application*) iskustvo. |
| **Frontend Build Tool** | Vite | Najnovija stabilna | Ekstremno brz razvojni server (HMR) i optimizovan produkcijski build. |
| **HTTP Klijent** | Axios | Standardna | Rukovanje asinhronim HTTP zahtjevima, presretanje (*interceptors*) za JWT. |
| **Baza podataka** | PostgreSQL | `v16` (Dockerized) | Pouzdana, open-source relaciono-objektna baza sa odličnim performansama. |
| **ORM** | Entity Framework Core | Kompatibilna sa .NET 10 | *Code-First* pristup, automatsko upravljanje migracijama i mapiranje entiteta. |
| **Sigurnost (Auth)** | JWT & BCrypt.Net-Next | Standardna | Bezbjedno heširanje lozinki i tokenizovana sesija klijenta. |

---

## 3. Struktura Projekta i Organizacija Koda

Repozitorijum je organizovan oko korijenskog direktorijuma `Projekat/` koji sadrži četiri glavne tehničke i logičke cjeline:
Projekat/
├── LabSistem.backend/          # Backend rješenje (.NET Solution)
│   ├── LabSistem.slnx          # Moderni fajl konfiguracije rješenja
│   ├── LABsistem.Presentation/ # API Host, Kontroleri, Program.cs (Entry point)
│   ├── LABsistem.Api/          # Poslovna logika, Servisi, DTOs, Validatori
│   ├── LabSistem.Dal/          # Data Access Layer, DbContext, Migracije
│   └── LabSistem.Domain/       # Core Entiteti, Enumi, Apstrakcije
├── LABsistem.Frontend/         # React SPA klijentska aplikacija
│   ├── src/                    # Izvorni kod (main.jsx, App.jsx, komponente)
│   ├── package.json            # npm skripte i zavisnosti
│   └── nginx.conf              # Konfiguracija za produkcijski web server
├── LABsistem.Tests/            # .NET projekat za unit i integracione testove
└── LABsistem.E2E/              # End-to-End testovi (Playwright)

---
### Entry Points - Tačka ulaza u aplikaciju

- **Backend** (Program.cs) : Inicijalizuje WebApplication.CreateBuilder, učitava .env konfiguracije, registruje servise u DI kontejner, konfiguriše HTTP pipeline (middleware) i pokreće aplikaciju preko app.Run()
- **Frontend** (src/main.jsx) : Pokreće React aplikaciju, montira klijentski ruter i renderuje glavnu <App/> komponentu u DOM.

---

## 4. Slojevita Arhitektura i Zavisnosti Modula

Unutar backend rješenja implementirana je Slojevita (Layered) Arhitektura sa jednosmjernim zavisnostima usmjerenim ka dole (od prezentacije prema domenu):

[Presentation] ──> [Api] ──> [Dal] ──> [Domain]
      │             │         │
      └─────────────┴─────────┘ (Projektne reference)




