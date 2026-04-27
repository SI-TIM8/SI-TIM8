# LABsistem Frontend

React + Vite frontend skeleton sa Axios klijentom za pozive prema LABsistem API-ju.

## Pokretanje lokalno

1. Kopiraj `.env.example` u `.env`.
2. Po potrebi promijeni `VITE_API_BASE_URL` (podrazumijevano `/api`).
3. Pokreni komande:

```bash
npm install
npm run dev
```

Frontend ce biti dostupan na `http://localhost:5173`.
U development modu Vite proxy preusmjerava `/api` zahtjeve na `http://localhost:8080`.

## Build

```bash
npm run build
npm run preview
```

## Napomena

Test konekcije u pocetnom ekranu poziva `GET /openapi/v1.json` na backend-u.

## Docker

Frontend image se builda iz `Dockerfile` datoteke i servira preko Nginx-a na portu `3000`.
Nginx prosljedjuje `/api/*` zahtjeve prema `labsistem.api:8080` kroz zajednicku Docker mrezu definisanu u backend `docker-compose` fajlovima.
