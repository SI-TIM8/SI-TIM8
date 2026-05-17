import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

function Zahtjevi() {
  const [zahtjevi, setZahtjevi] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [komentarModal, setKomentarModal] = useState(null);
  const [komentar, setKomentar] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => { loadZahtjevi(); }, []);

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

  function otvoriOdgovor(zahtjevId, odobri) {
    setKomentar("");
    setKomentarModal({ zahtjevId, odobri });
  }

  async function posaljiOdgovor(e) {
    e.preventDefault();
    setSaving(true);
    try {
      const { zahtjevId, odobri } = komentarModal;
      await api.post(`/Rezervacija/odgovor/${zahtjevId}?odobri=${odobri}&komentar=${encodeURIComponent(komentar)}`);
      setMessage({ type: "success", text: odobri ? "Zahtjev odobren." : "Zahtjev odbijen." });
      setKomentarModal(null);
      loadZahtjevi();
    } catch (error) {
      setMessage({ type: "error", text: error.response?.data || "Greska pri obradi zahtjeva." });
    } finally {
      setSaving(false);
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
                    <button className="button" onClick={() => otvoriOdgovor(z.id, true)}>
                      Odobri
                    </button>
                    <button className="button warn" onClick={() => otvoriOdgovor(z.id, false)}>
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

      {komentarModal && (
        <div className="users-modal-overlay" onClick={() => setKomentarModal(null)}>
          <div className="users-modal" onClick={e => e.stopPropagation()}>
            <div className="users-modal-header">
              <h2>{komentarModal.odobri ? "Odobri zahtjev" : "Odbij zahtjev"}</h2>
              <button className="users-modal-close" onClick={() => setKomentarModal(null)}>×</button>
            </div>
            <form onSubmit={posaljiOdgovor}>
              <div className="form-group">
                <label>Komentar za studenta (opcionalno)</label>
                <textarea
                  value={komentar}
                  onChange={e => setKomentar(e.target.value)}
                  rows={3}
                  maxLength={200}
                  placeholder="npr. Molimo dođite 5 minuta ranije..."
                  style={{
                    width: "100%", padding: "8px", borderRadius: "6px",
                    border: "1px solid var(--border)", resize: "vertical",
                    background: "var(--input-bg)", color: "var(--text)",
                    fontSize: "14px", boxSizing: "border-box"
                  }}
                />
              </div>
              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : komentarModal.odobri ? "Potvrdi odobrenje" : "Potvrdi odbijanje"}
                </button>
                <button className="button sekundarno" type="button" onClick={() => setKomentarModal(null)}>
                  Odustani
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Layout>
  );
}

export default Zahtjevi;