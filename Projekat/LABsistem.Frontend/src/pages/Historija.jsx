import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentRole } from "../auth/routeAccess";
import { exportHistorijaCSV, exportHistorijaPDF } from "../utils/exportUtils";

function formatDate(value) {
  if (!value) return "N/A";
  return new Date(value).toLocaleDateString("de-DE");
}

function formatTime(value) {
  if (!value) return "";
  return value.slice(0, 5);
}

function toDateTime(termin) {
  if (!termin?.datum || !termin?.vrijemeKraja) return null;
  const datePart = termin.datum.split("T")[0];
  const timePart = termin.vrijemeKraja;
  return new Date(`${datePart}T${timePart}`);
}

function canReportFault(termin) {
  const end = toDateTime(termin);
  if (!end) return false;

  const now = new Date();
  const allowedUntil = new Date(end.getTime() + 24 * 60 * 60 * 1000);
  return now >= end && now <= allowedUntil;
}

function Historija() {
  const role = getCurrentRole();
  const [termini, setTermini] = useState([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [selectedTermin, setSelectedTermin] = useState(null);
  const [selectedEquipment, setSelectedEquipment] = useState([]);
  const [equipmentLoading, setEquipmentLoading] = useState(false);
  const [reportModal, setReportModal] = useState(null);
  const [komentar, setKomentar] = useState("");
  const [saving, setSaving] = useState(false);
  const [filterOd, setFilterOd] = useState("");
  const [filterDo, setFilterDo] = useState("");
  const [filterKabinet, setFilterKabinet] = useState("");

  const kabineti = [...new Set(termini.map((t) => t.kabinetNaziv).filter(Boolean))].sort();

  function resetujFiltere() {
    setFilterOd("");
    setFilterDo("");
    setFilterKabinet("");
  }

  const visibleEquipment = useMemo(() => {
    return selectedEquipment.filter((oprema) => Number(oprema.stanje) === 1);
  }, [selectedEquipment]);

  useEffect(() => {
    loadHistorija();
  }, []);

  async function loadHistorija() {
    setLoading(true);
    try {
      const response = await api.get("/Rezervacija/moje");
      const pastTermini = (response.data || []).filter((termin) => {
        const end = toDateTime(termin);
        return end && end <= new Date();
      });
      setTermini(pastTermini);
    } catch {
      setMessage({ type: "error", text: "Neuspjesno ucitavanje historije termina." });
    } finally {
      setLoading(false);
    }
  }

  async function otvoriTermin(termin) {
    setSelectedTermin(termin);
    setEquipmentLoading(true);
    try {
      const response = await api.get(`/Oprema/kabinet/${termin.kabinetID}`);
      setSelectedEquipment(response.data || []);
    } catch {
      setSelectedEquipment([]);
      setMessage({ type: "error", text: "Neuspjesno ucitavanje opreme za termin." });
    } finally {
      setEquipmentLoading(false);
    }
  }

  function otvoriPrijavu(oprema, termin) {
    setKomentar("");
    setReportModal({ oprema, termin });
  }

  async function posaljiPrijavu(e) {
    e.preventDefault();
    setSaving(true);

    try {
      const { oprema, termin } = reportModal;
      await api.post("/Evidencija", {
        opremaID: oprema.id,
        terminID: termin.id,
        status: "Kvar",
        komentar,
      });

      setMessage({
        type: "success",
        text: "Kvar uspjesno prijavljen. Tehnicar je obavijesten mailom.",
      });
      setReportModal(null);
      setSelectedTermin(null);
      await loadHistorija();
      window.scrollTo({ top: 0, behavior: "smooth" });
    } catch (error) {
      const apiMessage =
        typeof error.response?.data === "string"
          ? error.response.data
          : error.response?.data?.message;

      setMessage({
        type: "error",
        text: apiMessage || "Greska pri prijavi kvara.",
      });
    } finally {
      setSaving(false);
    }
  }

  const sortedTermini = useMemo(() => {
    return [...termini].sort((left, right) => {
      const leftValue = `${left.datum.split("T")[0]}T${left.vrijemePocetka}`;
      const rightValue = `${right.datum.split("T")[0]}T${right.vrijemePocetka}`;
      return rightValue.localeCompare(leftValue);
    });
  }, [termini]);

  const filtriraniTermini = sortedTermini.filter((t) => {
    const datum = t.datum?.split("T")[0];
    if (filterOd && datum < filterOd) return false;
    if (filterDo && datum > filterDo) return false;
    if (filterKabinet && t.kabinetNaziv !== filterKabinet) return false;
    return true;
  });

  return (
    <Layout>
      <div className="page-header" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
        <div>
          <h1>Historija termina</h1>
          <p>Prikaz zavrsenih termina i prijava kvarova opreme u roku od 24 sata nakon termina.</p>
        </div>
        {sortedTermini.length > 0 && (
          <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
            <button
              className="button warn"
              onClick={() => exportHistorijaCSV(filtriraniTermini)}
            >
              Export CSV
            </button>
            <button
              className="button warn"
              onClick={() => exportHistorijaPDF(filtriraniTermini)}
            >
              Export PDF
            </button>
          </div>
        )}
      </div>

      {message.text && (
        <div className={message.type === "error" ? "form-error" : "form-success"} style={{ marginBottom: 16 }}>
          {message.text}
        </div>
      )}

      <div className="card">
        {sortedTermini.length > 0 && (
          <div style={{ display: "flex", gap: 12, alignItems: "flex-end", flexWrap: "wrap", marginBottom: 16 }}>
            <div className="form-group" style={{ margin: 0 }}>
              <label style={{ fontSize: 13 }}>Od</label>
              <input type="date" value={filterOd} onChange={(e) => setFilterOd(e.target.value)} />
            </div>
            <div className="form-group" style={{ margin: 0 }}>
              <label style={{ fontSize: 13 }}>Do</label>
              <input type="date" value={filterDo} onChange={(e) => setFilterDo(e.target.value)} />
            </div>
            <div className="form-group" style={{ margin: 0 }}>
              <label style={{ fontSize: 13 }}>Kabinet</label>
              <select value={filterKabinet} onChange={(e) => setFilterKabinet(e.target.value)}>
                <option value="">Svi kabineti</option>
                {kabineti.map((k) => <option key={k} value={k}>{k}</option>)}
              </select>
            </div>
            {(filterOd || filterDo || filterKabinet) && (
              <button className="button sekundarno" onClick={resetujFiltere} style={{ marginBottom: 1 }}>
                Resetuj filtere
              </button>
            )}
          </div>
        )}

        {loading ? (
          <div className="users-empty-state">Ucitavanje historije...</div>
        ) : filtriraniTermini.length > 0 ? (
          <div className="users-list">
            {filtriraniTermini.map((termin) => {
              const allowed = canReportFault(termin);
              return (
                <div className="users-list-item termini-list-row" key={termin.id} style={{ cursor: "pointer" }} onClick={() => otvoriTermin(termin)}>
                  <span style={{ fontWeight: 700 }}>{formatDate(termin.datum)}</span>
                  <span>
                    <span className="badge plavo">
                      {formatTime(termin.vrijemePocetka)} - {formatTime(termin.vrijemeKraja)}
                    </span>
                  </span>
                  <span>{termin.kabinetNaziv}</span>
                  <span>{termin.profesorIme || "N/A"}</span>
                  <span>
                    <span className={`badge ${allowed ? "zeleno" : "sivo"}`}>
                      {allowed ? "Kvar se moze prijaviti" : "Rok za prijavu je istekao"}
                    </span>
                  </span>
                </div>
              );
            })}
          </div>
        ) : (
          <div className="users-empty-state">
            {sortedTermini.length > 0 ? "Nema rezultata za odabrane filtere." : "Nema zavrsenih termina za prikaz."}
          </div>
        )}
      </div>

      {selectedTermin && (
        <div className="users-modal-overlay" onClick={() => setSelectedTermin(null)}>
          <div className="users-modal" onClick={(event) => event.stopPropagation()} style={{ maxWidth: 920, width: "min(920px, 100%)", overflowX: "hidden" }}>
            <div className="users-modal-header">
              <h2>Termin {formatDate(selectedTermin.datum)} {formatTime(selectedTermin.vrijemePocetka)} - {formatTime(selectedTermin.vrijemeKraja)}</h2>
              <button className="users-modal-close" onClick={() => setSelectedTermin(null)}>×</button>
            </div>

            <div style={{ marginBottom: 16, color: "var(--text-muted)" }}>
              Kabinet: <strong>{selectedTermin.kabinetNaziv}</strong>
              {role === "profesor" && selectedTermin.profesorIme && (
                <span style={{ marginLeft: 12 }}>Profesor: <strong>{selectedTermin.profesorIme}</strong></span>
              )}
            </div>

            {equipmentLoading ? (
              <div className="users-empty-state">Ucitavanje opreme...</div>
            ) : visibleEquipment.length > 0 ? (
              <div className="users-list" style={{ maxHeight: 360, overflow: "auto" }}>
                {visibleEquipment.map((oprema) => {
                  const allowed = canReportFault(selectedTermin) && Number(oprema.stanje) === 1;
                  return (
                    <div
                      className="users-list-item"
                      key={oprema.id}
                      style={{
                        display: "grid",
                        gridTemplateColumns: "minmax(0, 1.5fr) minmax(0, 0.9fr) minmax(0, 0.7fr) auto",
                        gap: "12px",
                        alignItems: "center",
                        padding: "16px 22px",
                        cursor: "default",
                      }}
                    >
                      <span style={{ fontWeight: 700 }}>{oprema.naziv}</span>
                      <span>
                        <div>{oprema.kategorija || "-"}</div>
                        <small style={{ color: "var(--text-muted)" }}>S/N {oprema.serijskiBroj ?? "-"}</small>
                      </span>
                      <span className="badge zeleno">
                        Ispravno
                      </span>
                      <span>
                        <button className="button" disabled={!allowed} onClick={() => otvoriPrijavu(oprema, selectedTermin)}>
                          Prijavi kvar
                        </button>
                      </span>
                    </div>
                  );
                })}
              </div>
            ) : (
              <div className="users-empty-state">Nema ispravne opreme za ovaj kabinet.</div>
            )}
          </div>
        </div>
      )}

      {reportModal && (
        <div className="users-modal-overlay" onClick={() => setReportModal(null)}>
          <div className="users-modal" onClick={(event) => event.stopPropagation()}>
            <div className="users-modal-header">
              <h2>Prijava kvara opreme</h2>
              <button className="users-modal-close" onClick={() => setReportModal(null)}>×</button>
            </div>

            <div style={{ marginBottom: 16, color: "var(--text-muted)" }}>
              <div><strong>Profesor:</strong> {selectedTermin?.profesorIme || "Autorizovani profesor"}</div>
              <div><strong>Termin:</strong> {formatDate(reportModal.termin.datum)} {formatTime(reportModal.termin.vrijemePocetka)} - {formatTime(reportModal.termin.vrijemeKraja)}</div>
              <div><strong>Oprema:</strong> {reportModal.oprema.naziv}</div>
            </div>

            <div className="form-grid" style={{ marginBottom: 16 }}>
              <div className="form-group">
                <label>Profesor</label>
                <input
                  type="text"
                  value={selectedTermin?.profesorIme || "Autorizovani profesor"}
                  disabled
                  style={{ width: "100%" }}
                />
              </div>
              <div className="form-group">
                <label>Datum termina</label>
                <input
                  type="text"
                  value={formatDate(reportModal.termin.datum)}
                  disabled
                  style={{ width: "100%" }}
                />
              </div>
              <div className="form-group">
                <label>Vrijeme termina</label>
                <input
                  type="text"
                  value={`${formatTime(reportModal.termin.vrijemePocetka)} - ${formatTime(reportModal.termin.vrijemeKraja)}`}
                  disabled
                  style={{ width: "100%" }}
                />
              </div>
            </div>

            <form onSubmit={posaljiPrijavu}>
              <div className="form-group">
                <label>Komentar kvara</label>
                <textarea
                  value={komentar}
                  onChange={(event) => setKomentar(event.target.value)}
                  rows={4}
                  maxLength={500}
                  required
                  placeholder="Opis kvara, npr. projektor ne prikazuje sliku..."
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
              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : "Posalji prijavu"}
                </button>
                <button className="button sekundarno" type="button" onClick={() => setReportModal(null)}>
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

export default Historija;