import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentRole } from "../auth/routeAccess";

function Rezervacije() {
  const [rezervacije, setRezervacije] = useState([]);
  const [zahtjevi, setZahtjevi] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [aktivniTab, setAktivniTab] = useState("rezervacije");
  const uloga = getCurrentRole();

  useEffect(() => {
    loadPodatke();
  }, []);

  function jeBuduciTermin(datum, vrijemeKraja) {
    const datumDio = datum?.split("T")[0];
    if (!datumDio || !vrijemeKraja) return false;

    const krajTermina = new Date(`${datumDio}T${vrijemeKraja}`);
    return krajTermina > new Date();
  }

  function klasaStatusaZahtjeva(status) {
    switch (status) {
      case "NaCekanju":
        return "plavo";
      case "Odbijen":
        return "crveno";
      case "Otkazan":
        return "sivo";
      default:
        return "zeleno";
    }
  }

  function labelaStatusaZahtjeva(status) {
    switch (status) {
      case "NaCekanju":
        return "Na čekanju";
      case "Odbijen":
        return "Odbijen";
      case "Otkazan":
        return "Otkazan";
      case "Odobren":
        return "Odobren";
      default:
        return status;
    }
  }

  async function loadPodatke() {
    setLoading(true);
    try {
      if (uloga === "student") {
        const [rezervacijeResponse, zahtjeviResponse] = await Promise.all([
          api.get("/Rezervacija/moje"),
          api.get("/Rezervacija/moji-zahtjevi"),
        ]);

        const aktivneRezervacije = rezervacijeResponse.data.filter((termin) =>
          jeBuduciTermin(termin.datum, termin.vrijemeKraja)
        );

        const aktivniZahtjevi = zahtjeviResponse.data.filter((zahtjev) =>
          jeBuduciTermin(zahtjev.datum, zahtjev.vrijemeKraja)
        );

        setRezervacije(aktivneRezervacije);
        setZahtjevi(aktivniZahtjevi);
      } else {
        const response = await api.get("/Rezervacija/moje");
        const filtriraniTermini = response.data.filter((termin) =>
          jeBuduciTermin(termin.datum, termin.vrijemeKraja)
        );

        setRezervacije(filtriraniTermini);
        setZahtjevi([]);
      }
    } catch {
      setMessage({ type: "error", text: "Neuspješno učitavanje rezervacija." });
    } finally {
      setLoading(false);
    }
  }

  async function otkaziRezervaciju(id) {
    const confirmMessage =
      uloga === "student"
        ? "Da li ste sigurni da želite otkazati svoj dolazak na termin?"
        : "Da li ste sigurni da želite otkazati rezervaciju?";

    if (!window.confirm(confirmMessage)) return;

    try {
      await api.post(`/Rezervacija/otkazi/${id}`);
      setMessage({ type: "success", text: "Rezervacija je uspješno otkazana." });
      await loadPodatke();
    } catch (error) {
      setMessage({ type: "error", text: error.response?.data || "Greška pri otkazivanju rezervacije." });
    }
  }

  async function otkaziZahtjev(zahtjevId) {
    if (!window.confirm("Da li ste sigurni da želite poništiti ovaj zahtjev?")) return;

    try {
      await api.post(`/Rezervacija/otkazi-zahtjev/${zahtjevId}`);
      setMessage({ type: "success", text: "Zahtjev je uspješno otkazan." });
      await loadPodatke();
    } catch (error) {
      setMessage({ type: "error", text: error.response?.data || "Greška pri otkazivanju zahtjeva." });
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>{uloga === "profesor" ? "Lista rezervacija" : "Moje rezervacije"}</h1>
        <p>
          {uloga === "profesor"
            ? "Termini koje ste rezervisali za nastavu."
            : "Pregled vaših odobrenih rezervacija i poslanih zahtjeva."}
        </p>
      </div>

      <div className="card">
        {message.text && (
          <p className={message.type === "error" ? "form-error" : "form-success"}>
            {message.text}
          </p>
        )}

        {uloga === "student" && (
          <div className="tabs">
            <button
              type="button"
              className={`tab-btn ${aktivniTab === "rezervacije" ? "active" : ""}`}
              onClick={() => setAktivniTab("rezervacije")}
            >
              Aktivne rezervacije
            </button>
            <button
              type="button"
              className={`tab-btn ${aktivniTab === "zahtjevi" ? "active" : ""}`}
              onClick={() => setAktivniTab("zahtjevi")}
            >
              Moji zahtjevi
            </button>
          </div>
        )}

        {aktivniTab === "rezervacije" || uloga !== "student" ? (
          <>
            <div className="termini-list-header termini-list-row">
              <span>Datum</span>
              <span>Vrijeme</span>
              <span>Kabinet</span>
              {uloga === "student" && <span>Profesor</span>}
              <span>Status</span>
              {(uloga === "profesor" || uloga === "student") && <span>Akcija</span>}
            </div>

            {loading ? (
              <div className="users-empty-state">Učitavanje...</div>
            ) : rezervacije.length > 0 ? (
              <div className="users-list">
                {rezervacije.map((termin) => (
                  <div className="termini-list-row users-list-item" key={termin.id}>
                    <span style={{ fontWeight: 700 }}>
                      {new Date(termin.datum).toLocaleDateString("de-DE")}
                    </span>
                    <span>
                      <span className="badge plavo">
                        {termin.vrijemePocetka.slice(0, 5)} - {termin.vrijemeKraja.slice(0, 5)}
                      </span>
                    </span>
                    <span>{termin.kabinetNaziv}</span>
                    {uloga === "student" && <span>{termin.profesorIme}</span>}
                    <span>
                      <span className={`badge ${termin.statusTermina === "Slobodan" ? "sivo" : "zeleno"}`}>
                        {termin.statusTermina}
                      </span>
                    </span>
                    {(uloga === "profesor" || uloga === "student") && (
                      <span>
                        <button className="button warn" onClick={() => otkaziRezervaciju(termin.id)}>
                          Otkaži
                        </button>
                      </span>
                    )}
                  </div>
                ))}
              </div>
            ) : (
              <div className="users-empty-state">Nema aktivnih rezervacija.</div>
            )}
          </>
        ) : (
          <>
            <div className="termini-list-header termini-list-row">
              <span>Datum</span>
              <span>Vrijeme</span>
              <span>Kabinet</span>
              <span>Profesor</span>
              <span>Status zahtjeva</span>
              <span>Akcija</span>
            </div>

            {loading ? (
              <div className="users-empty-state">Učitavanje...</div>
            ) : zahtjevi.length > 0 ? (
              <div className="users-list">
                {zahtjevi.map((zahtjev) => (
                  <div className="termini-list-row users-list-item" key={zahtjev.id}>
                    <span style={{ fontWeight: 700 }}>
                      {new Date(zahtjev.datum).toLocaleDateString("de-DE")}
                    </span>
                    <span>
                      <span className="badge plavo">
                        {zahtjev.vrijemePocetka.slice(0, 5)} - {zahtjev.vrijemeKraja.slice(0, 5)}
                      </span>
                    </span>
                    <span>{zahtjev.kabinetNaziv}</span>
                    <span>{zahtjev.profesorIme}</span>
                    <span>
                      <span className={`badge ${klasaStatusaZahtjeva(zahtjev.statusZahtjeva)}`}>
                        {labelaStatusaZahtjeva(zahtjev.statusZahtjeva)}
                      </span>
                    </span>
                    <span>
                      {zahtjev.mozeOtkazati ? (
                        <button className="button warn" onClick={() => otkaziZahtjev(zahtjev.id)}>
                          Poništi zahtjev
                        </button>
                      ) : (
                        <span className="badge sivo">Nema akcije</span>
                      )}
                    </span>
                  </div>
                ))}
              </div>
            ) : (
              <div className="users-empty-state">Nema aktivnih zahtjeva.</div>
            )}
          </>
        )}
      </div>
    </Layout>
  );
}

export default Rezervacije;
