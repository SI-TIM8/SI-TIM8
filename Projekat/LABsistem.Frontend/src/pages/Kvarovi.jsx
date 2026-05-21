import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

const STATUS_META = {
  Kvar: { label: "Kvar", color: "crveno" },
  "U obradi": { label: "U obradi", color: "zuto" },
  Riješeno: { label: "Riješeno", color: "zeleno" },
  Rijeseno: { label: "Riješeno", color: "zeleno" },
};

function formatDateTime(value) {
  if (!value) return "N/A";
  return new Date(value).toLocaleString("de-DE");
}

function formatTime(value) {
  if (!value) return "";
  return value.slice(0, 5);
}

function Kvarovi() {
  const [evidencije, setEvidencije] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [selected, setSelected] = useState(null);
  const [resolutionText, setResolutionText] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    loadEvidencije();
  }, []);

  async function loadEvidencije() {
    setLoading(true);
    try {
      const response = await api.get("/Evidencija");
      setEvidencije(response.data || []);
    } catch {
      setMessage({ type: "error", text: "Neuspjesno ucitavanje evidencija." });
    } finally {
      setLoading(false);
    }
  }

  function openDetails(item) {
    setSelected(item);
    setResolutionText(item.rjesenje || "");
  }

  async function updateStatus(status) {
    if (!selected) return;

    if (status === "Riješeno" && !resolutionText.trim()) {
      setMessage({ type: "error", text: "Unesite kako je problem rijesen." });
      return;
    }

    setSaving(true);
    try {
      await api.put(`/Evidencija/${selected.id}`, {
        status,
        rjesenje: status === "Riješeno" ? resolutionText.trim() : null,
      });

      setMessage({
        type: "success",
        text: status === "Riješeno" ? "Problem oznacen kao rijesen." : "Oprema je privremeno iskljucena.",
      });
      setSelected(null);
      setResolutionText("");
      await loadEvidencije();
    } catch (error) {
      setMessage({ type: "error", text: error.response?.data || "Greska pri azuriranju statusa." });
    } finally {
      setSaving(false);
    }
  }

  const sortedEvidencije = useMemo(() => {
    return [...evidencije].sort((left, right) => {
      return String(right.prijavljenoU || "").localeCompare(String(left.prijavljenoU || ""));
    });
  }, [evidencije]);

  return (
    <Layout>
      <div className="page-header">
        <h1>Kvarovi opreme</h1>
        <p>Pregled prijavljenih kvarova i obrada opreme od strane tehnicara.</p>
      </div>

      {message.text && (
        <div className={message.type === "error" ? "form-error" : "form-success"} style={{ marginBottom: 16 }}>
          {message.text}
        </div>
      )}

      <div className="card">
        {loading ? (
          <div className="users-empty-state">Ucitavanje kvarova...</div>
        ) : sortedEvidencije.length > 0 ? (
          <div className="users-list">
            <div className="termini-list-header termini-list-row">
              <span>Oprema</span>
              <span>Prijavio</span>
              <span>Termin</span>
              <span>Prijavljeno</span>
              <span>Status</span>
            </div>
            {sortedEvidencije.map((evidencija) => {
              const meta = STATUS_META[evidencija.status] || STATUS_META.Kvar;
              return (
                <div
                  className="termini-list-row users-list-item"
                  key={evidencija.id}
                  style={{ cursor: "pointer" }}
                  onClick={() => openDetails(evidencija)}
                >
                  <span style={{ fontWeight: 700 }}>{evidencija.opremaNaziv}</span>
                  <span>{evidencija.korisnikImePrezime}</span>
                  <span>
                    {evidencija.terminDatum ? formatDateTime(evidencija.terminDatum) : "N/A"}
                    {evidencija.terminVrijemePocetka && (
                      <div>
                        <small className="badge plavo">
                          {formatTime(evidencija.terminVrijemePocetka)} - {formatTime(evidencija.terminVrijemeKraja)}
                        </small>
                      </div>
                    )}
                  </span>
                  <span>{formatDateTime(evidencija.prijavljenoU)}</span>
                  <span>
                    <span className={`badge ${meta.color}`}>{meta.label}</span>
                  </span>
                </div>
              );
            })}
          </div>
        ) : (
          <div className="users-empty-state">Nema prijavljenih kvarova.</div>
        )}
      </div>

      {selected && (
        <div className="users-modal-overlay" onClick={() => setSelected(null)}>
          <div className="users-modal" onClick={(event) => event.stopPropagation()} style={{ maxWidth: 900 }}>
            <div className="users-modal-header">
              <h2>Detalji kvara</h2>
              <button className="users-modal-close" onClick={() => setSelected(null)}>×</button>
            </div>

            <div style={{ display: "grid", gap: 12, marginBottom: 20, color: "var(--text)" }}>
              <div><strong>Oprema:</strong> {selected.opremaNaziv}</div>
              <div><strong>Kategorija:</strong> {selected.opremaKategorija || "N/A"}</div>
              <div><strong>Serijski broj:</strong> {selected.opremaSerijskiBroj || "N/A"}</div>
              <div><strong>Kabinet:</strong> {selected.opremaKabinetNaziv || "N/A"}</div>
              <div><strong>Zgrada:</strong> {selected.opremaZgradaNaziv || "N/A"}</div>
              <div><strong>Trenutno stanje:</strong> {selected.opremaStanje === 1 ? "Ispravno" : selected.opremaStanje === 2 ? "U kvaru" : selected.opremaStanje === 3 ? "Na servisu" : selected.opremaStanje === 4 ? "Otpisano" : "N/A"}</div>
              <div><strong>Prijavio:</strong> {selected.korisnikImePrezime}</div>
              <div><strong>Profesor:</strong> {selected.profesorImePrezime || "N/A"}</div>
              <div><strong>Termin:</strong> {selected.terminDatum ? formatDateTime(selected.terminDatum) : "N/A"} {selected.terminVrijemePocetka ? `${formatTime(selected.terminVrijemePocetka)} - ${formatTime(selected.terminVrijemeKraja)}` : ""}</div>
              <div><strong>Prijavljeno:</strong> {formatDateTime(selected.prijavljenoU)}</div>
              <div><strong>Komentar:</strong> {selected.komentar || "Nema komentara."}</div>
              <div><strong>Rješenje:</strong> {selected.rjesenje || "Nije uneseno."}</div>
              <div><strong>Obradio:</strong> {selected.obradioImePrezime || "N/A"}</div>
              <div><strong>Status:</strong> <span className={`badge ${(STATUS_META[selected.status] || STATUS_META.Kvar).color}`}>{selected.status}</span></div>
            </div>

            <div style={{ marginBottom: 16 }}>
              <label style={{ display: "block", marginBottom: 8, fontWeight: 600 }}>Rješenje problema</label>
              <textarea
                value={resolutionText}
                onChange={(event) => setResolutionText(event.target.value)}
                rows={4}
                maxLength={500}
                placeholder="Npr. Zamijenjen kabl napajanja i testirano na drugom portu."
                style={{
                  width: "100%",
                  padding: "8px",
                  borderRadius: "6px",
                  border: "1px solid var(--border)",
                  resize: "vertical",
                  background: "var(--input-bg)",
                  color: "var(--text)",
                  fontSize: "14px",
                  boxSizing: "border-box",
                }}
              />
            </div>

            <div className="users-modal-actions" style={{ flexWrap: "wrap" }}>
              <button className="button warn" type="button" disabled={saving} onClick={() => updateStatus("U obradi")}>Privremeno ugasi</button>
              <button className="button" type="button" disabled={saving} onClick={() => updateStatus("Riješeno")}>Rijesen problem</button>
              <button className="button sekundarno" type="button" onClick={() => setSelected(null)}>Zatvori</button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}

export default Kvarovi;