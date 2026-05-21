import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentUserId } from "../auth/session";

const getLocalRole = () => localStorage.getItem("uloga") || "student";

const STATUS_OPTIONS = [
  { value: 1, label: "Ispravno", color: "zeleno" },
  { value: 2, label: "U kvaru", color: "crveno" },
  { value: 3, label: "Na servisu", color: "zuto" },
  { value: 4, label: "Otpisano", color: "sivo" },
];

const ARHIVA_OPTIONS = [
  { value: "aktivna", label: "Aktivna oprema" },
  { value: "arhivirana", label: "Arhivirana oprema" },
  { value: "sve", label: "Sve stavke" },
];

const INITIAL_FORM_STATE = {
  naziv: "",
  kategorija: "",
  stanje: 1,
  kabinetID: "",
};

const INITIAL_FILTERS = {
  searchTerm: "",
  status: "",
  kategorija: "",
  kabinet: "",
  zgrada: "",
  arhiva: "aktivna",
};

function extractErrorMessage(error, fallbackMessage) {
  const responseData = error?.response?.data;
  if (typeof responseData === "string" && responseData.trim()) return responseData;
  if (typeof responseData?.message === "string" && responseData.message.trim()) return responseData.message;
  return fallbackMessage;
}

function Oprema() {
  const [opremaList, setOpremaList] = useState([]);
  const [objekti, setObjekti] = useState([]);
  const [selectedObjekatID, setSelectedObjekatID] = useState("");
  const [filters, setFilters] = useState(INITIAL_FILTERS);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState("create");
  const [editingOprema, setEditingOprema] = useState(null);
  const [saving, setSaving] = useState(false);
  const [formState, setFormState] = useState(INITIAL_FORM_STATE);
  const [message, setMessage] = useState({ type: "", text: "" });

  const userRole = getLocalRole();
  const currentUserId = getCurrentUserId();
  const isAdminOrTehnicar = userRole === "tehnicar" || userRole === "admin";

  useEffect(() => {
    loadAll();
  }, [filters.arhiva]);

  async function loadAll() {
    setLoading(true);
    try {
      const prikaz = isAdminOrTehnicar ? filters.arhiva : "aktivna";
      const [opremaRes, objektiRes] = await Promise.all([
        api.get(`/Oprema?prikaz=${encodeURIComponent(prikaz)}`),
        api.get("/Objekat"),
      ]);

      setOpremaList(opremaRes.data);
      setObjekti(objektiRes.data);
    } catch (error) {
      setMessage({ type: "error", text: extractErrorMessage(error, "Neuspješno učitavanje opreme.") });
    } finally {
      setLoading(false);
    }
  }

  const kabinetiZaObjekat = useMemo(() => {
    if (!selectedObjekatID) return [];
    return objekti.find((objekat) => objekat.id === Number(selectedObjekatID))?.kabineti || [];
  }, [selectedObjekatID, objekti]);

  const kabinetiOptions = useMemo(() => {
    const unique = [...new Map(opremaList.map((item) => [item.kabinetNaziv, item.kabinetNaziv])).values()];
    return unique.filter(Boolean);
  }, [opremaList]);

  const zgradaOptions = useMemo(() => {
    const unique = [...new Map(opremaList.map((item) => [item.zgradaNaziv, item.zgradaNaziv])).values()];
    return unique.filter(Boolean);
  }, [opremaList]);

  const kategorijeOptions = useMemo(() => {
    const unique = [...new Map(opremaList.map((item) => [item.kategorija, item.kategorija])).values()];
    return unique.filter(Boolean).sort((left, right) => left.localeCompare(right));
  }, [opremaList]);

  const filteredOprema = useMemo(() => {
    return opremaList.filter((item) => {
      const trimmedSearchTerm = filters.searchTerm.toLowerCase().trim();
      const termMatch =
        !trimmedSearchTerm ||
        item.naziv.toLowerCase().includes(trimmedSearchTerm) ||
        (item.kategorija || "").toLowerCase().includes(trimmedSearchTerm) ||
        item.serijskiBroj.toString().includes(filters.searchTerm.trim());

      const statusMatch = !filters.status || item.stanje === Number(filters.status);
      const kategorijaMatch = !filters.kategorija || item.kategorija === filters.kategorija;
      const kabinetMatch = !filters.kabinet || item.kabinetNaziv === filters.kabinet;
      const zgradaMatch = !filters.zgrada || item.zgradaNaziv === filters.zgrada;

      return termMatch && statusMatch && kategorijaMatch && kabinetMatch && zgradaMatch;
    });
  }, [opremaList, filters]);

  const visibleOprema = useMemo(() => {
    if (userRole === "profesor") {
      return filteredOprema.filter((item) => Number(item.stanje) !== 3);
    }

    return filteredOprema;
  }, [filteredOprema, userRole]);

  const activeFilterCount = [
    filters.searchTerm,
    filters.status,
    filters.kategorija,
    filters.kabinet,
    filters.zgrada,
    isAdminOrTehnicar && filters.arhiva !== INITIAL_FILTERS.arhiva ? filters.arhiva : "",
  ].filter(Boolean).length;

  function handleFilterChange(event) {
    const { name, value } = event.target;
    setFilters((previous) => ({ ...previous, [name]: value }));
  }

  function resetFilters() {
    setFilters(INITIAL_FILTERS);
  }

  function handleFormChange(event) {
    const { name, value } = event.target;
    const processedValue = name === "stanje" || name === "kabinetID" ? Number(value) : value;

    setFormState((previous) => ({ ...previous, [name]: processedValue }));
  }

  function openCreateModal() {
    setModalMode("create");
    setEditingOprema(null);
    setFormState(INITIAL_FORM_STATE);
    setSelectedObjekatID("");
    setMessage({ type: "", text: "" });
    setModalOpen(true);
  }

  function openEditModal(oprema) {
    setModalMode("edit");
    setEditingOprema(oprema);
    setFormState({
      naziv: oprema.naziv,
      kategorija: oprema.kategorija || "",
      stanje: oprema.stanje,
      kabinetID: oprema.kabinetID,
    });

    const objekat = objekti.find((item) => item.kabineti?.some((kabinet) => kabinet.id === oprema.kabinetID));
    setSelectedObjekatID(objekat ? String(objekat.id) : "");
    setMessage({ type: "", text: "" });
    setModalOpen(true);
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setSaving(true);
    setMessage({ type: "", text: "" });

    try {
      if (modalMode === "create") {
        await api.post("/Oprema", {
          ...formState,
          kreatorID: Number(currentUserId),
        });
        setMessage({ type: "success", text: "Oprema je uspješno dodana." });
      } else {
        await api.put(`/Oprema/${editingOprema.id}`, {
          ...formState,
          id: editingOprema.id,
          kreatorID: editingOprema.kreatorID,
        });
        setMessage({ type: "success", text: "Izmjene su uspješno sačuvane." });
      }

      await loadAll();
      setTimeout(() => setModalOpen(false), 800);
    } catch (error) {
      setMessage({ type: "error", text: extractErrorMessage(error, "Greška prilikom čuvanja podataka.") });
    } finally {
      setSaving(false);
    }
  }

  async function handleArchive(id) {
    if (!window.confirm("Da li ste sigurni da želite arhivirati ovu opremu?")) return;

    try {
      await api.delete(`/Oprema/${id}`);
      await loadAll();
      setMessage({ type: "success", text: "Oprema je uspješno arhivirana." });
    } catch (error) {
      setMessage({ type: "error", text: extractErrorMessage(error, "Greška pri arhiviranju opreme.") });
    }
  }

  async function handleRestore(id) {
    if (!window.confirm("Da li ste sigurni da želite vratiti ovu opremu iz arhive?")) return;

    try {
      await api.post(`/Oprema/${id}/restore`);
      await loadAll();
      setMessage({ type: "success", text: "Oprema je vraćena iz arhive." });
    } catch (error) {
      setMessage({ type: "error", text: extractErrorMessage(error, "Greška pri vraćanju opreme iz arhive.") });
    }
  }

  function getStatusInfo(value) {
    return STATUS_OPTIONS.find((status) => status.value === Number(value)) || STATUS_OPTIONS[0];
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje opremom</h1>
        <p>Pregledajte aktivnu i arhiviranu opremu, te upravljajte njenim statusima i lokacijama.</p>
      </div>

      <div className="users-page">
        <div className="card users-toolbar" style={{ flexWrap: "wrap", gap: "10px", alignItems: "center" }}>
          {isAdminOrTehnicar && (
            <button className="button users-create-button" onClick={openCreateModal}>
              <span className="users-create-icon">+</span> Dodaj opremu
            </button>
          )}

          <div style={{ position: "relative", flex: "1", minWidth: "220px" }}>
            <input
              type="text"
              name="searchTerm"
              placeholder="Pretraži po nazivu, kategoriji ili serijskom broju"
              value={filters.searchTerm}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: "var(--text)",
                fontSize: "14px",
                boxSizing: "border-box",
              }}
            />
          </div>

          <select
            name="status"
            value={filters.status}
            onChange={handleFilterChange}
            style={{
              minWidth: "160px",
              padding: "8px 12px",
              border: "1px solid var(--border)",
              borderRadius: "8px",
              background: "var(--input-bg)",
              color: filters.status ? "var(--text)" : "var(--text-muted)",
              fontSize: "14px",
            }}
          >
            <option value="">Svi statusi</option>
            {STATUS_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>

          <select
            name="kabinet"
            value={filters.kabinet}
            onChange={handleFilterChange}
            style={{
              minWidth: "160px",
              padding: "8px 12px",
              border: "1px solid var(--border)",
              borderRadius: "8px",
              background: "var(--input-bg)",
              color: filters.kabinet ? "var(--text)" : "var(--text-muted)",
              fontSize: "14px",
            }}
          >
            <option value="">Svi kabineti</option>
            {kabinetiOptions.map((kabinet) => (
              <option key={kabinet} value={kabinet}>
                {kabinet}
              </option>
            ))}
          </select>

          <select
            name="zgrada"
            value={filters.zgrada}
            onChange={handleFilterChange}
            style={{
              minWidth: "160px",
              padding: "8px 12px",
              border: "1px solid var(--border)",
              borderRadius: "8px",
              background: "var(--input-bg)",
              color: filters.zgrada ? "var(--text)" : "var(--text-muted)",
              fontSize: "14px",
            }}
          >
            <option value="">Sve zgrade</option>
            {zgradaOptions.map((zgrada) => (
              <option key={zgrada} value={zgrada}>
                {zgrada}
              </option>
            ))}
          </select>

          <select
            name="kategorija"
            value={filters.kategorija}
            onChange={handleFilterChange}
            style={{
              minWidth: "180px",
              padding: "8px 12px",
              border: "1px solid var(--border)",
              borderRadius: "8px",
              background: "var(--input-bg)",
              color: filters.kategorija ? "var(--text)" : "var(--text-muted)",
              fontSize: "14px",
            }}
          >
            <option value="">Sve kategorije</option>
            {kategorijeOptions.map((kategorija) => (
              <option key={kategorija} value={kategorija}>
                {kategorija}
              </option>
            ))}
          </select>

          {isAdminOrTehnicar && (
            <select
              name="arhiva"
              value={filters.arhiva}
              onChange={handleFilterChange}
              style={{
                minWidth: "180px",
                padding: "8px 12px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: "var(--text)",
                fontSize: "14px",
              }}
            >
              {ARHIVA_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          )}

          {activeFilterCount > 0 && (
            <button
              className="button sekundarno"
              onClick={resetFilters}
              style={{ whiteSpace: "nowrap", borderRadius: "8px" }}
            >
              Resetuj filtere ({activeFilterCount})
            </button>
          )}
        </div>

        <div className="card users-list-card">
          {message.text && (
            <p className={message.type === "error" ? "form-error" : "form-success"}>
              {message.text}
            </p>
          )}

          <div className="users-list-header users-list-row">
            <span>Naziv</span>
            <span>Kategorija</span>
            <span>Serijski broj</span>
            <span>Status</span>
            <span>Arhiva</span>
            <span>Kabinet</span>
            <span>Zgrada</span>
            <span>Akcije</span>
          </div>

          {loading ? (
            <div className="users-empty-state">Učitavanje podataka...</div>
          ) : visibleOprema.length > 0 ? (
            <div className="users-list">
              {visibleOprema.map((item) => (
                <div className="users-list-row users-list-item" key={item.id}>
                  <span style={{ fontWeight: 600 }}>{item.naziv}</span>
                  <span>{item.kategorija || "N/A"}</span>
                  <span>{item.serijskiBroj}</span>
                  <span>
                    <span className={`badge ${getStatusInfo(item.stanje).color}`}>
                      {getStatusInfo(item.stanje).label}
                    </span>
                  </span>
                  <span>
                    <span className={`badge ${item.isArchived ? "sivo" : "zeleno"}`}>
                      {item.isArchived ? "Arhivirana" : "Aktivna"}
                    </span>
                  </span>
                  <span>{item.kabinetNaziv}</span>
                  <span>{item.zgradaNaziv}</span>
                  <span>
                    <div className="users-actions">
                      {isAdminOrTehnicar && !item.isArchived && (
                        <>
                          <button className="users-action-btn" onClick={() => openEditModal(item)}>
                            Uredi
                          </button>
                          <button className="users-action-btn warn" onClick={() => handleArchive(item.id)}>
                            Arhiviraj
                          </button>
                        </>
                      )}

                      {isAdminOrTehnicar && item.isArchived && (
                        <button className="users-action-btn" onClick={() => handleRestore(item.id)}>
                          Vrati iz arhive
                        </button>
                      )}
                    </div>
                  </span>
                </div>
              ))}
            </div>
          ) : (
            <div className="users-empty-state">
              Nema pronađene opreme.
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
              <h2>{modalMode === "create" ? "Nova oprema" : "Uredi opremu"}</h2>
              <button className="users-modal-close" onClick={() => setModalOpen(false)}>
                ×
              </button>
            </div>

            {message.text && (
              <p className={message.type === "error" ? "form-error" : "form-success"}>
                {message.text}
              </p>
            )}

            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Naziv</label>
                <input name="naziv" value={formState.naziv} onChange={handleFormChange} required />
              </div>

              <div className="form-group">
                <label>Kategorija</label>
                <input
                  name="kategorija"
                  value={formState.kategorija}
                  onChange={handleFormChange}
                  placeholder="npr. laptop, osciloskop, projektor"
                  maxLength={40}
                  required
                />
              </div>

              <div className="form-group">
                <label>Serijski broj</label>
                <input value="Dodjeljuje se automatski nakon spremanja" readOnly />
              </div>

              <div className="form-group">
                <label>Status</label>
                <select name="stanje" value={formState.stanje} onChange={handleFormChange}>
                  {STATUS_OPTIONS.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label>Objekat</label>
                <select
                  value={selectedObjekatID}
                  onChange={(event) => {
                    setSelectedObjekatID(event.target.value);
                    setFormState((previous) => ({ ...previous, kabinetID: "" }));
                  }}
                  required
                >
                  <option value="">-- Odaberi objekat --</option>
                  {objekti.map((objekat) => (
                    <option key={objekat.id} value={objekat.id}>
                      {objekat.lokacija}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label>Kabinet</label>
                <select
                  name="kabinetID"
                  value={formState.kabinetID}
                  onChange={handleFormChange}
                  required
                  disabled={!selectedObjekatID}
                >
                  <option value="">
                    {selectedObjekatID ? "-- Odaberi kabinet --" : "Prvo odaberi objekat"}
                  </option>
                  {kabinetiZaObjekat.map((kabinet) => (
                    <option key={kabinet.id} value={kabinet.id}>
                      {kabinet.naziv}
                    </option>
                  ))}
                </select>
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Spremanje..." : "Sačuvaj"}
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

export default Oprema;
