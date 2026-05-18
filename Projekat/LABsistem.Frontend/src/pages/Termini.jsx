import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentUserId } from "../auth/session";
import { getCurrentRole } from "../auth/routeAccess";

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
  if (typeof responseData === "string" && responseData.trim())
    return responseData;
  if (typeof responseData?.message === "string" && responseData.message.trim())
    return responseData.message;
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
  const selectedDateTime = new Date(
    `${formState.datum}T${formState.vrijemePocetka}:00`,
  );
  return selectedDateTime.getTime() < Date.now();
}

function Termini() {
  const [termini, setTermini] = useState([]);
  const [kabineti, setKabineti] = useState([]);
  const [filters, setFilters] = useState(INITIAL_FILTERS);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState("create");
  const [editingTermin, setEditingTermin] = useState(null);
  const [saving, setSaving] = useState(false);
  const [formState, setFormState] = useState(INITIAL_FORM_STATE);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [rezervacijaModalOpen, setRezervacijaModalOpen] = useState(false);
  const [rezervacijaForm, setRezervacijaForm] = useState({
    limitOsoba: 20,
    vidljivoStudentima: true,
  });
  const [selectedTerminId, setSelectedTerminId] = useState(null);
  const [equipmentModalOpen, setEquipmentModalOpen] = useState(false);
  const [selectedCabinetEquipment, setSelectedCabinetEquipment] = useState([]);
  const [selectedCabinetName, setSelectedCabinetName] = useState("");
  const [loadingEquipment, setLoadingEquipment] = useState(false);

  const currentUserId = getCurrentUserId();
  const currentRole = getCurrentRole();
  const canManageTermini =
    currentRole === "tehnicar" || currentRole === "admin";

  useEffect(() => {
    loadTermini();
  }, []);

  async function loadTermini() {
    setLoading(true);
    try {
      const [terminiResponse, kabinetiResponse] = await Promise.all([
        api.get("/Termin"),
        api.get("/Kabinet"),
      ]);
      setTermini(terminiResponse.data);
      setKabineti(kabinetiResponse.data);
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Neuspjesno ucitavanje termina."),
      });
    } finally {
      setLoading(false);
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

  const kabinetOptions = useMemo(() => {
    return kabineti.map((kabinet) => ({
      id: kabinet.id,
      naziv: kabinet.naziv,
      objekatLokacija: kabinet.objekatLokacija,
      kapacitet: kabinet.kapacitet,
    }));
  }, [kabineti]);

  const filteredTermini = useMemo(() => {
    return termini.filter((termin) => {
      // Sakrij prosle termine
      const terminEnd = new Date(
        `${termin.datum.split("T")[0]}T${termin.vrijemeKraja}`,
      );
      if (terminEnd < new Date()) return false;

      const datum = formatDateForInput(termin.datum);
      const search = filters.searchTerm.toLowerCase().trim();

      const searchMatch =
        !search ||
        termin.kabinetNaziv?.toLowerCase().includes(search) ||
        termin.kreatorIme?.toLowerCase().includes(search) ||
        datum.includes(search);

      const dateMatch = !filters.datum || datum === filters.datum;
      const kabinetMatch =
        !filters.kabinet || termin.kabinetID === Number(filters.kabinet);

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
    if (
      !formState.datum ||
      !formState.vrijemePocetka ||
      !formState.vrijemeKraja ||
      !formState.kabinetID
    ) {
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
      `Da li ste sigurni da zelite obrisati termin ${formatDateForDisplay(termin.datum)} u ${formatTimeForInput(termin.vrijemePocetka)}?`,
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

  async function handleRezervisi(e) {
    e.preventDefault();
    setSaving(true);
    setMessage({ type: "", text: "" });

    if (!rezervacijaForm.maxKapacitet || rezervacijaForm.maxKapacitet < 1) {
      setMessage({
        type: "error",
        text: "Kapacitet kabineta nije ispravno ucitan.",
      });
      setSaving(false);
      return;
    }

    if (
      rezervacijaForm.limitOsoba < 1 ||
      rezervacijaForm.limitOsoba > rezervacijaForm.maxKapacitet
    ) {
      setMessage({
        type: "error",
        text: `Limit osoba mora biti izmedju 1 i ${rezervacijaForm.maxKapacitet}.`,
      });
      setSaving(false);
      return;
    }

    try {
      await api.post(
        `/Rezervacija/rezervisi/${selectedTerminId}`,
        rezervacijaForm,
      );
      setMessage({ type: "success", text: "Termin uspjesno rezervisan." });
      setRezervacijaModalOpen(false);
      loadTermini();
    } catch (error) {
      setMessage({
        type: "error",
        text: error.response?.data || "Greska pri rezervaciji.",
      });
    } finally {
      setSaving(false);
    }
  }

  function openRezervacijaModal(id) {
    const termin = termini.find((t) => t.id === id);
    const kabinet = kabineti.find((k) => k.id === termin?.kabinetID);

    if (!kabinet || !kabinet.kapacitet || kabinet.kapacitet < 1) {
      setMessage({
        type: "error",
        text: "Kapacitet odabranog kabineta nije dostupan. Provjerite kabinet prije rezervacije.",
      });
      return;
    }

    setSelectedTerminId(id);
    setRezervacijaForm({
      limitOsoba: kabinet.kapacitet,
      vidljivoStudentima: true,
      maxKapacitet: kabinet.kapacitet,
    });
    setRezervacijaModalOpen(true);
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje terminima</h1>
        <p>
          {canManageTermini
            ? "Definisanje, izmjena i brisanje termina za kabinete."
            : "Pregled termina po datumima i kabinetima."}
        </p>
      </div>

      <div className="users-page">
        <div
          className="card users-toolbar"
          style={{ flexWrap: "wrap", gap: "10px", alignItems: "center" }}
        >
          {canManageTermini && (
            <button
              className="button users-create-button"
              onClick={openCreateModal}
            >
              <span className="users-create-icon">+</span> Dodaj termin
            </button>
          )}

          <div style={{ position: "relative", flex: "1", minWidth: "220px" }}>
            <span
              style={{
                position: "absolute",
                left: "10px",
                top: "50%",
                transform: "translateY(-50%)",
                color: "var(--text-muted)",
                fontSize: "14px",
                pointerEvents: "none",
              }}
            >
              🔍
            </span>
            <input
              type="text"
              name="searchTerm"
              placeholder="Pretrazi po kabinetu, kreatoru ili datumu..."
              value={filters.searchTerm}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px 8px 32px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: "var(--text)",
                fontSize: "14px",
                boxSizing: "border-box",
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
            <span
              style={{
                position: "absolute",
                left: "10px",
                top: "50%",
                transform: "translateY(-50%)",
                pointerEvents: "none",
                fontSize: "13px",
                color: "var(--text-muted)",
              }}
            >
              🚪
            </span>
            <select
              name="kabinet"
              value={filters.kabinet}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px 8px 30px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: filters.kabinet ? "var(--text)" : "var(--text-muted)",
                fontSize: "14px",
                appearance: "none",
                cursor: "pointer",
              }}
            >
              <option value="">Svi kabineti</option>
              {kabinetOptions.map((kabinet) => (
                <option key={kabinet.id} value={kabinet.id}>
                  {kabinet.naziv}
                  {kabinet.objekatLokacija
                    ? ` (${kabinet.objekatLokacija})`
                    : ""}
                </option>
              ))}
            </select>
            <span
              style={{
                position: "absolute",
                right: "10px",
                top: "50%",
                transform: "translateY(-50%)",
                pointerEvents: "none",
                fontSize: "11px",
                color: "var(--text-muted)",
              }}
            >
              ▼
            </span>
          </div>

          {activeFilterCount > 0 && (
            <button
              className="button sekundarno"
              onClick={resetFilters}
              style={{ whiteSpace: "nowrap" }}
            >
              Resetuj ({activeFilterCount})
            </button>
          )}
        </div>

        <div className="card users-list-card">
          {message.text && !modalOpen && (
            <p
              className={
                message.type === "error" ? "form-error" : "form-success"
              }
              style={{ margin: "16px 22px 0" }}
            >
              {message.text}
            </p>
          )}

          <div className="termini-list-header termini-list-row">
            <span>Datum</span>
            <span>Vrijeme</span>
            <span>Kabinet</span>
            <span>Kreator</span>
            <span>Status</span>
            {(canManageTermini || currentRole === "profesor") && (
              <span>Akcije</span>
            )}
          </div>

          {loading ? (
            <div className="users-empty-state">Ucitavanje termina...</div>
          ) : sortedTermini.length > 0 ? (
            <div className="users-list">
              {sortedTermini.map((termin) => (
                <div
                  className="termini-list-row users-list-item"
                  key={termin.id}
                >
                  <span style={{ fontWeight: 700 }}>
                    {formatDateForDisplay(termin.datum)}
                  </span>
                  <span>
                    <span className="badge plavo">
                      {formatTimeForInput(termin.vrijemePocetka)} -{" "}
                      {formatTimeForInput(termin.vrijemeKraja)}
                    </span>
                  </span>
                  <span>
                    <button
                      className="text-button"
                      onClick={() =>
                        loadEquipment(termin.kabinetID, termin.kabinetNaziv)
                      }
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
                      {termin.kabinetNaziv || `Kabinet #${termin.kabinetID}`}
                    </button>
                  </span>
                  <span>
                    {termin.kreatorIme || `Korisnik #${termin.kreatorID}`}
                  </span>
                  <span>
                    <span
                      className={`badge ${termin.statusTermina === "Slobodan" ? "zeleno" : "plavo"}`}
                    >
                      {termin.statusTermina}
                    </span>
                  </span>
                  {(canManageTermini ||
                    (currentRole === "profesor" &&
                      termin.statusTermina === "Slobodan")) && (
                    <span>
                      <div className="users-actions">
                        {canManageTermini && (
                          <>
                            <button
                              className="users-action-btn"
                              onClick={() => openEditModal(termin)}
                            >
                              Uredi
                            </button>
                            <button
                              className="users-action-btn warn"
                              onClick={() => handleDelete(termin)}
                            >
                              Brisi
                            </button>
                          </>
                        )}
                        {currentRole === "profesor" &&
                          termin.statusTermina === "Slobodan" && (
                            <button
                              className="button"
                              style={{ padding: "4px 12px" }}
                              onClick={() => openRezervacijaModal(termin.id)}
                            >
                              Rezervisi
                            </button>
                          )}
                      </div>
                    </span>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <div className="users-empty-state">
              Nema pronadjenih termina.
              {activeFilterCount > 0 && (
                <button
                  className="button sekundarno"
                  onClick={resetFilters}
                  style={{ marginLeft: "10px" }}
                >
                  Resetuj filtere
                </button>
              )}
            </div>
          )}
        </div>
      </div>

      {modalOpen && canManageTermini && (
        <div
          className="users-modal-overlay"
          onClick={() => setModalOpen(false)}
        >
          <div
            className="users-modal"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="users-modal-header">
              <div>
                <h2>
                  {modalMode === "create" ? "Novi termin" : "Uredi termin"}
                </h2>
                <p>Unesite datum, vrijeme i kabinet za termin.</p>
              </div>
              <button
                className="users-modal-close"
                onClick={() => setModalOpen(false)}
              >
                x
              </button>
            </div>

            {message.text && (
              <p
                className={
                  message.type === "error" ? "form-error" : "form-success"
                }
              >
                {message.text}
              </p>
            )}

            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Datum</label>
                <input
                  name="datum"
                  type="date"
                  value={formState.datum}
                  onChange={handleFormChange}
                  required
                />
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
                <label>Kabinet</label>
                <select
                  name="kabinetID"
                  value={formState.kabinetID}
                  onChange={handleFormChange}
                  required
                >
                  <option value="">Odaberi kabinet</option>
                  {kabinetOptions.map((kabinet) => (
                    <option key={kabinet.id} value={kabinet.id}>
                      {kabinet.naziv}
                      {kabinet.objekatLokacija
                        ? ` (${kabinet.objekatLokacija})`
                        : ""}
                    </option>
                  ))}
                </select>
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : "Sacuvaj"}
                </button>
                <button
                  className="button sekundarno"
                  type="button"
                  onClick={() => setModalOpen(false)}
                >
                  Odustani
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {rezervacijaModalOpen && (
        <div
          className="users-modal-overlay"
          onClick={() => setRezervacijaModalOpen(false)}
        >
          <div className="users-modal" onClick={(e) => e.stopPropagation()}>
            <div className="users-modal-header">
              <h2>Rezervacija termina</h2>
              <button
                className="users-modal-close"
                onClick={() => setRezervacijaModalOpen(false)}
              >
                x
              </button>
            </div>
            <form onSubmit={handleRezervisi}>
              <div className="form-group">
                <label>
                  Limit osoba (Maksimalno: {rezervacijaForm.maxKapacitet})
                </label>
                <input
                  type="number"
                  value={rezervacijaForm.limitOsoba}
                  onChange={(e) =>
                    setRezervacijaForm({
                      ...rezervacijaForm,
                      limitOsoba: Number(e.target.value),
                    })
                  }
                  min="1"
                  max={rezervacijaForm.maxKapacitet}
                  required
                />
                {rezervacijaForm.limitOsoba > rezervacijaForm.maxKapacitet && (
                  <p
                    className="form-error"
                    style={{ fontSize: "12px", marginTop: "4px" }}
                  >
                    Limit ne može biti veći od kapaciteta kabineta (
                    {rezervacijaForm.maxKapacitet}).
                  </p>
                )}
              </div>
              <div className="form-group" style={{ margin: "15px 0" }}>
                <label
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: "10px",
                    cursor: "pointer",
                    fontWeight: "500",
                  }}
                >
                  <input
                    type="checkbox"
                    checked={rezervacijaForm.vidljivoStudentima}
                    onChange={(e) =>
                      setRezervacijaForm({
                        ...rezervacijaForm,
                        vidljivoStudentima: e.target.checked,
                      })
                    }
                    style={{ width: "20px", height: "20px", margin: 0 }}
                  />
                  <span>Vidljivo studentima</span>
                </label>
              </div>
              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : "Potvrdi rezervaciju"}
                </button>
                <button
                  className="button sekundarno"
                  type="button"
                  onClick={() => setRezervacijaModalOpen(false)}
                >
                  Odustani
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

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

export default Termini;
