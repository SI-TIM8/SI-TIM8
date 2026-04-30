import { BrowserRouter, Navigate, Route, Routes, useLocation, useNavigate } from "react-router-dom";
import AccessDenied from "./pages/AccessDenied";
import AboutApp from "./pages/AboutApp";
import Dashboard from "./pages/Dashboard";
import Korisnici from "./pages/Korisnici";
import Layout from "./components/Layout";
import Login from "./pages/Login";
import Profil from "./pages/Profil";
import { ALLOWED_ROLES_BY_ROUTE, getCurrentRole } from "./auth/routeAccess";
import { clearSession, hasActiveAccessToken } from "./auth/session";

function ZasticenaRuta({ children, allowedRoles }) {
  const location = useLocation();

  if (!hasActiveAccessToken()) {
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
