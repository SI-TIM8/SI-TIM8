import { useEffect, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

const BADGE_KLASA = {
  "Kvar": "crveno",
  "U obradi": "amber",
  "Riješeno": "zeleno",
};

function Kvarovi() {
  const [evidencije, setEvidencije] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadEvidencije();
  }, []);

  async function loadEvidencije() {
    setLoading(true);
    try {
      const response = await api.get("/Evidencija");
      setEvidencije(response.data);
    } catch (error) {
      console.error("Greška pri učitavanju evidencija:", error);
    } finally {
      setLoading(false);
    }
  }

  async function handleStatus(id, noviStatus) {
    try {
      await api.put(`/Evidencija/${id}`, { status: noviStatus });
      await loadEvidencije();
    } catch (error) {
      console.error("Greška pri ažuriranju statusa:", error);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Da li ste sigurni da želite obrisati ovaj kvar?")) return;
    try {
      await api.delete(`/Evidencija/${id}`);
      await loadEvidencije();
    } catch (error) {
      console.error("Greška pri brisanju:", error);
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Kvarovi opreme</h1>
      </div>

      <div className="table-wrapper">
        <div className="table-header">
          <h2>Prijavljeni kvarovi</h2>
        </div>
        {loading ? (
          <p style={{ padding: "16px" }}>Učitavanje...</p>
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
              {evidencije.map((e) => (
                <tr key={e.id}>
                  <td>{e.opremaNaziv}</td>
                  <td>{e.korisnikImePrezime}</td>
                  <td>{e.komentar}</td>
                  <td>
                    <span className={`badge ${BADGE_KLASA[e.status] || ""}`}>
                      {e.status}
                    </span>
                  </td>
                  <td>
                    <div style={{ display: "flex", gap: "6px" }}>
                      {e.status !== "Riješeno" && (
                        <button
                          className="users-action-btn"
                          onClick={() => handleStatus(e.id, "Riješeno")}
                        >
                          ✓ Riješi
                        </button>
                      )}
                      {e.status !== "U obradi" && e.status !== "Riješeno" && (
                        <button
                          className="users-action-btn"
                          onClick={() => handleStatus(e.id, "U obradi")}
                        >
                          🔧 Obrada
                        </button>
                      )}
                      <button
                        className="users-action-btn warn"
                        onClick={() => handleDelete(e.id)}
                      >
                        🗑 Briši
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
    </Layout>
  );
}

export default Kvarovi;