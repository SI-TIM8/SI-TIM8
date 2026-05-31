import { useEffect, useMemo, useRef, useState } from "react";
import { NavLink, useLocation, useNavigate } from "react-router-dom";
import { NAVIGATION_BY_ROLE, ROLE_LABELS, getCurrentRole } from "../auth/routeAccess";
import api from "../api/client";
import { clearSession, getRefreshToken } from "../auth/session";
import NotifikacijaBell from "../pages/NotifikacijaBell";

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

  const [theme, setTheme] = useState(() => {
    return localStorage.getItem("theme") || "light";
  });

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  const toggleTheme = () => {
    setTheme((prev) => (prev === "light" ? "dark" : "light"));
  };


  useEffect(() => {
    setMenuOtvoren(false);
  }, [location.pathname]);

  useEffect(() => {
    let aktivno = true;

    async function ucitajProfilZaMeni() {
      const token = localStorage.getItem("token");
      if (!token) return;
      try {
        const response = await api.get("/Auth/profile");
        const email = response.data?.email;
        if (!aktivno || !email) return;
        localStorage.setItem("korisnikEmail", email);
        setKorisnikEmail(email);
      } catch {}
    }

    ucitajProfilZaMeni();
    return () => { aktivno = false; };
  }, []);

  useEffect(() => {
    const handleKlikVan = (event) => {
      if (accountMenuRef.current && !accountMenuRef.current.contains(event.target)) {
        setMenuOtvoren(false);
      }
    };
    const handleEscape = (event) => {
      if (event.key === "Escape") setMenuOtvoren(false);
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
      await api.post("/Auth/logout", { refreshToken: getRefreshToken() });
    } catch {}
    finally {
      clearSession();
      navigate("/login");
    }
  };

  const [showScrollButton, setShowScrollButton] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setShowScrollButton(window.scrollY > 300);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };
  return (
    <div className="app-layout">
      <aside className="sidebar">
        <NavLink to="/dashboard" style={{ textDecoration: "none" }}>
          <div className="sidebar-logo">LAB<span>sistem</span></div>
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

          <div style={{ display: "flex", alignItems: "center", marginRight: "16px", gap: "8px" }}>
            <button
              onClick={toggleTheme}
              className="theme-toggle-btn"
              title={theme === "light" ? "Aktiviraj tamni režim" : "Aktiviraj svijetli režim"}
              style={{
                background: "none",
                border: "none",
                cursor: "pointer",
                padding: "6px",
                color: "var(--text-muted, #64748b)",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                borderRadius: "50%",
                transition: "background 0.2s, color 0.2s"
              }}
              onMouseEnter={(e) => {
                e.currentTarget.style.background = "var(--hover-bg)";
                e.currentTarget.style.color = "var(--text-primary)";
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.background = "none";
                e.currentTarget.style.color = "var(--text-muted)";
              }}
            >
              {theme === "light" ? (
                <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                  <path d="M12.3 22c-5.5 0-10-4.5-10-10 0-4.8 3.5-8.9 8.2-9.8.5-.1 1 .3.9.8-.1.4-.4.8-.4 1.2 0 4.4 3.6 8 8 8 .4 0 .8-.1 1.2-.2.5-.1.9.4.8.9-.9 4.7-5 8.1-9.7 8.1z"/>
                </svg>
              ) : (
                <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                  <path d="M12 7c-2.76 0-5 2.24-5 5s2.24 5 5 5 5-2.24 5-5-2.24-5-5-5zM2 13h2c.55 0 1-.45 1-1s-.45-1-1-1H2c-.55 0-1 .45-1 1s.45 1 1 1zm18 0h2c.55 0 1-.45 1-1s-.45-1-1-1h-2c-.55 0-1 .45-1 1s.45 1 1 1zM11 2v2c0 .55.45 1 1 1s1-.45 1-1V2c0-.55-.45-1-1-1s-1 .45-1 1zm0 18v2c0 .55.45 1 1 1s1-.45 1-1v-2c0-.55-.45-1-1-1s-1 .45-1 1zM5.99 4.58c-.39-.39-1.03-.39-1.41 0s-.39 1.03 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0s.39-1.03 0-1.41L5.99 4.58zm12.37 12.37c-.39-.39-1.03-.39-1.41 0s-.39 1.03 0 1.41l1.06 1.06c.39.39 1.03.39 1.41 0s.39-1.03 0-1.41l-1.06-1.06zm1.06-12.37c-.39-.39-.39-1.03 0-1.41s1.03-.39 1.41 0l1.06 1.06c.39.39.39 1.03 0 1.41s-1.03.39-1.41 0l-1.06-1.06zm-12.37 12.37c-.39-.39-.39-1.03 0-1.41s1.03-.39 1.41 0l1.06 1.06c.39.39.39 1.03 0 1.41s-1.03.39-1.41 0l-1.06-1.06z"/>
                </svg>
              )}
            </button>
            <NotifikacijaBell />
          </div>


          <div className="topbar-account" ref={accountMenuRef}>
            <button
              type="button"
              className="topbar-account-trigger"
              onClick={() => setMenuOtvoren((v) => !v)}
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
                <button type="button" className="topbar-menu-item" onClick={() => navigate("/profil")}>
                  Moj profil
                </button>
                <button type="button" className="topbar-menu-item" onClick={() => navigate("/o-aplikaciji")}>
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

        {/* Dugme za povratak na vrh */}
        {showScrollButton && (
          <button
            onClick={scrollToTop}
            style={{
              position: "fixed",
              bottom: "30px",
              right: "30px",
              background: "#0f766e",
              color: "white",
              border: "none",
              borderRadius: "50%",
              width: "50px",
              height: "50px",
              fontSize: "24px",
              cursor: "pointer",
              boxShadow: "0 4px 12px rgba(0,0,0,0.2)",
              zIndex: 999
            }}
          >
            ↑
          </button>
        )}
       </div>
    </div>
  );
}

export default Layout;
