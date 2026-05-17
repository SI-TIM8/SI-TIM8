import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from "react-router-dom";
import AccessDenied from "./pages/AccessDenied";
import AboutApp from "./pages/AboutApp";
import Dashboard from "./pages/Dashboard";
import ForgotPassword from "./pages/ForgotPassword";
import Kalendar from "./pages/Kalendar";
import Korisnici from "./pages/Korisnici";
import Objekti from "./pages/Objekti";
import Layout from "./components/Layout";
import Login from "./pages/Login";
import Profil from "./pages/Profil";
import Oprema from "./pages/Oprema";
import ResetPassword from "./pages/ResetPassword";
import Termini from "./pages/Termini";
import Zakazivanje from "./pages/Zakazivanje";
import Rezervacije from "./pages/Rezervacije";
import Zahtjevi from "./pages/Zahtjevi";
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
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />
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
              <Kalendar />
            </ProtectedPage>
          }
        />
        <Route
          path="/zakazivanje"
          element={
            <ProtectedPage path="/zakazivanje">
              <Zakazivanje />
            </ProtectedPage>
          }
        />
        <Route
          path="/rezervacije"
          element={
            <ProtectedPage path="/rezervacije">
              <Rezervacije />
            </ProtectedPage>
          }
        />
        <Route
          path="/oprema"
          element={
            <ProtectedPage path="/oprema">
              <Oprema />
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
              <Zahtjevi />
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
              <Termini />
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
              <Objekti />
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
