import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

function Zakazivanje() {
  const [termini, setTermini] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [equipmentModalOpen, setEquipmentModalOpen] = useState(false);
  const [selectedCabinetEquipment, setSelectedCabinetEquipment] = useState([]);
  const [selectedCabinetName, setSelectedCabinetName] = useState("");
  const [loadingEquipment, setLoadingEquipment] = useState(false);

  useEffect(() => {
    loadDostupniTermini();
  }, []);

  async function loadDostupniTermini() {
    setLoading(true);
    try {
      const response = await api.get("/Rezervacija/dostupni-studentima");
      const filtered = response.data.filter((t) => {
        const terminEnd = new Date(
          `${t.datum.split("T")[0]}T${t.vrijemeKraja}`,
        );
        return terminEnd > new Date();
      });
      setTermini(filtered);
    } catch (error) {
      setMessage({ type: "error", text: "Neuspjesno ucitavanje termina." });
    } finally {
      setLoading(false);
    }
  }

  async function posaljiZahtjev(id) {
    // Odmah ažuriraj UI optimistički
    setTermini((prev) =>
      prev.map((t) => (t.id === id ? { ...t, statusPrijave: "NaCekanju" } : t)),
    );
    try {
      await api.post(`/Rezervacija/zahtjev/${id}`);
      setMessage({ type: "success", text: "Zahtjev uspjesno poslan." });
    } catch (error) {
      // Vrati na staro ako faila
      setTermini((prev) =>
        prev.map((t) => (t.id === id ? { ...t, statusPrijave: null } : t)),
      );
      setMessage({
        type: "error",
        text: error.response?.data || "Greska pri slanju zahtjeva.",
      });
    }
  }

  async function loadEquipment(kabinetId, kabinetNaziv) {
    setLoadingEquipment(true);
    setSelectedCabinetName(kabinetNaziv);
    setEquipmentModalOpen(true);
    try {
      const response = await api.get(`/Oprema/kabinet/${kabinetId}`);
      setSelectedCabinetEquipment(response.data);
    } catch (error) {
      console.error("Greska pri ucitavanju opreme:", error);
    } finally {
      setLoadingEquipment(false);
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Zakazi termin</h1>
        <p>Prijavite se na termine koje su profesori ucinili dostupnim.</p>
      </div>

      <div className="card">
        {message.text && (
          <p
            className={message.type === "error" ? "form-error" : "form-success"}
          >
            {message.text}
          </p>
        )}

        <div className="termini-list-header termini-list-row">
          <span>Datum</span>
          <span>Vrijeme</span>
          <span>Kabinet</span>
          <span>Profesor</span>
          <span>Popunjenost</span>
          <span>Vidljivost</span>
          <span>Akcija</span>
        </div>

        {loading ? (
          <div className="users-empty-state">Ucitavanje termina...</div>
        ) : termini.length > 0 ? (
          <div className="users-list">
            {termini
              .filter((t) => t.statusPrijave !== "Odobren") // Ako je odobren, on je u "Moje rezervacije"
              .map((t) => (
                <div className="termini-list-row users-list-item" key={t.id}>
                  <span style={{ fontWeight: 700 }}>
                    {new Date(t.datum).toLocaleDateString("de-DE")}
                  </span>
                  <span>
                    <span className="badge plavo">
                      {t.vrijemePocetka.slice(0, 5)} -{" "}
                      {t.vrijemeKraja.slice(0, 5)}
                    </span>
                  </span>
                  <span>
                    <button
                      className="text-button"
                      onClick={() => loadEquipment(t.kabinetID, t.kabinetNaziv)}
                      style={{
                        background: "none",
                        border: "none",
                        color: "#2563eb",
                        cursor: "pointer",
                        padding: 0,
                        fontSize: "inherit",
                        fontWeight: "inherit",
                        textDecoration: "underline",
                      }}
                    >
                      {t.kabinetNaziv}
                    </button>
                  </span>
                  <span>{t.profesorIme}</span>
                  <span>
                    {t.brojOdobrenih} / {t.limitOsoba || "∞"}
                  </span>
                  <span>
                    <span className="badge sivo">Javno</span>
                  </span>
                  <span>
                    {t.statusPrijave === "NaCekanju" ? (
                      <span className="badge crveno">Zahtjev u obradi</span>
                    ) : t.statusPrijave === "Odbijen" ? (
                      <span className="badge crveno">Odbijeno</span>
                    ) : (
                      <button
                        className="button"
                        onClick={() => posaljiZahtjev(t.id)}
                      >
                        Pošalji zahtjev
                      </button>
                    )}
                  </span>
                </div>
              ))}
          </div>
        ) : (
          <div className="users-empty-state">
            Trenutno nema dostupnih termina za prijavu.
          </div>
        )}
      </div>

      {/* Oprema Modal */}
      {equipmentModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content" style={{ maxWidth: "500px" }}>
            <div className="modal-header">
              <h2>Oprema u kabinetu: {selectedCabinetName}</h2>
              <button
                className="close-button"
                onClick={() => setEquipmentModalOpen(false)}
              >
                &times;
              </button>
            </div>
            <div className="modal-body">
              {loadingEquipment ? (
                <p>Učitavanje opreme...</p>
              ) : selectedCabinetEquipment.length > 0 ? (
                <table className="equipment-table">
                  <thead>
                    <tr>
                      <th>Naziv</th>
                      <th>Tip opreme</th>
                      <th>Serijski broj</th>
                      <th>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {selectedCabinetEquipment.map((o) => (
                      <tr key={o.id}>
                        <td>{o.naziv}</td>
                        <td>{o.kategorija || "N/A"}</td>
                        <td>{o.serijskiBroj}</td>
                        <td>
                          <span
                            className={`badge ${o.stanje === 1 ? "zeleno" : "crveno"}`}
                          >
                            {o.stanje === 1 ? "U funkciji" : "Kvar"}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              ) : (
                <p>Ovaj kabinet trenutno nema registrovane opreme.</p>
              )}
            </div>
            <div className="modal-footer">
              <button
                className="button secondary"
                onClick={() => setEquipmentModalOpen(false)}
              >
                Zatvori
              </button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}

export default Zakazivanje;
