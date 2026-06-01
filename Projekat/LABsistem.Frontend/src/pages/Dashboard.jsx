import Layout from "../components/Layout";
import { useEffect, useMemo, useState } from "react";
import api from "../api/client";
import { getCurrentRole } from "../auth/routeAccess";

const DASHBOARD_TEXT_BY_ROLE = {
  student: {
    naslov: "Dobrodosli u LABsistem",
    opis: "Pregledajte dostupne termine i upravljajte svojim rezervacijama.",
    tabelaNaslov: "Moje rezervacije",
    tabelaKolone: ["Laboratorij", "Datum", "Vrijeme", "Status"],
  },
  profesor: {
    naslov: "Pregled sistema",
    opis: "Pratite rezervisane termine i obradite studentske zahtjeve.",
    tabelaNaslov: "Zahtjevi studenata",
    tabelaKolone: ["Student", "Laboratorij", "Datum", "Status"],
  },
  tehnicar: {
    naslov: "Upravljanje laboratorijem",
    opis: "Pregledajte termine, opremu i prijavljene kvarove.",
  },
  admin: {
    naslov: "Administratorski panel",
    opis: "Upravljajte korisnicima, objektima i kabinetima.",
  },
};

const BADGE_KLASA = {
  odobreno: "zeleno",
  odobren: "zeleno",
  "na cekanju": "zuto",
  nacekanju: "zuto",
  prijavljen: "crveno",
  kvar: "crveno",
  "u obradi": "amber",
  rijeseno: "zeleno",
  "riješeno": "zeleno",
  "u popravci": "amber",
  aktivan: "zeleno",
  odbijeno: "crveno",
  odbijen: "crveno",
  rezervisan: "plavo",
  slobodan: "sivo",
  otkazan: "crveno",
};

function normalizeStatusLabel(value) {
  return (value || "").toString().trim().toLowerCase();
}

function getBadgeClass(value) {
  return BADGE_KLASA[normalizeStatusLabel(value)] || "";
}

function formatDate(value) {
  if (!value) return "N/A";
  return new Date(value).toLocaleDateString("de-DE");
}

function formatTimeRange(start, end) {
  if (!start || !end) return "N/A";
  return `${start.slice(0, 5)} - ${end.slice(0, 5)}`;
}

function isUpcomingTermin(termin) {
  if (!termin?.datum || !termin?.vrijemeKraja) {
    return false;
  }

  const terminEnd = new Date(`${termin.datum.split("T")[0]}T${termin.vrijemeKraja}`);
  return terminEnd > new Date();
}

function isTerminToday(termin) {
  if (!termin?.datum) {
    return false;
  }

  const today = new Date();
  const date = new Date(termin.datum);
  return (
    date.getFullYear() === today.getFullYear() &&
    date.getMonth() === today.getMonth() &&
    date.getDate() === today.getDate()
  );
}

