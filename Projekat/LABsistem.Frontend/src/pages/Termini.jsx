import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentUserId } from "../auth/session";

const INITIAL_FORM_STATE = {
  datum: "",
  vrijemePocetka: "",
  vrijemeKraja: "",
  kabinetID: "",
};

const INITIAL_FILTERS = {
  searchTerm: "",
  datum: "",
  kabinet: "",
};

function extractErrorMessage(error, fallbackMessage) {
  const responseData = error?.response?.data;
  if (typeof responseData === "string" && responseData.trim()) return responseData;
  if (typeof responseData?.message === "string" && responseData.message.trim()) return responseData.message;
  return fallbackMessage;
}

function formatDateForInput(value) {
  if (!value) return "";
  return value.split("T")[0];
}

function formatDateForDisplay(value) {
  const inputDate = formatDateForInput(value);
  if (!inputDate) return "N/A";

  const [year, month, day] = inputDate.split("-");
  return `${day}.${month}.${year}.`;
}

function formatTimeForInput(value) {
  if (!value) return "";
  return value.slice(0, 5);
}

function formatTimeForPayload(value) {
  if (!value) return "";
  return value.length === 5 ? `${value}:00` : value;
}

function isPastTermin(formState) {
  if (!formState.datum || !formState.vrijemePocetka) return false;
  const selectedDateTime = new Date(`${formState.datum}T${formState.vrijemePocetka}:00`);
  return selectedDateTime.getTime() < Date.now();
}

