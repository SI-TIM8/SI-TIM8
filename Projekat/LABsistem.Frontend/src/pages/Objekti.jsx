import { useEffect, useMemo, useState, useRef } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

const getLocalRole = () => localStorage.getItem("uloga") || "student";

const INIT_OBJEKAT = { lokacija: "", radnoVrijeme: "" };
const INIT_KABINET = { naziv: "", korisnikID: "", objekatID: null, kapacitet: "" };

function extractError(error, fallback) {
  const d = error?.response?.data;
  if (typeof d === "string" && d.trim()) return d;
  if (typeof d?.message === "string" && d.message.trim()) return d.message;
  return fallback;
}

function ProfAutocomplete({ korisnici, value, onChange, initialDisplayName = "" }) {
  const [inputVal, setInputVal] = useState("");
  const [open, setOpen] = useState(false);
  const ref = useRef(null);

  useEffect(() => {
    if (!value) {
      setInputVal(initialDisplayName || "");
      return;
    }
    const found = korisnici.find(k => k.userId === Number(value));
    if (found) setInputVal(found.imePrezime);
    else setInputVal(initialDisplayName || "");
  }, [value, korisnici, initialDisplayName]);

  useEffect(() => {
    function handleClick(e) {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false);
    }
    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, []);

  const filtered = useMemo(() =>
    !inputVal.trim()
      ? korisnici
      : korisnici.filter(k => k.imePrezime.toLowerCase().includes(inputVal.toLowerCase().trim()))
  , [inputVal, korisnici]);

  return (
    <div ref={ref} style={{ position: "relative" }}>
      <input
        type="text"
        value={inputVal}
        onChange={e => { setInputVal(e.target.value); onChange(""); setOpen(true); }}
        onFocus={() => setOpen(true)}
        placeholder="Ime i prezime profesora..."
        autoComplete="off"
        required
        style={{ width: "100%", boxSizing: "border-box" }}
      />
      {open && filtered.length > 0 && (
        <div style={{
          position: "absolute", top: "100%", left: 0, right: 0, zIndex: 1000,
          background: "var(--card-bg, #fff)", border: "1px solid var(--border)",
          borderRadius: "8px", boxShadow: "0 4px 12px rgba(0,0,0,0.15)",
          maxHeight: "200px", overflowY: "auto"
        }}>
          {filtered.map(k => (
            <div
              key={k.userId}
              onMouseDown={() => { setInputVal(k.imePrezime); onChange(k.userId); setOpen(false); }}
              style={{ padding: "8px 12px", cursor: "pointer", fontSize: "14px", borderBottom: "1px solid var(--border)" }}
              onMouseEnter={e => e.currentTarget.style.background = "var(--hover-bg, #f0f0f0)"}
              onMouseLeave={e => e.currentTarget.style.background = "transparent"}
            >
              {k.imePrezime}
            </div>
          ))}
        </div>
      )}
      {open && filtered.length === 0 && inputVal && (
        <div style={{
          position: "absolute", top: "100%", left: 0, right: 0, zIndex: 1000,
          background: "var(--card-bg, #fff)", border: "1px solid var(--border)",
          borderRadius: "8px", padding: "8px 12px", fontSize: "13px", color: "var(--text-muted)"
        }}>
          Nema pronađenih profesora.
        </div>
      )}
    </div>
  );
}