function Dashboard() {
  const uloga = getCurrentRole();
  const tekst = DASHBOARD_TEXT_BY_ROLE[uloga] || DASHBOARD_TEXT_BY_ROLE.student;

  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [statCards, setStatCards] = useState([]);
  const [tableData, setTableData] = useState(null);
  const [evidencije, setEvidencije] = useState([]);
  const [loadingEvidencije, setLoadingEvidencije] = useState(false);
  const [opremaList, setOpremaList] = useState([]);

  const healthStats = useMemo(() => {
    if (!Array.isArray(opremaList) || opremaList.length === 0) {
      return null;
    }
    const total = opremaList.length;
    const ispravno = opremaList.filter((o) => Number(o.stanje) === 1).length;
    const uKvaru = opremaList.filter((o) => Number(o.stanje) === 2).length;
    const naServisu = opremaList.filter((o) => Number(o.stanje) === 3).length;
    const otpisano = opremaList.filter((o) => Number(o.stanje) === 4).length;

    const procenat = total > 0 ? Math.round((ispravno / total) * 100) : 0;

    let healthColor = "#10b981"; // green
    if (procenat < 50) {
      healthColor = "#ef4444"; // red
    } else if (procenat < 80) {
      healthColor = "#f59e0b"; // amber
    }

    return {
      total,
      ispravno,
      uKvaru,
      naServisu,
      otpisano,
      procenat,
      healthColor,
    };
  }, [opremaList]);


  useEffect(() => {
    loadDashboard();
  }, [uloga]);

  async function loadDashboard() {
    setLoading(true);
    setMessage({ type: "", text: "" });

    try {
      if (uloga === "student") {
        await loadStudentDashboard();
      } else if (uloga === "profesor") {
        await loadProfesorDashboard();
      } else if (uloga === "tehnicar") {
        await loadTehnicarDashboard();
      } else if (uloga === "admin") {
        await loadAdminDashboard();
      }
    } catch (error) {
      setMessage({
        type: "error",
        text: error?.response?.data?.message || error?.response?.data?.Message || "Neuspjesno ucitavanje dashboard podataka.",
      });
    } finally {
      setLoading(false);
    }
  }

  async function loadStudentDashboard() {
    const [mojeResponse, dostupniResponse] = await Promise.all([
      api.get("/Rezervacija/moje"),
      api.get("/Rezervacija/dostupni-studentima"),
    ]);

    const mojeRezervacije = Array.isArray(mojeResponse.data)
      ? mojeResponse.data.filter(isUpcomingTermin)
      : [];
    const dostupniTermini = Array.isArray(dostupniResponse.data)
      ? dostupniResponse.data.filter(isUpcomingTermin)
      : [];

    const pendingRequests = dostupniTermini.filter(
      (termin) => termin.statusPrijave === "NaCekanju",
    ).length;

    setStatCards([
      { label: "Aktivne rezervacije", vrijednost: String(mojeRezervacije.length), klasa: "plavo" },
      { label: "Dostupni termini", vrijednost: String(dostupniTermini.length), klasa: "" },
      { label: "Zahtjevi na cekanju", vrijednost: String(pendingRequests), klasa: "amber" },
    ]);

    setTableData({
      naslov: tekst.tabelaNaslov,
      kolone: tekst.tabelaKolone,
      redovi: mojeRezervacije.slice(0, 5).map((termin) => ([
        termin.kabinetNaziv,
        formatDate(termin.datum),
        formatTimeRange(termin.vrijemePocetka, termin.vrijemeKraja),
        termin.statusTermina,
      ])),
      emptyMessage: "Nemate aktivnih rezervacija.",
    });
  }

  async function loadProfesorDashboard() {
    const [mojeResponse, zahtjeviResponse, opremaResponse] = await Promise.all([
      api.get("/Rezervacija/moje"),
      api.get("/Rezervacija/dolazni-zahtjevi"),
      api.get("/Oprema"),
    ]);

    const mojeRezervacije = Array.isArray(mojeResponse.data)
      ? mojeResponse.data.filter(isUpcomingTermin)
      : [];
    const dolazniZahtjevi = Array.isArray(zahtjeviResponse.data)
      ? zahtjeviResponse.data
      : [];
    const oprema = Array.isArray(opremaResponse.data) ? opremaResponse.data : [];
    setOpremaList(oprema);

    const javniTermini = mojeRezervacije.filter((termin) => termin.vidljivoStudentima).length;

    setStatCards([
      { label: "Zahtjevi na cekanju", vrijednost: String(dolazniZahtjevi.length), klasa: "amber" },
      { label: "Aktivne rezervacije", vrijednost: String(mojeRezervacije.length), klasa: "plavo" },
      { label: "Javni termini", vrijednost: String(javniTermini), klasa: "" },
    ]);

    setTableData({
      naslov: tekst.tabelaNaslov,
      kolone: tekst.tabelaKolone,
      redovi: dolazniZahtjevi.slice(0, 5).map((zahtjev) => ([
        zahtjev.studentIme,
        zahtjev.kabinetNaziv,
        `${formatDate(zahtjev.datum)} ${zahtjev.vrijemePocetka.slice(0, 5)}`,
        zahtjev.statusZahtjeva,
      ])),

      emptyMessage: "Trenutno nema novih zahtjeva na cekanju.",
    });
  }

  async function loadTehnicarDashboard() {
    setLoadingEvidencije(true);
    try {
      const [terminiResponse, opremaResponse, evidencijeResponse] = await Promise.all([
        api.get("/Termin"),
        api.get("/Oprema"),
        api.get("/Evidencija"),
      ]);

      const termini = Array.isArray(terminiResponse.data) ? terminiResponse.data : [];
      const oprema = Array.isArray(opremaResponse.data) ? opremaResponse.data : [];
      const sveEvidencije = Array.isArray(evidencijeResponse.data) ? evidencijeResponse.data : [];

      setEvidencije(sveEvidencije);
      setOpremaList(oprema);

      const aktivniKvarovi = sveEvidencije.filter((evidencija) => evidencija.status === "Kvar").length;

      setStatCards([
        { label: "Termini danas", vrijednost: String(termini.filter(isTerminToday).length), klasa: "plavo" },
        { label: "Ukupna oprema", vrijednost: String(oprema.length), klasa: "" },
        { label: "Prijavljeni kvarovi", vrijednost: String(aktivniKvarovi), klasa: "crveno" },
      ]);
      setTableData(null);
    } finally {
      setLoadingEvidencije(false);
    }
  }

  async function loadAdminDashboard() {
    const [usersResponse, objektiResponse, kabinetiResponse, opremaResponse] = await Promise.all([
      api.get("/Auth/users"),
      api.get("/Objekat"),
      api.get("/Kabinet"),
      api.get("/Oprema"),
    ]);
    const users = Array.isArray(usersResponse.data) ? usersResponse.data : [];
    const objekti = Array.isArray(objektiResponse.data) ? objektiResponse.data : [];
    const kabineti = Array.isArray(kabinetiResponse.data) ? kabinetiResponse.data : [];
    const oprema = Array.isArray(opremaResponse.data) ? opremaResponse.data : [];
    setOpremaList(oprema);

    const aktivniObjekti = objekti.filter((objekat) => Array.isArray(objekat.kabineti) && objekat.kabineti.length > 0).length;

    setStatCards([
      { label: "Ukupno korisnika", vrijednost: String(users.length), klasa: "blue" },
      { label: "Aktivni objekti", vrijednost: String(aktivniObjekti), klasa: "" },

      { label: "Kabineti", vrijednost: String(kabineti.length), klasa: "" },
    ]);
    setTableData(null);
  }

  async function handleEvidencijaStatus(id, noviStatus) {
    try {
      await api.put(`/Evidencija/${id}`, { status: noviStatus });
      await loadDashboard();
    } catch (error) {
      console.error("Greska pri azuriranju statusa:", error);
    }
  }

  async function handleEvidencijaDelete(id) {
    if (!window.confirm("Da li ste sigurni da zelite obrisati ovaj kvar?")) return;

    try {
      await api.delete(`/Evidencija/${id}`);
      await loadDashboard();
    } catch (error) {
      console.error("Greska pri brisanju:", error);
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>{tekst.naslov}</h1>
        <p>{tekst.opis}</p>
      </div>

      {message.text && (
        <p className={message.type === "error" ? "form-error" : "form-success"}>
          {message.text}
        </p>
      )}

      <div className="cards-grid">
        {loading
          ? (
            <div className="card">
              <p style={{ color: "var(--text-muted)" }}>Ucitavanje statistika...</p>
            </div>
            )
          : statCards.map((stat, index) => (
            <div key={index} className={`stat-card ${stat.klasa}`}>
              <div className="stat-value">{stat.vrijednost}</div>
              <div className="stat-label">{stat.label}</div>
            </div>
          ))}
      </div>

      <div
        className="dashboard-grid-layout"
        style={{
          display: "grid",
          gridTemplateColumns: healthStats ? "repeat(auto-fit, minmax(320px, 1fr))" : "1fr",
          gap: "24px",
          marginTop: "24px"
        }}
      >
        {uloga === "tehnicar" && (
          <div className="table-wrapper" style={{ height: "fit-content" }}>
            <div className="table-header">
              <h2>Prijavljeni kvarovi</h2>
            </div>
            {loading || loadingEvidencije ? (
              <p style={{ padding: "16px" }}>Ucitavanje...</p>
            ) : evidencije.length > 0 ? (
              <table>
                <thead>
                  <tr>
                    <th>Oprema</th>
                    <th>Prijavio</th>
                    <th>Komentar</th>
                    <th>Status</th>
                    <th>Akcije</th>
                  </tr>
                </thead>
                <tbody>
                  {evidencije.map((evidencija) => (
                    <tr key={evidencija.id}>
                      <td>{evidencija.opremaNaziv}</td>
                      <td>{evidencija.korisnikImePrezime}</td>
                      <td>{evidencija.komentar}</td>
                      <td>
                        <span className={`badge ${getBadgeClass(evidencija.status)}`}>
                          {evidencija.status}
                        </span>
                      </td>
                      <td>
                        <div style={{ display: "flex", gap: "6px" }}>
                          {evidencija.status !== "Riješeno" && (
                            <button
                              className="users-action-btn"
                              onClick={() => handleEvidencijaStatus(evidencija.id, "Riješeno")}
                            >
                              Rijesi
                            </button>
                          )}
                          {evidencija.status !== "U obradi" && evidencija.status !== "Riješeno" && (
                            <button
                              className="users-action-btn"
                              onClick={() => handleEvidencijaStatus(evidencija.id, "U obradi")}
                            >
                              Obrada
                            </button>
                          )}
                          <button
                            className="users-action-btn warn"
                            onClick={() => handleEvidencijaDelete(evidencija.id)}
                          >
                            Obrisi
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <p style={{ padding: "16px", color: "var(--text-muted)" }}>
                Nema prijavljenih kvarova.
              </p>
            )}
          </div>
        )}

        {uloga !== "tehnicar" && tableData && (
          <div className="table-wrapper" style={{ height: "fit-content" }}>
            <div className="table-header">
              <h2>{tableData.naslov}</h2>
            </div>
            {loading ? (
              <p style={{ padding: "16px" }}>Ucitavanje...</p>
            ) : tableData.redovi.length > 0 ? (
              <table>
                <thead>
                  <tr>
                    {tableData.kolone.map((kolona, index) => (
                      <th key={index}>{kolona}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {tableData.redovi.map((red, rowIndex) => (
                    <tr key={rowIndex}>
                      {red.map((celija, cellIndex) => (
                        <td key={cellIndex}>
                          {getBadgeClass(celija)
                            ? <span className={`badge ${getBadgeClass(celija)}`}>{celija}</span>
                            : celija}
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <p style={{ padding: "16px", color: "var(--text-muted)" }}>
                {tableData.emptyMessage}
              </p>
            )}
          </div>
        )}

        {healthStats && (
          <div
            className="card health-widget-card"
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              padding: "24px",
              height: "fit-content",
              boxShadow: "0 10px 30px rgba(0, 0, 0, 0.04)"
            }}
          >
            <h3
              style={{
                fontSize: "16px",
                fontWeight: "600",
                color: "var(--text-primary)",
                marginBottom: "20px",
                alignSelf: "flex-start"
              }}
            >
              Zdravlje laboratorije
            </h3>

            <div
              style={{
                position: "relative",
                width: "150px",
                height: "150px",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                marginBottom: "20px"
              }}
            >
              <svg width="150" height="150" viewBox="0 0 150 150" style={{ transform: "rotate(-90deg)" }}>
                <circle
                  cx="75"
                  cy="75"
                  r="60"
                  fill="transparent"
                  stroke="var(--border-color)"
                  strokeWidth="10"
                />
                <circle
                  cx="75"
                  cy="75"
                  r="60"
                  fill="transparent"
                  stroke={healthStats.healthColor}
                  strokeWidth="10"
                  strokeDasharray={2 * Math.PI * 60}
                  strokeDashoffset={2 * Math.PI * 60 * (1 - healthStats.procenat / 100)}
                  strokeLinecap="round"
                  style={{ transition: "stroke-dashoffset 0.8s ease-in-out" }}
                />
              </svg>
              <div style={{ position: "absolute", textAlign: "center" }}>
                <span style={{ fontSize: "28px", fontWeight: "800", color: "var(--text-primary)" }}>
                  {healthStats.procenat}%
                </span>
                <div style={{ fontSize: "12px", color: "var(--text-muted)", marginTop: "2px" }}>
                  Ispravno
                </div>
              </div>
            </div>

            <div style={{ width: "100%", display: "grid", gap: "10px", marginTop: "10px" }}>
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  fontSize: "13px",
                  paddingBottom: "6px",
                  borderBottom: "1px solid var(--border-color)"
                }}
              >
                <span style={{ display: "flex", alignItems: "center", gap: "6px" }}>
                  <span style={{ width: "8px", height: "8px", borderRadius: "50%", background: "#10b981" }} />
                  Ispravna oprema
                </span>
                <strong style={{ color: "var(--text-primary)" }}>
                  {healthStats.ispravno} / {healthStats.total}
                </strong>
              </div>
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  fontSize: "13px",
                  paddingBottom: "6px",
                  borderBottom: "1px solid var(--border-color)"
                }}
              >
                <span style={{ display: "flex", alignItems: "center", gap: "6px" }}>
                  <span style={{ width: "8px", height: "8px", borderRadius: "50%", background: "#ef4444" }} />
                  U kvaru
                </span>
                <strong style={{ color: "var(--text-primary)" }}>{healthStats.uKvaru}</strong>
              </div>
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  fontSize: "13px",
                  paddingBottom: "6px",
                  borderBottom: "1px solid var(--border-color)"
                }}
              >
                <span style={{ display: "flex", alignItems: "center", gap: "6px" }}>
                  <span style={{ width: "8px", height: "8px", borderRadius: "50%", background: "#f59e0b" }} />
                  Na servisu
                </span>
                <strong style={{ color: "var(--text-primary)" }}>{healthStats.naServisu}</strong>
              </div>
              {healthStats.otpisano > 0 && (
                <div style={{ display: "flex", justifyContent: "space-between", fontSize: "13px", paddingBottom: "6px" }}>
                  <span style={{ display: "flex", alignItems: "center", gap: "6px" }}>
                    <span style={{ width: "8px", height: "8px", borderRadius: "50%", background: "#6b7280" }} />
                    Otpisana oprema
                  </span>
                  <strong style={{ color: "var(--text-primary)" }}>{healthStats.otpisano}</strong>
                </div>
              )}
            </div>
          </div>
        )}
      </div>

    </Layout>
  );
}

export default Dashboard;
