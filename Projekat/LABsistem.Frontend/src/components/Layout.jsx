import { NavLink, useNavigate } from "react-router-dom";
import { NAVIGATION_BY_ROLE, ROLE_LABELS, getCurrentRole } from "../auth/routeAccess";
import api from "../api/client";

function Layout({ children }) {
  const navigate = useNavigate();
  const uloga = getCurrentRole();
  const korisnik = localStorage.getItem("korisnik") || "Korisnik";
  const navStavke = NAVIGATION_BY_ROLE[uloga] || NAVIGATION_BY_ROLE.student;

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

        <div className="sidebar-footer">
          <strong>{korisnik}</strong>
          <button className="button sekundarno malo" onClick={handleOdjava}>
            Odjavi se
          </button>
        </div>
      </aside>

      <main className="main-content">{children}</main>
    </div>
  );
}

export default Layout;