export default function Objekti() {
  const [objekti, setObjekti] = useState([]);
  const [korisnici, setKorisnici] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [expanded, setExpanded] = useState({});
  const [message, setMessage] = useState({ type: "", text: "" });
  const [modal, setModal] = useState(null);
  const [form, setForm] = useState({});
  const [saving, setSaving] = useState(false);
  const [formMsg, setFormMsg] = useState({ type: "", text: "" });

  const isAdmin = getLocalRole() === "admin";

  useEffect(() => { load(); }, []);

  async function load() {
    setLoading(true);
    try {
      const [objRes, korRes] = await Promise.all([
        api.get("/Objekat"),
        api.get("/Auth/users"),
      ]);
      setObjekti(objRes.data);
      // Samo profesori
      setKorisnici(korRes.data.filter(k => k.role === "Profesor"));
    } catch (e) {
      setMessage({ type: "error", text: "Greška pri učitavanju podataka." });
    } finally {
      setLoading(false);
    }
  }

  function toggleExpand(id) {
    setExpanded(prev => ({ ...prev, [id]: !prev[id] }));
  }

  const filtered = useMemo(() => {
    const t = search.toLowerCase().trim();
    if (!t) return objekti;
    return objekti.filter(o =>
      o.lokacija.toLowerCase().includes(t) ||
      o.radnoVrijeme.toLowerCase().includes(t) ||
      o.kabineti?.some(k => k.naziv.toLowerCase().includes(t))
    );
  }, [objekti, search]);

  function openModal(type, data = {}) {
    setModal({ type, data });
    if (type === "objekat-create") setForm(INIT_OBJEKAT);
    else if (type === "objekat-edit") setForm({ lokacija: data.lokacija, radnoVrijeme: data.radnoVrijeme });
    else if (type === "kabinet-create") setForm({ ...INIT_KABINET, objekatID: data.objekatID });
    else if (type === "kabinet-edit") setForm({ naziv: data.naziv, korisnikID: data.korisnikID, objekatID: data.objekatID, kapacitet: data.kapacitet });
    setFormMsg({ type: "", text: "" });
  }

  function closeModal() { setModal(null); setForm({}); }

  function handleChange(e) {
    const { name, value } = e.target;
    const numericFields = ["korisnikID", "kapacitet"];
    setForm(prev => ({ ...prev, [name]: numericFields.includes(name) ? Number(value) : value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setSaving(true);
    setFormMsg({ type: "", text: "" });
    try {
      const { type, data } = modal;
      if (type === "objekat-create") {
        await api.post("/Objekat", form);
        setFormMsg({ type: "success", text: "Objekat dodan!" });
      } else if (type === "objekat-edit") {
        await api.put(`/Objekat/${data.id}`, form);
        setFormMsg({ type: "success", text: "Objekat ažuriran!" });
      } else if (type === "kabinet-create") {
        await api.post("/Kabinet", form);
        setFormMsg({ type: "success", text: "Kabinet dodan!" });
      } else if (type === "kabinet-edit") {
        await api.put(`/Kabinet/${data.id}`, form);
        setFormMsg({ type: "success", text: "Izmjene sačuvane!" });
      }
      await load();
      setTimeout(closeModal, 700);
    } catch (e) {
      setFormMsg({ type: "error", text: extractError(e, "Greška pri čuvanju.") });
    } finally {
      setSaving(false);
    }
  }

  async function deleteObjekat(id) {
    if (!window.confirm("Obrisati objekat i sve kabinete u njemu?")) return;
    try {
      await api.delete(`/Objekat/${id}`);
      await load();
      setMessage({ type: "success", text: "Objekat obrisan." });
    } catch (e) {
      setMessage({ type: "error", text: extractError(e, "Greška pri brisanju.") });
    }
  }

  async function deleteKabinet(id) {
    if (!window.confirm("Obrisati ovaj kabinet?")) return;
    try {
      await api.delete(`/Kabinet/${id}`);
      await load();
      setMessage({ type: "success", text: "Kabinet obrisan." });
    } catch (e) {
      setMessage({ type: "error", text: extractError(e, "Greška pri brisanju.") });
    }
  }

  const modalTitle = {
    "objekat-create": "Novi objekat",
    "objekat-edit": "Uredi objekat",
    "kabinet-create": "Novi kabinet",
    "kabinet-edit": "Uredi kabinet",
  }[modal?.type] || "";

  const isKabinetModal = modal?.type?.startsWith("kabinet");

  return (
    <Layout>
      <div className="page-header">
        <h1>Objekti i kabineti</h1>
      </div>

      <div className="users-page">
        <div className="card users-toolbar" style={{ flexWrap: "wrap", gap: "10px", alignItems: "center" }}>
          {isAdmin && (
            <button className="button users-create-button" onClick={() => openModal("objekat-create")}>
              <span className="users-create-icon">+</span> Dodaj objekat
            </button>
          )}
          <div style={{ position: "relative", flex: 1, minWidth: "200px" }}>
            <span style={{ position: "absolute", left: "10px", top: "50%", transform: "translateY(-50%)", color: "var(--text-muted)", pointerEvents: "none" }}>🔍</span>
            <input
              type="text"
              placeholder="Pretraži objekte i kabinete..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              style={{ width: "100%", padding: "8px 12px 8px 32px", border: "1px solid var(--border)", borderRadius: "8px", background: "var(--input-bg)", color: "var(--text)", fontSize: "14px", boxSizing: "border-box" }}
            />
          </div>
        </div>

        {message.text && (
          <p className={message.type === "error" ? "form-error" : "form-success"} style={{ margin: "0 0 12px" }}>
            {message.text}
          </p>
        )}

        <div className="card users-list-card">
          {loading ? (
            <div className="users-empty-state">Učitavanje...</div>
          ) : filtered.length === 0 ? (
            <div className="users-empty-state">Nema pronađenih objekata.</div>
          ) : (
            filtered.map(objekat => (
              <div key={objekat.id} style={{ borderBottom: "1px solid var(--border)", padding: "12px 0" }}>
                <div style={{ display: "flex", alignItems: "center", gap: "12px", padding: "4px 8px" }}>
                  <button
                    onClick={() => toggleExpand(objekat.id)}
                    style={{ background: "none", border: "none", cursor: "pointer", fontSize: "18px", color: "var(--text-muted)", width: "28px" }}
                  >
                    {expanded[objekat.id] ? "▾" : "▸"}
                  </button>
                  <div style={{ flex: 1 }}>
                    <span style={{ fontWeight: "700", fontSize: "16px" }}>{objekat.lokacija}</span>
                    <span style={{ marginLeft: "12px", color: "var(--text-muted)", fontSize: "13px" }}>🕐 {objekat.radnoVrijeme}</span>
                    <span style={{ marginLeft: "12px", color: "var(--text-muted)", fontSize: "13px" }}>📦 {objekat.kabineti?.length || 0} kabineta</span>
                  </div>
                  {isAdmin && (
                    <div className="users-actions">
                      <button className="users-action-btn" onClick={() => openModal("objekat-edit", objekat)}>✎ Uredi</button>
                      <button className="users-action-btn" onClick={() => openModal("kabinet-create", { objekatID: objekat.id })}>+ Kabinet</button>
                      <button className="users-action-btn warn" onClick={() => deleteObjekat(objekat.id)}>🗑 Briši</button>
                    </div>
                  )}
                </div>

                {expanded[objekat.id] && (
                  <div style={{ marginLeft: "48px", marginTop: "8px" }}>
                    {!objekat.kabineti?.length ? (
                      <div style={{ color: "var(--text-muted)", fontSize: "13px", padding: "6px 0" }}>Nema kabineta u ovom objektu.</div>
                    ) : (
                      <>
                        <div className="users-list-header users-list-row" style={{ fontSize: "12px" }}>
                          <span>Naziv</span>
                          <span>Odgovorni profesor</span>
                          <span>Kapacitet</span>
                          {isAdmin && <span>Akcije</span>}
                        </div>
                        {objekat.kabineti.map(k => (
                          <div className="users-list-row users-list-item" key={k.id}>
                            <span style={{ fontWeight: "600" }}>{k.naziv}</span>
                            <span>{k.odgovorniKorisnik}</span>
                            <span>{k.kapacitet}</span>
                            {isAdmin && (
                              <span>
                                <div className="users-actions">
                                  <button className="users-action-btn" onClick={() => openModal("kabinet-edit", k)}>✎ Uredi</button>
                                  <button className="users-action-btn warn" onClick={() => deleteKabinet(k.id)}>🗑 Briši</button>
                                </div>
                              </span>
                            )}
                          </div>
                        ))}
                      </>
                    )}
                  </div>
                )}
              </div>
            ))
          )}
        </div>
      </div>

      {modal && (
        <div className="users-modal-overlay" onClick={closeModal}>
          <div className="users-modal" onClick={e => e.stopPropagation()}>
            <div className="users-modal-header">
              <h2>{modalTitle}</h2>
              <button className="users-modal-close" onClick={closeModal}>×</button>
            </div>
            {formMsg.text && (
              <p className={formMsg.type === "error" ? "form-error" : "form-success"}>{formMsg.text}</p>
            )}
            <form onSubmit={handleSubmit}>
              {!isKabinetModal ? (
                <>
                  <div className="form-group">
                    <label>Lokacija</label>
                    <input name="lokacija" value={form.lokacija || ""} onChange={handleChange} required maxLength={20} />
                  </div>
                  <div className="form-group">
                    <label>Radno vrijeme</label>
                    <input name="radnoVrijeme" value={form.radnoVrijeme || ""} onChange={handleChange} required maxLength={20} placeholder="npr. 08:00 - 20:00" />
                  </div>
                </>
              ) : (
                <>
                  <div className="form-group">
                    <label>Naziv kabineta</label>
                    <input name="naziv" value={form.naziv || ""} onChange={handleChange} required />
                  </div>
                  <div className="form-group">
                    <label>Odgovorni profesor</label>
                    <ProfAutocomplete
                      korisnici={korisnici}
                      value={form.korisnikID}
                      onChange={val => setForm(prev => ({ ...prev, korisnikID: val }))}
                      initialDisplayName={modal?.data?.odgovorniKorisnik || ""}
                    />
                  </div>
                  <div className="form-group">
                    <label>Kapacitet</label>
                    <input type="number" name="kapacitet" value={form.kapacitet || ""} onChange={handleChange} required min="1" />
                  </div>
                </>
              )}
              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={saving}>
                  {saving ? "Slanje..." : "Sačuvaj"}
                </button>
                <button className="button sekundarno" type="button" onClick={closeModal}>Odustani</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Layout>
  );
}
