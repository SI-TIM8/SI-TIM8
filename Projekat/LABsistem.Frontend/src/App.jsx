import { BrowserRouter, Navigate, Route, Routes, useLocation, useNavigate } from "react-router-dom";
import { useCallback, useEffect, useState } from "react";
import AccessDenied from "./pages/AccessDenied";
import AboutApp from "./pages/AboutApp";
import Dashboard from "./pages/Dashboard";
import Korisnici from "./pages/Korisnici";
import Layout from "./components/Layout";
import Login from "./pages/Login";
import Profil from "./pages/Profil";
import { ALLOWED_ROLES_BY_ROUTE, canAccessRoute, getCurrentRole } from "./auth/routeAccess";

const SESSION_DURATION_MS = 30 * 60 * 1000;
const UPOZORENJE_MS = 5 * 60 * 1000;

function clearSession() {
  localStorage.removeItem("token");
  localStorage.removeItem("tokenExpiry");
  localStorage.removeItem("uloga");
  localStorage.removeItem("korisnik");
}

function ZasticenaRuta({ children, allowedRoles }) {
  const token = localStorage.getItem("token");
  const expiry = localStorage.getItem("tokenExpiry");
  const location = useLocation();
  const aktivna = token && expiry && Date.now() < parseInt(expiry, 10);

  if (!aktivna) {
    clearSession();
    return <Navigate to="/login?sesija=istekla" replace />;
  }

  const uloga = getCurrentRole();
  if (allowedRoles && !allowedRoles.includes(uloga)) {
    return (
      <Navigate
        to="/pristup-odbijen"
        replace
        state={{ attemptedPath: location.pathname }}
      />
    );
  }

  return children;
}

function SesijaTimer() {
  const navigate = useNavigate();
  const [upozorenje, setUpozorenje] = useState(false);
  const [preostaloSekundi, setPreostaloSekundi] = useState(null);

  const resetujTimer = useCallback(() => {
    const expiry = localStorage.getItem("tokenExpiry");
    if (!expiry) {
      return;
    }

    localStorage.setItem("tokenExpiry", (Date.now() + SESSION_DURATION_MS).toString());
    setUpozorenje(false);
  }, []);

  useEffect(() => {
    const dogadjaji = ["mousemove", "keydown", "click", "scroll", "touchstart"];
    dogadjaji.forEach((dogadjaj) => window.addEventListener(dogadjaj, resetujTimer));

    const interval = setInterval(() => {
      const expiry = localStorage.getItem("tokenExpiry");
      if (!expiry) {
        return;
      }

      const preostalo = parseInt(expiry, 10) - Date.now();

      if (preostalo <= 0) {
        clearInterval(interval);
        clearSession();
        navigate("/login?sesija=istekla");
      } else if (preostalo <= UPOZORENJE_MS) {
        setUpozorenje(true);
        setPreostaloSekundi(Math.floor(preostalo / 1000));
      }
    }, 1000);

    return () => {
      clearInterval(interval);
      dogadjaji.forEach((dogadjaj) => window.removeEventListener(dogadjaj, resetujTimer));
    };
  }, [navigate, resetujTimer]);

  if (!upozorenje) {
    return null;
  }

  return (
    <div className="sesija-upozorenje">
      Sesija istice za <strong>{preostaloSekundi}</strong> sekundi zbog neaktivnosti.
    </div>
  );
}

function PlaceholderStranica({ naslov, opis }) {
  return (
    <Layout>
      <div className="page-header">
        <h1>{naslov}</h1>
        <p>{opis}</p>
      </div>
      <div className="card">
        <p style={{ color: "#94a3b8", fontStyle: "italic" }}>
          Stranica je u izradi.
        </p>
      </div>
    </Layout>
  );
}

function ProtectedPage({ path, children }) {
  return (
    <ZasticenaRuta allowedRoles={ALLOWED_ROLES_BY_ROUTE[path]}>
      {children}
    </ZasticenaRuta>
  );
}

function App() {
  return (
    <BrowserRouter>
      <SesijaTimer />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<Navigate to="/login" replace />} />

        <Route
          path="/dashboard"
          element={
            <ProtectedPage path="/dashboard">
              <Dashboard />
            </ProtectedPage>
          }
        />
        <Route
          path="/kalendar"
          element={
            <ProtectedPage path="/kalendar">
              <PlaceholderStranica
                naslov="Kalendar termina"
                opis="Prikaz termina u kalendarskom prikazu."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/zakazivanje"
          element={
            <ProtectedPage path="/zakazivanje">
              <PlaceholderStranica
                naslov="Zakazi termin"
                opis="Forma za odabir laboratorija, datuma i trajanja."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/rezervacije"
          element={
            <ProtectedPage path="/rezervacije">
              <PlaceholderStranica
                naslov="Rezervacije"
                opis="Lista aktivnih i proslih rezervacija."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/oprema"
          element={
            <ProtectedPage path="/oprema">
              <PlaceholderStranica
                naslov="Oprema"
                opis="Lista opreme po laboratorijima."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/profil"
          element={
            <ProtectedPage path="/profil">
              <Profil />
            </ProtectedPage>
          }
        />
        <Route
          path="/o-aplikaciji"
          element={
            <ProtectedPage path="/o-aplikaciji">
              <AboutApp />
            </ProtectedPage>
          }
        />
        <Route
          path="/zahtjevi"
          element={
            <ProtectedPage path="/zahtjevi">
              <PlaceholderStranica
                naslov="Zahtjevi studenata"
                opis="Lista zahtjeva s akcijama odobravanja i odbijanja."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/historija"
          element={
            <ProtectedPage path="/historija">
              <PlaceholderStranica
                naslov="Historija studenata"
                opis="Tabelarni prikaz pohadjanja vjezbi."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/termini"
          element={
            <ProtectedPage path="/termini">
              <PlaceholderStranica naslov="Upravljanje terminima" opis="CRUD okruzenje za termine." />
            </ProtectedPage>
          }
        />
        <Route
          path="/kvarovi"
          element={
            <ProtectedPage path="/kvarovi">
              <PlaceholderStranica
                naslov="Kvarovi opreme"
                opis="Pregled prijavljenih kvarova i promjena statusa."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/korisnici"
          element={
            <ProtectedPage path="/korisnici">
              <Korisnici />
            </ProtectedPage>
          }
        />
        <Route
          path="/objekti"
          element={
            <ProtectedPage path="/objekti">
              <PlaceholderStranica
                naslov="Objekti i kabineti"
                opis="CRUD za lokacije, laboratorije i radno vrijeme."
              />
            </ProtectedPage>
          }
        />
        <Route
          path="/pristup-odbijen"
          element={
            <ZasticenaRuta>
              <AccessDenied />
            </ZasticenaRuta>
          }
        />

        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
