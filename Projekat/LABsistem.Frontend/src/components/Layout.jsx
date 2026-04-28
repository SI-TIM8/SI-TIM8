import { NavLink, useNavigate } from "react-router-dom";
import api from "../api/client";

const NAV_PO_ULOZI = {
  student: [
    { label: "Kalendar termina", path: "/kalendar" },
    { label: "Zakaži termin", path: "/zakazivanje" },
    { label: "Moje rezervacije", path: "/rezervacije" },
    { label: "Oprema", path: "/oprema" },
    { label: "Moj profil", path: "/profil" },
  ],
  profesor: [
    { label: "Zahtjevi studenata", path: "/zahtjevi" },
    { label: "Lista rezervacija", path: "/rezervacije" },
    { label: "Historija studenata", path: "/historija" },
    { label: "Kalendar termina", path: "/kalendar" },
    { label: "Moj profil", path: "/profil" },
  ],
  tehnicar: [
    { label: "Upravljanje terminima", path: "/termini" },
    { label: "Upravljanje opremom", path: "/oprema" },
    { label: "Kvarovi opreme", path: "/kvarovi" },
    { label: "Kalendar termina", path: "/kalendar" },
    { label: "Moj profil", path: "/profil" },
  ],
  admin: [
    { label: "Upravljanje korisnicima", path: "/korisnici" },
    { label: "Objekti i kabineti", path: "/objekti" },
    { label: "Kalendar termina", path: "/kalendar" },
    { label: "Moj profil", path: "/profil" },
  ],
};

const ULOGA_LABELA = {
  student: "Student",
  profesor: "Profesor / Asistent",
  tehnicar: "Lab. tehničar",
  admin: "Administrator",
};

function Layout({ children }) {
  const navigate = useNavigate();

  // Čita ulogu iz localStorage (postavlja se pri prijavi)
  const uloga = localStorage.getItem("uloga") || "student";
  const korisnik = localStorage.getItem("korisnik") || "Korisnik";
  const navStavke = NAV_PO_ULOZI[uloga] || NAV_PO_ULOZI.student;

  const handleOdjava = async () => {
    try {
      await api.post("/Auth/logout");
    } catch {
      // Client-side cleanup still happens even if the backend logout fails.
    } finally {
      localStorage.removeItem("token");
      localStorage.removeItem("tokenExpiry");
      localStorage.removeItem("uloga");
      localStorage.removeItem("korisnik");
      navigate("/login");
    }
  };

  return (
    <div className="app-layout">
      {/* Sidebar */}
      <aside className="sidebar">
        <NavLink to="/dashboard" style={{ textDecoration: "none" }}>
        <div className="sidebar-logo">
            LAB<span>sistem</span>
            </div>
        </NavLink>

        <nav className="sidebar-nav">
          <span className="nav-section">{ULOGA_LABELA[uloga]}</span>
          {navStavke.map((stavka) => (
            <NavLink
              key={stavka.path}
              to={stavka.path}
              className={({ isActive }) => isActive ? "active" : ""}
            >
              {stavka.label}
            </NavLink>
          ))}
        </nav>

        <div className="sidebar-footer">
          <strong>{korisnik}</strong>
          <button className="button sekundarno malo" onClick={handleOdjava}>
            Odjavi se
          </button>
        </div>
      </aside>

      {/* Glavni sadržaj */}
      <main className="main-content">
        {children}
      </main>
    </div>
  );
}

export default Layout;
