import { Link, useLocation } from "react-router-dom";
import Layout from "../components/Layout";

function AccessDenied() {
  const location = useLocation();
  const attemptedPath = location.state?.attemptedPath;

  return (
    <Layout>
      <div className="page-header">
        <h1>Nemate pristup ovoj stranici</h1>
        <p>
          Ovaj sadrzaj nije dostupan za vasu ulogu. Koristite opcije iz svog menija
          ili se vratite na pocetnu stranicu.
        </p>
      </div>

      <div className="card access-denied-card">
        <p>
          Pokusali ste otvoriti rutu{" "}
          <strong>{attemptedPath || "kojoj trenutno nemate pristup"}</strong>.
        </p>
        <p style={{ color: "#64748b" }}>
          Ako mislite da je ovo greska, prijavite problem administratoru sistema.
        </p>
        <div className="access-denied-actions">
          <Link className="button" to="/dashboard">
            Nazad na dashboard
          </Link>
        </div>
      </div>
    </Layout>
  );
}

export default AccessDenied;
