import Layout from "../components/Layout";

const DASHBOARD_PO_ULOZI = {
  student: {
    naslov: "Dobrodošli u LABsistem",
    opis: "Pregledajte dostupne termine i upravljajte rezervacijama.",
    statistike: [
      { label: "Aktivne rezervacije", vrijednost: "2", klasa: "plavo" },
      { label: "Termini ovaj mjesec", vrijednost: "3", klasa: "" },
      { label: "Preostalo rezervacija", vrijednost: "0", klasa: "amber" },
    ],
    tabela: {
      naslov: "Moje rezervacije",
      kolone: ["Laboratorij", "Datum", "Vrijeme", "Status"],
      redovi: [
        ["Kabinet 101", "28.04.2026.", "10:00 – 12:00", "odobreno"],
        ["Kabinet 203", "02.05.2026.", "14:00 – 16:00", "na čekanju"],
      ],
    },
  },
  profesor: {
    naslov: "Pregled sistema",
    opis: "Upravljajte zahtjevima i pratite napredak studenata.",
    statistike: [
      { label: "Zahtjevi na čekanju", vrijednost: "5", klasa: "amber" },
      { label: "Aktivne rezervacije", vrijednost: "12", klasa: "plavo" },
      { label: "Studenti aktivni", vrijednost: "24", klasa: "" },
    ],
    tabela: {
      naslov: "Zahtjevi studenata",
      kolone: ["Student", "Laboratorij", "Datum", "Status"],
      redovi: [
        ["Ajla Kovač", "Kabinet 101", "29.04.2026.", "na čekanju"],
        ["Mirza Ilić", "Kabinet 203", "30.04.2026.", "na čekanju"],
        ["Sara Begić", "Kabinet 101", "01.05.2026.", "odobreno"],
      ],
    },
  },
  tehnicar: {
    naslov: "Upravljanje laboratorijem",
    opis: "Pregledajte termine, opremu i prijavljene kvarove.",
    statistike: [
      { label: "Termini danas", vrijednost: "4", klasa: "plavo" },
      { label: "Ukupna oprema", vrijednost: "38", klasa: "" },
      { label: "Aktivni kvarovi", vrijednost: "2", klasa: "crveno" },
    ],
    tabela: {
      naslov: "Prijavljeni kvarovi",
      kolone: ["Oprema", "Serijski broj", "Prijavio", "Status"],
      redovi: [
        ["Mikroskop XR-200", "SN-4421", "Ajla Kovač", "prijavljen"],
        ["Centrifuga C-10", "SN-8834", "Mirza Ilić", "u popravci"],
      ],
    },
  },
  admin: {
    naslov: "Administratorski panel",
    opis: "Upravljajte korisnicima, objektima i kabinetima.",
    statistike: [
      { label: "Ukupno korisnika", vrijednost: "47", klasa: "plavo" },
      { label: "Aktivni objekti", vrijednost: "3", klasa: "" },
      { label: "Kabineti", vrijednost: "12", klasa: "" },
    ],
    tabela: {
      naslov: "Korisnici",
      kolone: ["Ime i prezime", "Email", "Uloga", "Status"],
      redovi: [
        ["Ajla Kovač", "ajla.kovac@uni.ba", "Student", "aktivan"],
        ["Dr. Samir Subašić", "s.subasic@uni.ba", "Profesor", "aktivan"],
        ["Zlatan Mujić", "z.mujic@uni.ba", "Tehničar", "aktivan"],
      ],
    },
  },
};

const BADGE_KLASA = {
  "odobreno": "zeleno",
  "na čekanju": "zuto",
  "prijavljen": "crveno",
  "u popravci": "amber",
  "aktivan": "zeleno",
  "odbijeno": "crveno",
};

function Dashboard() {
  const uloga = localStorage.getItem("uloga") || "student";
  const podaci = DASHBOARD_PO_ULOZI[uloga];

  return (
    <Layout>
      <div className="page-header">
        <h1>{podaci.naslov}</h1>
        <p>{podaci.opis}</p>
      </div>

      {/* Statistike */}
      <div className="cards-grid">
        {podaci.statistike.map((stat, i) => (
          <div key={i} className={`stat-card ${stat.klasa}`}>
            <div className="stat-value">{stat.vrijednost}</div>
            <div className="stat-label">{stat.label}</div>
          </div>
        ))}
      </div>

      {/* Tabela */}
      <div className="table-wrapper">
        <div className="table-header">
          <h2>{podaci.tabela.naslov}</h2>
        </div>
        <table>
          <thead>
            <tr>
              {podaci.tabela.kolone.map((k, i) => (
                <th key={i}>{k}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {podaci.tabela.redovi.map((red, i) => (
              <tr key={i}>
                {red.map((celija, j) => (
                  <td key={j}>
                    {BADGE_KLASA[celija] ? (
                      <span className={`badge ${BADGE_KLASA[celija]}`}>
                        {celija}
                      </span>
                    ) : celija}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </Layout>
  );
}

export default Dashboard;