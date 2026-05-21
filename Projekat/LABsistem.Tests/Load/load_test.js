import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const greske = new Rate('greske');

export const options = {
  stages: [
    { duration: '30s', target: 25 }, // ramp up
    { duration: '2m',  target: 50 }, // NFR-13: 50 concurrent usera
    { duration: '30s', target: 0 },  // ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<1000'], // NFR-13: odziv < 1s
    greske:            ['rate<0.01'],
  },
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:3001';
const HEADERS  = { 'Content-Type': 'application/json' };

// ─── Pomoćna funkcija za login ────────────────────────────────────────────────
function login(username, password) {
  const res = http.post(
    `${BASE_URL}/api/Auth/login`,
    JSON.stringify({ username, password }),
    { headers: HEADERS }
  );
  const token = res.json('token');
  if (!token) console.error(`Login neuspješan za ${username}: ${res.body}`);
  return token;
}

// ─── SETUP: pripremi podatke prije load testa ─────────────────────────────────
export function setup() {

  // 1. Login sa svim ulogama
  const adminToken    = login('admin',    'admin123');
  const tehnicarToken = login('tehnicar', 'tehnicar123');
  const profesorToken = login('profesor', 'profesor123');

  const tehnicarHeaders = { ...HEADERS, Authorization: `Bearer ${tehnicarToken}` };
  const profesorHeaders = { ...HEADERS, Authorization: `Bearer ${profesorToken}` };

  // 2. Admin kreira 50 test studenata (jedan po virtualnom useru)
  const studentTokeni = [];

  for (let i = 1; i <= 50; i++) {
    const username = `loadstudent${i}`;

    const kreiranjeRes = http.post(
      `${BASE_URL}/api/Auth/create-user?uloga=3`,
      JSON.stringify({
        imePrezime: `Load Student`,
        email:      `${username}@labsistem.local`,
        username:   username,
        password:   'student123',
      }),
      { headers: adminHeaders }
    );
  }

  for (let i = 1; i <= 50; i++) {
    const username = `loadstudent${i}`;
    const token = login(username, 'student123');
    
    if (token) {
      studentTokeni.push(token);
    } else {
      console.error(`Nije moguće prijaviti postojećeg studenta: ${username}`);
    }
  }

  console.log(`Uspješno prijavljeno studenata za test: ${studentTokeni.length}`);

  // 3. Tehničar kreira 10 termina za load test
  const terminIDevi = [];

  const danZaTest = new Date();
  danZaTest.setDate(danZaTest.getDate() + 5); 

  const godina = danZaTest.getFullYear();
  const mjesec = String(danZaTest.getMonth() + 1).padStart(2, '0');
  const dan = String(danZaTest.getDate()).padStart(2, '0');

  const datumString = `${godina}-${mjesec}-${dan}T00:00:00.000Z`;
  
  console.log(`Pokušavam kreirati termine za lokalni datum: ${datumString}`);

  for (let i = 0; i < 10; i++) {
    const sat = 8 + i; // 08:00, 09:00...
    
    const terminRes = http.post(
      `${BASE_URL}/api/Termin`,
      JSON.stringify({
        datum: datumString,
        vrijemePocetka: `${String(sat).padStart(2, '0')}:00:00`,
        vrijemeKraja:   `${String(sat + 1).padStart(2, '0')}:00:00`,
        kreatorID: 4, 
        kabinetID: 1,
      }),
      { headers: tehnicarHeaders }
    );

    if (terminRes.status === 200 || terminRes.status === 201) {
      const terminId = terminRes.json('id') || terminRes.json('Id');
      if (terminId) {

        // 4. Profesor otvara termin studentima
        const rezervacijaRes = http.post(
          `${BASE_URL}/api/Rezervacija/rezervisi/${terminId}`,
          JSON.stringify({
            limitOsoba: 50,
            maxKapacitet: 100,
            vidljivoStudentima: true,
          }),
          { headers: profesorHeaders }
        );

        if (rezervacijaRes.status === 200 || rezervacijaRes.status === 201) {
          const rezervacijaId = rezervacijaRes.json('id') || rezervacijaRes.json('Id') || terminId;
          terminIDevi.push({ terminId, rezervacijaId });
          console.log(`Termin ${terminId} otvoren kao rezervacija ${rezervacijaId}`);
        } else {
          console.error(`Otvaranje termina ${terminId} neuspješno: ${rezervacijaRes.status} ${rezervacijaRes.body}`);
        }
      }
    } else {
      console.error(`Kreiranje termina u ${sat}:00h neuspješno: Status ${terminRes.status} | Odgovor: ${terminRes.body}`);
    }
  } return {
    studentTokeni,
    profesorToken,
    terminIDevi,
  };
}
// ─── GLAVNI SCENARIO: simulira 50 concurrent korisnika ───────────────────────
export default function (data) {

  // Svaki virtualni user dobija svoj token po indeksu
  const studentToken = data.studentTokeni[(__VU - 1) % data.studentTokeni.length];

  // ~70% studenata, ~30% profesor koji gleda opremu
  const jeStudent = Math.random() < 0.7;

  if (jeStudent) {
    const headers = { ...HEADERS, Authorization: `Bearer ${studentToken}` };

    // Student pregledava dostupne termine
    const termini = http.get(
      `${BASE_URL}/api/Rezervacija/dostupni-studentima`,
      { headers }
    );
    check(termini, {
      'termini 200': (r) => r.status === 200,
      'odziv < 1s':  (r) => r.timings.duration < 1000,
    });
    greske.add(termini.status !== 200);

    sleep(1);

    // Student se prijavljuje na random dostupni termin
    if (termini.status === 200 && data.terminIDevi.length > 0) {
      const random = data.terminIDevi[Math.floor(Math.random() * data.terminIDevi.length)];

      const prijava = http.post(
        `${BASE_URL}/api/Rezervacija/zahtjev/${random.rezervacijaId}`,
        JSON.stringify({}),
        { headers }
      );
      check(prijava, {
        // 409 = već prijavljen — nije greška sistema, normalno ponašanje
        'prijava prihvaćena': (r) => r.status === 200 || r.status === 201 || r.status === 409,
      });
      // Broji samo stvarne greške servera (5xx)
      greske.add(prijava.status >= 500);
    }

  } else {
    const headers = { ...HEADERS, Authorization: `Bearer ${data.profesorToken}` };

    // Profesor gleda listu opreme
    const oprema = http.get(`${BASE_URL}/api/Oprema`, { headers });
    check(oprema, {
      'oprema 200': (r) => r.status === 200,
      'odziv < 1s': (r) => r.timings.duration < 1000,
    });
    greske.add(oprema.status !== 200);
  }

  sleep(2);
}
