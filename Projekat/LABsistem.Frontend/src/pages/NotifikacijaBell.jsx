import { useEffect, useRef, useState } from "react";
import api from "../api/client";

export default function NotifikacijaBell() {
  const [broj, setBroj] = useState(0);
  const [lista, setLista] = useState([]);
  const [open, setOpen] = useState(false);
  const ref = useRef(null);

  useEffect(() => {
    ucitajBroj();
    const interval = setInterval(ucitajBroj, 30000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    function handleClick(e) {
      if (ref.current && !ref.current.contains(e.target)) setOpen(false);
    }
    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, []);

  async function ucitajBroj() {
    try {
      const res = await api.get("/Obavijest/broj");
      setBroj(res.data.broj);
    } catch {}
  }

  async function toggleOpen() {
    if (!open) {
      try {
        const res = await api.get("/Obavijest");
        setLista(res.data);
      } catch {}
    }
    setOpen(prev => !prev);
  }

  async function oznaciSve() {
    try {
      await api.put("/Obavijest/sve-procitane");
      setBroj(0);
      setLista(prev => prev.map(n => ({ ...n, dostupnost: true })));
    } catch {}
  }

  async function oznaciJednu(id) {
    try {
      await api.put(`/Obavijest/${id}/procitana`);
      setLista(prev => prev.map(n => n.id === id ? { ...n, dostupnost: true } : n));
      setBroj(prev => Math.max(0, prev - 1));
    } catch {}
  }

  async function obrisiJednu(id, e) {
    e.stopPropagation();
    try {
      await api.delete(`/Obavijest/${id}`);
      const brisana = lista.find(n => n.id === id);
      setLista(prev => prev.filter(n => n.id !== id));
      if (brisana && !brisana.dostupnost) setBroj(prev => Math.max(0, prev - 1));
    } catch {}
  }

  async function obrisiSve() {
    try {
      await api.delete("/Obavijest/sve");
      setLista([]);
      setBroj(0);
    } catch {}
  }

  function formatVrijeme(iso) {
    return new Date(iso).toLocaleString("de-DE", {
      day: "2-digit", month: "2-digit", year: "numeric",
      hour: "2-digit", minute: "2-digit"
    });
  }

  return (
    <div ref={ref} style={{ position: "relative" }}>
      <button
        onClick={toggleOpen}
        style={{
          background: "none", border: "none", cursor: "pointer",
          position: "relative", padding: "6px", color: "var(--text)",
          display: "flex", alignItems: "center", justifyContent: "center"
        }}
        title="Obavijesti"
      >
        {/* SVG ikonica */}
        <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.9 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z"/>
        </svg>
        {broj > 0 && (
          <span style={{
            position: "absolute", top: "0", right: "0",
            background: "#ef4444", color: "#fff",
            borderRadius: "50%", fontSize: "10px", fontWeight: "700",
            width: "16px", height: "16px",
            display: "flex", alignItems: "center", justifyContent: "center",
            lineHeight: 1
          }}>
            {broj > 9 ? "9+" : broj}
          </span>
        )}
      </button>

      {open && (
        <div style={{
          position: "absolute", right: 0, top: "calc(100% + 8px)", zIndex: 1000,
          width: "340px", background: "var(--card-bg, #fff)",
          border: "1px solid var(--border)", borderRadius: "12px",
          boxShadow: "0 8px 24px rgba(0,0,0,0.12)", overflow: "hidden"
        }}>
          {/* Header */}
          <div style={{
            display: "flex", alignItems: "center", justifyContent: "space-between",
            padding: "12px 16px", borderBottom: "1px solid var(--border)"
          }}>
            <span style={{ fontWeight: "700", fontSize: "14px" }}>Obavijesti</span>
            <div style={{ display: "flex", gap: "8px" }}>
              {broj > 0 && (
                <button onClick={oznaciSve} style={{
                  background: "none", border: "none", cursor: "pointer",
                  fontSize: "12px", color: "var(--primary, #2563eb)"
                }}>
                  Označi sve
                </button>
              )}
              {lista.length > 0 && (
                <button onClick={obrisiSve} style={{
                  background: "none", border: "none", cursor: "pointer",
                  fontSize: "12px", color: "#ef4444"
                }}>
                  Obriši sve
                </button>
              )}
            </div>
          </div>

          {/* Lista */}
          <div style={{ maxHeight: "360px", overflowY: "auto" }}>
            {lista.length === 0 ? (
              <div style={{
                padding: "24px 16px", textAlign: "center",
                color: "var(--text-muted)", fontSize: "13px"
              }}>
                Nema obavijesti.
              </div>
            ) : (
              lista.map(n => (
                <div
                  key={n.id}
                  onClick={() => !n.dostupnost && oznaciJednu(n.id)}
                  style={{
                    padding: "10px 16px",
                    borderBottom: "1px solid var(--border)",
                    background: n.dostupnost ? "transparent" : "var(--hover-bg, #f0f7ff)",
                    cursor: n.dostupnost ? "default" : "pointer",
                    display: "flex", alignItems: "flex-start", gap: "8px"
                  }}
                >
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ fontSize: "13px", color: "var(--text)", lineHeight: "1.4" }}>
                      {!n.dostupnost && (
                        <span style={{
                          display: "inline-block", width: "7px", height: "7px",
                          borderRadius: "50%", background: "#2563eb",
                          marginRight: "6px", verticalAlign: "middle", flexShrink: 0
                        }} />
                      )}
                      {n.novosti}
                    </div>
                    <div style={{ fontSize: "11px", color: "var(--text-muted)", marginTop: "4px" }}>
                      {formatVrijeme(n.datumKreiranja)}
                    </div>
                  </div>
                  {/* Dugme za brisanje */}
                  <button
                    onClick={(e) => obrisiJednu(n.id, e)}
                    title="Obriši"
                    style={{
                      background: "none", border: "none", cursor: "pointer",
                      color: "var(--text-muted)", fontSize: "14px", padding: "0 2px",
                      lineHeight: 1, flexShrink: 0, marginTop: "2px"
                    }}
                    onMouseEnter={e => e.currentTarget.style.color = "#ef4444"}
                    onMouseLeave={e => e.currentTarget.style.color = "var(--text-muted)"}
                  >
                    ×
                  </button>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}