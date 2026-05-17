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

const INITIAL_FORM_STATE = {
  naziv: "",
  kategorija: "",
  serijskiBroj: "",
  stanje: 1,
  kabinetID: "",
};

const INITIAL_FILTERS = {
  searchTerm: "",
  status: "",
  kategorija: "",
  kabinet: "",
  zgrada: "",
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

  const [kvarModalOpen, setKvarModalOpen] = useState(false);
  const [kvarOprema, setKvarOprema] = useState(null);
  const [kvarKomentar, setKvarKomentar] = useState("");
  const [kvarSaving, setKvarSaving] = useState(false);

  const userRole = getLocalRole();
  const isTehnicar = userRole === "tehnicar" || userRole === "admin";
  const currentUserId = getCurrentUserId();

  useEffect(() => { loadAll(); }, []);

  async function loadAll() {
    setLoading(true);
    try {
      const [opremaRes, objektiRes] = await Promise.all([
        api.get("/Oprema"),
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
    return objekti.find(o => o.id === Number(selectedObjekatID))?.kabineti || [];
  }, [selectedObjekatID, objekti]);

  const kabinetiOptions = useMemo(() => {
    const unique = [...new Map(opremaList.map(o => [o.kabinetNaziv, o.kabinetNaziv])).values()];
    return unique.filter(Boolean);
  }, [opremaList]);

  const zgradaOptions = useMemo(() => {
    const unique = [...new Map(opremaList.map(o => [o.zgradaNaziv, o.zgradaNaziv])).values()];
    return unique.filter(Boolean);
  }, [opremaList]);

  const kategorijeOptions = useMemo(() => {
    const unique = [...new Map(opremaList.map(o => [o.kategorija, o.kategorija])).values()];
    return unique.filter(Boolean).sort((left, right) => left.localeCompare(right));
  }, [opremaList]);

  const nextSerijskiBroj = useMemo(() => {
    if (!opremaList.length) return 1;
    return Math.max(...opremaList.map((item) => Number(item.serijskiBroj) || 0)) + 1;
  }, [opremaList]);

  const filteredOprema = useMemo(() => {
    return opremaList.filter(o => {
      const termMatch =
        !filters.searchTerm ||
        o.naziv.toLowerCase().includes(filters.searchTerm.toLowerCase().trim()) ||
        (o.kategorija || "").toLowerCase().includes(filters.searchTerm.toLowerCase().trim()) ||
        o.serijskiBroj.toString().includes(filters.searchTerm.trim());
      const statusMatch = !filters.status || o.stanje === Number(filters.status);
      const kategorijaMatch = !filters.kategorija || o.kategorija === filters.kategorija;
      const kabinetMatch = !filters.kabinet || o.kabinetNaziv === filters.kabinet;
      const zgradaMatch = !filters.zgrada || o.zgradaNaziv === filters.zgrada;
      return termMatch && statusMatch && kategorijaMatch && kabinetMatch && zgradaMatch;
    });
  }, [opremaList, filters]);

  const activeFilterCount = Object.values(filters).filter(Boolean).length;

  function handleFilterChange(e) {
    const { name, value } = e.target;
    setFilters(prev => ({ ...prev, [name]: value }));
  }

  function resetFilters() { setFilters(INITIAL_FILTERS); }

  function handleFormChange(e) {
    const { name, value } = e.target;
    const processedValue =
      name === "stanje" || name === "serijskiBroj" || name === "kabinetID"
        ? Number(value)
        : value;
    setFormState(prev => ({ ...prev, [name]: processedValue }));
  }

  function openCreateModal() {
    setModalMode("create");
    setFormState({ ...INITIAL_FORM_STATE, serijskiBroj: nextSerijskiBroj });
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
      serijskiBroj: oprema.serijskiBroj,
      stanje: oprema.stanje,
      kabinetID: oprema.kabinetID,
    });
    const obj = objekti.find(o => o.kabineti?.some(k => k.id === oprema.kabinetID));
    setSelectedObjekatID(obj ? String(obj.id) : "");
    setMessage({ type: "", text: "" });
    setModalOpen(true);
  }

  function openKvarModal(item) {
    setKvarOprema(item);
    setKvarKomentar("");
    setKvarModalOpen(true);
  }

  async function handleKvarSubmit(e) {
    e.preventDefault();
    setKvarSaving(true);
    try {
      await api.post("/Evidencija", {
        opremaID: kvarOprema.id,
        korisnikID: Number(currentUserId),
        status: "Kvar",
        komentar: kvarKomentar,
      });
      setKvarModalOpen(false);
      setMessage({ type: "success", text: "Kvar uspješno prijavljen!" });
    } catch (error) {
      setMessage({ type: "error", text: extractErrorMessage(error, "Greška pri prijavi kvara.") });
      setKvarModalOpen(false);
    } finally {
      setKvarSaving(false);
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setSaving(true);
    setMessage({ type: "", text: "" });
    try {
      if (modalMode === "create") {
        await api.post("/Oprema", {
          ...formState,
          kreatorID: Number(currentUserId),
        });
        setMessage({ type: "success", text: "Oprema uspješno dodana!" });
      } else {
        await api.put(`/Oprema/${editingOprema.id}`, {
          ...formState,
          id: editingOprema.id,
          kreatorID: editingOprema.kreatorID,
        });
        setMessage({ type: "success", text: "Izmjene sačuvane!" });
      }
      await loadAll();
      setTimeout(() => setModalOpen(false), 800);
    } catch (error) {
      setMessage({ type: "error", text: extractErrorMessage(error, "Greška prilikom čuvanja podataka.") });
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(id) {
    if (!window.confirm("Da li ste sigurni da želite obrisati ovu opremu?")) return;
    try {
      await api.delete(`/Oprema/${id}`);
      await loadAll();
      setMessage({ type: "success", text: "Oprema obrisana." });
    } catch (error) {
      alert(extractErrorMessage(error, "Greška pri brisanju."));
    }
  }

  const getStatusInfo = val => STATUS_OPTIONS.find(s => s.value === Number(val)) || STATUS_OPTIONS[0];

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje opremom</h1>
      </div>

      <div className="users-page">
        <div
          className="card users-toolbar"
          style={{ flexWrap: "wrap", gap: "10px", alignItems: "center" }}
        >
          {isTehnicar && (
            <button className="button users-create-button" onClick={openCreateModal}>
              <span className="users-create-icon">+</span> Dodaj opremu
            </button>
          )}

          <div style={{ position: "relative", flex: "1", minWidth: "200px" }}>
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
              placeholder="Pretraži (naziv, serijski broj)..."
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

          <div style={{ position: "relative", minWidth: "160px" }}>
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
              ⚙️
            </span>
            <select
              name="status"
              value={filters.status}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px 8px 30px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: filters.status ? "var(--text)" : "var(--text-muted)",
                fontSize: "14px",
                appearance: "none",
                cursor: "pointer",
              }}
            >
              <option value="">Svi statusi</option>
              {STATUS_OPTIONS.map(opt => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
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

          <div style={{ position: "relative", minWidth: "160px" }}>
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
              {kabinetiOptions.map(k => <option key={k} value={k}>{k}</option>)}
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

          <div style={{ position: "relative", minWidth: "160px" }}>
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
              🏢
            </span>
            <select
              name="zgrada"
              value={filters.zgrada}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px 8px 30px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: filters.zgrada ? "var(--text)" : "var(--text-muted)",
                fontSize: "14px",
                appearance: "none",
                cursor: "pointer",
              }}
            >
              <option value="">Sve zgrade</option>
              {zgradaOptions.map(z => <option key={z} value={z}>{z}</option>)}
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

          <div style={{ position: "relative", minWidth: "180px" }}>
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
              #
            </span>
            <select
              name="kategorija"
              value={filters.kategorija}
              onChange={handleFilterChange}
              style={{
                width: "100%",
                padding: "8px 12px 8px 30px",
                border: "1px solid var(--border)",
                borderRadius: "8px",
                background: "var(--input-bg)",
                color: filters.kategorija ? "var(--text)" : "var(--text-muted)",
                fontSize: "14px",
                appearance: "none",
                cursor: "pointer",
              }}
            >
              <option value="">Sve kategorije</option>
              {kategorijeOptions.map((kategorija) => (
                <option key={kategorija} value={kategorija}>{kategorija}</option>
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
              â–¼
            </span>
          </div>

          {activeFilterCount > 0 && (
            <button
              className="button sekundarno"
              onClick={resetFilters}
              style={{ whiteSpace: "nowrap", borderRadius: "8px" }}
            >
              ✕ Resetuj ({activeFilterCount})
            </button>
          )}
        </div>

        <div className="card users-list-card">
          {message.text && !modalOpen && !kvarModalOpen && (
            <p className={message.type === "error" ? "form-error" : "form-success"}>
              {message.text}
            </p>
          )}

          <div className="users-list-header users-list-row">
            <span>Naziv</span>
            <span>Kategorija</span>
            <span>Serijski broj</span>
            <span>Status</span>
            <span>Kabinet</span>
            <span>Zgrada</span>
            <span>Akcije</span>
          </div>

          {loading ? (
            <div className="users-empty-state">Učitavanje podataka...</div>
          ) : filteredOprema.length > 0 ? (
            <div className="users-list">
              {filteredOprema.map(item => (
                <div className="users-list-row users-list-item" key={item.id}>
                  <span style={{ fontWeight: "600" }}>{item.naziv}</span>
                  <span>{item.kategorija || "N/A"}</span>
                  <span>{item.serijskiBroj}</span>
                  <span>
                    <span className={`badge ${getStatusInfo(item.stanje).color}`}>
                      {getStatusInfo(item.stanje).label}
                    </span>
                  </span>
                  <span>{item.kabinetNaziv}</span>
                  <span>{item.zgradaNaziv}</span>
                  <span>
                    <div className="users-actions">
                      {isTehnicar && (
                        <>
                          <button className="users-action-btn" onClick={() => openEditModal(item)}>
                            ✎ Uredi
                          </button>
                          <button className="users-action-btn warn" onClick={() => handleDelete(item.id)}>
                            🗑 Briši
                          </button>
                        </>
                      )}
                      {userRole === "student" && (
                        <button className="users-action-btn warn" onClick={() => openKvarModal(item)}>
                          ⚠ Prijavi kvar
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
          <div className="users-modal" onClick={e => e.stopPropagation()}>
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
                  placeholder="npr. Laptop, osciloskop, projektor"
                  maxLength={40}
                  required
                />
              </div>
              <div className="form-group">
                <label>Serijski broj</label>
                <input
                  name="serijskiBroj"
                  type="number"
                  value={formState.serijskiBroj}
                  onChange={handleFormChange}
                  readOnly
                  required
                />
              </div>
              <div className="form-group">
                <label>Status</label>
                <select name="stanje" value={formState.stanje} onChange={handleFormChange}>
                  {STATUS_OPTIONS.map(opt => <option key={opt.value} value={opt.value}>{opt.label}</option>)}
                </select>
              </div>

              <div className="form-group">
                <label>Objekat</label>
                <select
                  value={selectedObjekatID}
                  onChange={e => {
                    setSelectedObjekatID(e.target.value);
                    setFormState(prev => ({ ...prev, kabinetID: "" }));
                  }}
                  required
                >
                  <option value="">-- Odaberi objekat --</option>
                  {objekti.map(o => (
                    <option key={o.id} value={o.id}>{o.lokacija}</option>
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
                  {kabinetiZaObjekat.map(k => (
                    <option key={k.id} value={k.id}>{k.naziv}</option>
                  ))}
                </select>
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : "Sačuvaj"}
                </button>
                <button className="button sekundarno" type="button" onClick={() => setModalOpen(false)}>
                  Odustani
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {kvarModalOpen && (
        <div className="users-modal-overlay" onClick={() => setKvarModalOpen(false)}>
          <div className="users-modal" onClick={e => e.stopPropagation()}>
            <div className="users-modal-header">
              <h2>⚠ Prijava kvara — {kvarOprema?.naziv}</h2>
              <button className="users-modal-close" onClick={() => setKvarModalOpen(false)}>
                ×
              </button>
            </div>
            <form onSubmit={handleKvarSubmit}>
              <div className="form-group">
                <label>Opis kvara</label>
                <textarea
                  value={kvarKomentar}
                  onChange={e => setKvarKomentar(e.target.value)}
                  maxLength={20}
                  required
                  rows={4}
                  placeholder="Opišite kvar što detaljnije..."
                  style={{
                    width: "100%",
                    padding: "8px",
                    borderRadius: "6px",
                    border: "1px solid var(--border)",
                    resize: "vertical",
                    background: "var(--input-bg)",
                    color: "var(--text)",
                    fontSize: "14px",
                  }}
                />
              </div>
              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={kvarSaving}>
                  {kvarSaving ? "Slanje..." : "Prijavi kvar"}
                </button>
                <button className="button sekundarno" type="button" onClick={() => setKvarModalOpen(false)}>
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