function Termini() {
  const [termini, setTermini] = useState([]);
  const [filters, setFilters] = useState(INITIAL_FILTERS);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState("create");
  const [editingTermin, setEditingTermin] = useState(null);
  const [saving, setSaving] = useState(false);
  const [formState, setFormState] = useState(INITIAL_FORM_STATE);
  const [message, setMessage] = useState({ type: "", text: "" });

  const currentUserId = getCurrentUserId();

  useEffect(() => {
    loadTermini();
  }, []);

  async function loadTermini() {
    setLoading(true);
    try {
      const response = await api.get("/Termin");
      setTermini(response.data);
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Neuspjesno ucitavanje termina."),
      });
    } finally {
      setLoading(false);
    }
  }

  const kabinetOptions = useMemo(() => {
    const unique = new Map();
    termini.forEach((termin) => {
      if (termin.kabinetID && termin.kabinetNaziv) {
        unique.set(termin.kabinetID, termin.kabinetNaziv);
      }
    });

    return [...unique.entries()].map(([id, naziv]) => ({ id, naziv }));
  }, [termini]);

  const filteredTermini = useMemo(() => {
    return termini.filter((termin) => {
      const datum = formatDateForInput(termin.datum);
      const search = filters.searchTerm.toLowerCase().trim();

      const searchMatch =
        !search ||
        termin.kabinetNaziv?.toLowerCase().includes(search) ||
        termin.kreatorIme?.toLowerCase().includes(search) ||
        datum.includes(search);

      const dateMatch = !filters.datum || datum === filters.datum;
      const kabinetMatch = !filters.kabinet || termin.kabinetID === Number(filters.kabinet);

      return searchMatch && dateMatch && kabinetMatch;
    });
  }, [termini, filters]);

  const sortedTermini = useMemo(() => {
    return [...filteredTermini].sort((a, b) => {
      const first = `${formatDateForInput(a.datum)}T${formatTimeForInput(a.vrijemePocetka)}`;
      const second = `${formatDateForInput(b.datum)}T${formatTimeForInput(b.vrijemePocetka)}`;
      return first.localeCompare(second);
    });
  }, [filteredTermini]);

  const activeFilterCount = Object.values(filters).filter(Boolean).length;

  function handleFilterChange(e) {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  }

  function resetFilters() {
    setFilters(INITIAL_FILTERS);
  }

  function handleFormChange(e) {
    const { name, value } = e.target;
    setFormState((prev) => ({ ...prev, [name]: value }));
  }

  function openCreateModal() {
    setModalMode("create");
    setEditingTermin(null);
    setFormState(INITIAL_FORM_STATE);
    setMessage({ type: "", text: "" });
    setModalOpen(true);
  }

  function openEditModal(termin) {
    setModalMode("edit");
    setEditingTermin(termin);
    setFormState({
      datum: formatDateForInput(termin.datum),
      vrijemePocetka: formatTimeForInput(termin.vrijemePocetka),
      vrijemeKraja: formatTimeForInput(termin.vrijemeKraja),
      kabinetID: termin.kabinetID?.toString() || "",
    });
    setMessage({ type: "", text: "" });
    setModalOpen(true);
  }

  function validateForm() {
    if (!formState.datum || !formState.vrijemePocetka || !formState.vrijemeKraja || !formState.kabinetID) {
      return "Sva polja su obavezna.";
    }

    if (formState.vrijemeKraja <= formState.vrijemePocetka) {
      return "Vrijeme kraja mora biti nakon vremena pocetka.";
    }

    if (isPastTermin(formState)) {
      return "Termin ne moze biti u proslosti.";
    }

    if (!currentUserId) {
      return "Nije pronadjen prijavljeni korisnik.";
    }

    return "";
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setSaving(true);
    setMessage({ type: "", text: "" });

    const validationError = validateForm();
    if (validationError) {
      setMessage({ type: "error", text: validationError });
      setSaving(false);
      return;
    }

    const payload = {
      datum: `${formState.datum}T00:00:00.000Z`,
      vrijemePocetka: formatTimeForPayload(formState.vrijemePocetka),
      vrijemeKraja: formatTimeForPayload(formState.vrijemeKraja),
      kreatorID: Number(currentUserId),
      kabinetID: Number(formState.kabinetID),
    };

    try {
      if (modalMode === "create") {
        await api.post("/Termin", payload);
        setMessage({ type: "success", text: "Termin uspjesno dodan." });
      } else {
        await api.put(`/Termin/${editingTermin.id}`, payload);
        setMessage({ type: "success", text: "Termin uspjesno azuriran." });
      }

      await loadTermini();
      setTimeout(() => setModalOpen(false), 700);
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Greska prilikom cuvanja termina."),
      });
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(termin) {
    const potvrda = window.confirm(
      `Da li ste sigurni da zelite obrisati termin ${formatDateForDisplay(termin.datum)} u ${formatTimeForInput(termin.vrijemePocetka)}?`
    );

    if (!potvrda) return;

    try {
      await api.delete(`/Termin/${termin.id}`);
      await loadTermini();
      setMessage({ type: "success", text: "Termin obrisan." });
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Greska pri brisanju termina."),
      });
    }
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje terminima</h1>
        <p>Definisanje, izmjena i brisanje termina za kabinete.</p>
      </div>

      <div className="users-page">
        <div className="card users-toolbar" style={{ flexWrap: "wrap", gap: "10px", alignItems: "center" }}>
          <button className="button users-create-button" onClick={openCreateModal}>
            <span className="users-create-icon">+</span> Dodaj termin
          </button>

          <div style={{ position: "relative", flex: "1", minWidth: "220px" }}>
            <input
              type="text"
              name="searchTerm"
              placeholder="Pretrazi po kabinetu, kreatoru ili datumu..."
              value={filters.searchTerm}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px",
                border: "1px solid #e2e8f0",
                borderRadius: "8px",
                color: "#1f2937",
                fontSize: "14px",
              }}
            />
          </div>

          <div style={{ minWidth: "170px" }}>
            <input
              type="date"
              name="datum"
              value={filters.datum}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px",
                border: "1px solid #e2e8f0",
                borderRadius: "8px",
                color: filters.datum ? "#1f2937" : "#64748b",
                fontSize: "14px",
              }}
            />
          </div>

          <div style={{ position: "relative", minWidth: "170px" }}>
            <select
              name="kabinet"
              value={filters.kabinet}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px",
                border: "1px solid #e2e8f0",
                borderRadius: "8px",
                background: "white",
                color: filters.kabinet ? "#1f2937" : "#64748b",
                fontSize: "14px",
              }}
            >
              <option value="">Svi kabineti</option>
              {kabinetOptions.map((kabinet) => (
                <option key={kabinet.id} value={kabinet.id}>
                  {kabinet.naziv}
                </option>
              ))}
            </select>
          </div>

          {activeFilterCount > 0 && (
            <button className="button sekundarno" onClick={resetFilters} style={{ whiteSpace: "nowrap" }}>
              Resetuj ({activeFilterCount})
            </button>
          )}
        </div>

        <div className="card users-list-card">
          {message.text && !modalOpen && (
            <p className={message.type === "error" ? "form-error" : "form-success"} style={{ margin: "16px 22px 0" }}>
              {message.text}
            </p>
          )}

          <div className="termini-list-header termini-list-row">
            <span>Datum</span>
            <span>Vrijeme</span>
            <span>Kabinet</span>
            <span>Kreator</span>
            <span>Akcije</span>
          </div>

          {loading ? (
            <div className="users-empty-state">Ucitavanje termina...</div>
          ) : sortedTermini.length > 0 ? (
            <div className="users-list">
              {sortedTermini.map((termin) => (
                <div className="termini-list-row users-list-item" key={termin.id}>
                  <span style={{ fontWeight: 700 }}>{formatDateForDisplay(termin.datum)}</span>
                  <span>
                    <span className="badge plavo">
                      {formatTimeForInput(termin.vrijemePocetka)} - {formatTimeForInput(termin.vrijemeKraja)}
                    </span>
                  </span>
                  <span>{termin.kabinetNaziv || `Kabinet #${termin.kabinetID}`}</span>
                  <span>{termin.kreatorIme || `Korisnik #${termin.kreatorID}`}</span>
                  <span>
                    <div className="users-actions">
                      <button className="users-action-btn" onClick={() => openEditModal(termin)}>
                        Uredi
                      </button>
                      <button className="users-action-btn warn" onClick={() => handleDelete(termin)}>
                        Brisi
                      </button>
                    </div>
                  </span>
                </div>
              ))}
            </div>
          ) : (
            <div className="users-empty-state">
              Nema pronadjenih termina.
              {activeFilterCount > 0 && (
                <button className="button sekundarno" onClick={resetFilters} style={{ marginLeft: "10px" }}>
                  Resetuj filtere
                </button>
              )}
            </div>
          )}
        </div>
      </div>

      {modalOpen && (
        <div className="users-modal-overlay" onClick={() => setModalOpen(false)}>
          <div className="users-modal" onClick={(event) => event.stopPropagation()}>
            <div className="users-modal-header">
              <div>
                <h2>{modalMode === "create" ? "Novi termin" : "Uredi termin"}</h2>
                <p>Unesite datum, vrijeme i kabinet za termin.</p>
              </div>
              <button className="users-modal-close" onClick={() => setModalOpen(false)}>
                x
              </button>
            </div>

            {message.text && (
              <p className={message.type === "error" ? "form-error" : "form-success"}>{message.text}</p>
            )}

            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Datum</label>
                <input name="datum" type="date" value={formState.datum} onChange={handleFormChange} required />
              </div>

              <div className="form-group">
                <label>Vrijeme pocetka</label>
                <input
                  name="vrijemePocetka"
                  type="time"
                  value={formState.vrijemePocetka}
                  onChange={handleFormChange}
                  required
                />
              </div>

              <div className="form-group">
                <label>Vrijeme kraja</label>
                <input
                  name="vrijemeKraja"
                  type="time"
                  value={formState.vrijemeKraja}
                  onChange={handleFormChange}
                  required
                />
              </div>

              <div className="form-group">
                <label>ID kabineta</label>
                <input
                  name="kabinetID"
                  type="number"
                  min="1"
                  value={formState.kabinetID}
                  onChange={handleFormChange}
                  required
                />
                <div className="users-field-hint">
                  Postojeci kabineti: {kabinetOptions.length > 0 ? kabinetOptions.map((k) => `${k.id} - ${k.naziv}`).join(", ") : "nema ucitanih kabineta"}
                </div>
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : "Sacuvaj"}
                </button>
                <button className="button sekundarno" type="button" onClick={() => setModalOpen(false)}>
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

export default Termini;
