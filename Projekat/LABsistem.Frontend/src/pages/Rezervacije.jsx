import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentRole } from "../auth/routeAccess";

function Rezervacije() {
  const [termini, setTermini] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const uloga = getCurrentRole();

  useEffect(() => {
    loadMojeRezervacije();
  }, []);

  async function loadMojeRezervacije() {
    setLoading(true);
    try {
      const response = await api.get("/Rezervacija/moje");
      const filtered = response.data.filter((t) => {
        const terminEnd = new Date(`${t.datum.split("T")[0]}T${t.vrijemeKraja}`);
        return terminEnd > new Date();
      });
      setTermini(filtered);
    } catch (error) {
      setMessage({ type: "error", text: "Neuspjesno ucitavanje rezervacija." });
    } finally {
      setLoading(false);
    }
  }

  async function otkaziRezervaciju(id) {
    if (!window.confirm("Da li ste sigurni da zelite otkazati rezervaciju?")) return;
    try {
      await api.post(`/Rezervacija/otkazi/${id}`);
      setMessage({ type: "success", text: "Rezervacija uspjesno otkazana." });
      loadMojeRezervacije();
    } catch (error) {
      setMessage({ type: "error", text: error.response?.data || "Greska pri otkazivanju." });
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>{uloga === "profesor" ? "Lista rezervacija" : "Moje rezervacije"}</h1>
        <p>{uloga === "profesor" ? "Termini koje ste rezervisali za nastavu." : "Termini na kojima ste prihvaceni."}</p>
      </div>

      <div className="card">
        {message.text && (
          <p className={message.type === "error" ? "form-error" : "form-success"}>
            {message.text}
          </p>
        )}

        <div className="termini-list-header termini-list-row">
          <span>Datum</span>
          <span>Vrijeme</span>
          <span>Kabinet</span>
          {uloga === "student" && <span>Profesor</span>}
          <span>Status</span>
          {uloga === "profesor" && <span>Akcija</span>}
        </div>

        {loading ? (
          <div className="users-empty-state">Ucitavanje...</div>
        ) : termini.length > 0 ? (
          <div className="users-list">
            {termini.map((t) => (
              <div className="termini-list-row users-list-item" key={t.id}>
                <span style={{ fontWeight: 700 }}>{new Date(t.datum).toLocaleDateString("de-DE")}</span>
                <span>
                  <span className="badge plavo">
                    {t.vrijemePocetka.slice(0, 5)} - {t.vrijemeKraja.slice(0, 5)}
                  </span>
                </span>
                <span>{t.kabinetNaziv}</span>
                {uloga === "student" && <span>{t.profesorIme}</span>}
                <span>
                  <span className={`badge ${t.statusTermina === "Slobodan" ? "sivo" : "zeleno"}`}>
                    {t.statusTermina}
                  </span>
                </span>
                {uloga === "profesor" && (
                  <span>
                    <button className="button warn" onClick={() => otkaziRezervaciju(t.id)}>
                      Otkazi
                    </button>
                  </span>
                )}
              </div>
            ))}
          </div>
        ) : (
          <div className="users-empty-state">Nema aktivnih rezervacija.</div>
        )}
      </div>
    </Layout>
  );
}

export default Rezervacije;
