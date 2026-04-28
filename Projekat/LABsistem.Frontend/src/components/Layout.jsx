import { useEffect, useMemo, useRef, useState } from "react";
import { NavLink, useLocation, useNavigate } from "react-router-dom";
import { NAVIGATION_BY_ROLE, ROLE_LABELS, getCurrentRole } from "../auth/routeAccess";
import api from "../api/client";

function Layout({ children }) {
  const navigate = useNavigate();
  const location = useLocation();
  const uloga = getCurrentRole();
  const korisnik = localStorage.getItem("korisnik") || "Korisnik";
  const [korisnikEmail, setKorisnikEmail] = useState(
    localStorage.getItem("korisnikEmail") || "Prijavljeni korisnik"
  );
  const navStavke = NAVIGATION_BY_ROLE[uloga] || NAVIGATION_BY_ROLE.student;
  const [menuOtvoren, setMenuOtvoren] = useState(false);
  const accountMenuRef = useRef(null);
  const inicijal = useMemo(() => korisnik.trim().charAt(0).toUpperCase() || "K", [korisnik]);

  useEffect(() => {
    setMenuOtvoren(false);
  }, [location.pathname]);

  useEffect(() => {
    let aktivno = true;

    async function ucitajProfilZaMeni() {
      const token = localStorage.getItem("token");
      if (!token) {
        return;
      }

      try {
        const response = await api.get("/Auth/profile");
        const email = response.data?.email;
        if (!aktivno || !email) {
          return;
        }

        localStorage.setItem("korisnikEmail", email);
        setKorisnikEmail(email);
      } catch {
        // Fallback ostaje lokalno sacuvana vrijednost.
      }
    }

    ucitajProfilZaMeni();

    return () => {
      aktivno = false;
    };
  }, []);

  useEffect(() => {
    const handleKlikVan = (event) => {
      if (accountMenuRef.current && !accountMenuRef.current.contains(event.target)) {
        setMenuOtvoren(false);
      }
    };

    const handleEscape = (event) => {
      if (event.key === "Escape") {
        setMenuOtvoren(false);
      }
    };

    document.addEventListener("mousedown", handleKlikVan);
    document.addEventListener("keydown", handleEscape);

    return () => {
      document.removeEventListener("mousedown", handleKlikVan);
      document.removeEventListener("keydown", handleEscape);
    };
  }, []);

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
      localStorage.removeItem("korisnikEmail");
      navigate("/login");
    }
  };

  const idiNaProfil = () => {
    navigate("/profil");
  };

  const idiNaOAplikaciji = () => {
    navigate("/o-aplikaciji");
  };

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <NavLink to="/dashboard" style={{ textDecoration: "none" }}>
          <div className="sidebar-logo">
            LAB<span>sistem</span>
          </div>
        </NavLink>

        <nav className="sidebar-nav">
          <span className="nav-section">{ROLE_LABELS[uloga]}</span>
          {navStavke.map((stavka) => (
            <NavLink
              key={stavka.path}
              to={stavka.path}
              className={({ isActive }) => (isActive ? "active" : "")}
            >
              {stavka.label}
            </NavLink>
          ))}
        </nav>
      </aside>

      <div className="content-shell">
        <header className="topbar">
          <div className="topbar-spacer" />

          <div className="topbar-account" ref={accountMenuRef}>
            <button
              type="button"
              className="topbar-account-trigger"
              onClick={() => setMenuOtvoren((vrijednost) => !vrijednost)}
              aria-expanded={menuOtvoren}
            >
              <span className="topbar-avatar">{inicijal}</span>
              <span className="topbar-account-meta">
                <strong>{korisnik}</strong>
                <small>{ROLE_LABELS[uloga]}</small>
              </span>
              <span className={`topbar-chevron ${menuOtvoren ? "open" : ""}`}>▾</span>
            </button>

            {menuOtvoren && (
              <div className="topbar-menu">
                <div className="topbar-menu-header">
                  <strong>{korisnik}</strong>
                  <span>{korisnikEmail}</span>
                </div>

                <button type="button" className="topbar-menu-item" onClick={idiNaProfil}>
                  Moj profil
                </button>
                <button type="button" className="topbar-menu-item" onClick={idiNaOAplikaciji}>
                  O aplikaciji
                </button>
                <div className="topbar-menu-divider" />
                <button type="button" className="topbar-menu-item logout" onClick={handleOdjava}>
                  Odjavi se
                </button>
              </div>
            )}
          </div>
        </header>

        <main className="main-content">{children}</main>
      </div>
    </div>
  );
}

export default Layout;
