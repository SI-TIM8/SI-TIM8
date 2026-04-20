# LABsistem Frontend

React + Vite frontend skeleton sa Axios klijentom za pozive prema LABsistem API-ju.

## Pokretanje lokalno

1. Kopiraj `.env.example` u `.env`.
2. Po potrebi promijeni `VITE_API_BASE_URL` (podrazumijevano `http://localhost:8080`).
3. Pokreni komande:

```bash
npm install
npm run dev
```

Frontend ce biti dostupan na `http://localhost:5173`.

## Build

```bash
npm run build
npm run preview
```

## Napomena

Test konekcije u pocetnom ekranu poziva `GET /openapi/v1.json` na backend-u.
