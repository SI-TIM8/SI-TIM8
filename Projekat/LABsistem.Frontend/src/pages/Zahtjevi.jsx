import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

function Zahtjevi() {
  const [zahtjevi, setZahtjevi] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });

  useEffect(() => {
    loadZahtjevi();
  }, []);

  async function loadZahtjevi() {
    setLoading(true);
    try {
      const response = await api.get("/Rezervacija/dolazni-zahtjevi");
      setZahtjevi(response.data);
    } catch (error) {
      setMessage({ type: "error", text: "Neuspjesno ucitavanje zahtjeva." });
    } finally {
      setLoading(false);
    }
  }

  async function odgovori(zahtjevId, odobri) {
    try {
      await api.post(`/Rezervacija/odgovor/${zahtjevId}?odobri=${odobri}`);
      setMessage({ type: "success", text: odobri ? "Zahtjev odobren." : "Zahtjev odbijen." });
      loadZahtjevi();
    } catch (error) {
      setMessage({ type: "error", text: error.response?.data || "Greska pri obradi zahtjeva." });
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Zahtjevi studenata</h1>
        <p>Upravljajte prijavama studenata za vase rezervisane termine.</p>
      </div>

      <div className="card">
        {message.text && (
          <p className={message.type === "error" ? "form-error" : "form-success"}>
            {message.text}
          </p>
        )}

        <div className="termini-list-header termini-list-row">
          <span>Student</span>
          <span>Kabinet</span>
          <span>Datum i Vrijeme</span>
          <span>Status</span>
          <span>Akcije</span>
        </div>

        {loading ? (
          <div className="users-empty-state">Ucitavanje zahtjeva...</div>
        ) : zahtjevi.length > 0 ? (
          <div className="users-list">
            {zahtjevi.map((z) => (
              <div className="termini-list-row users-list-item" key={z.id}>
                <span style={{ fontWeight: 700 }}>{z.studentIme}</span>
                <span>{z.kabinetNaziv}</span>
                <span>
                  {new Date(z.datum).toLocaleDateString("de-DE")} <br />
                  <small className="badge plavo">
                    {z.vrijemePocetka.slice(0, 5)} - {z.vrijemeKraja.slice(0, 5)}
                  </small>
                </span>
                <span>
                  <span className="badge sivo">{z.statusZahtjeva}</span>
                </span>
                <span>
                  <div className="users-actions">
                    <button className="button" onClick={() => odgovori(z.id, true)}>
                      Odobri
                    </button>
                    <button className="button warn" onClick={() => odgovori(z.id, false)}>
                      Odbij
                    </button>
                  </div>
                </span>
              </div>
            ))}
          </div>
        ) : (
          <div className="users-empty-state">Nema novih zahtjeva na cekanju.</div>
        )}
      </div>
    </Layout>
  );
}

export default Zahtjevi;
