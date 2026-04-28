import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Layout from "./components/Layout";
import Profil from "./pages/Profil";
import Korisnici from "./pages/Korisnici";
import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";

const SESSION_DURATION_MS = 30 * 60 * 1000;
const UPOZORENJE_MS = 5 * 60 * 1000;

// ─── Zaštićena ruta ────────────────────────────────
function ZasticenaRuta({ children }) {
  const token = localStorage.getItem("token");
  const expiry = localStorage.getItem("tokenExpiry");
  const aktivna = token && expiry && Date.now() < parseInt(expiry);

  if (!aktivna) {
    localStorage.removeItem("token");
    localStorage.removeItem("tokenExpiry");
    localStorage.removeItem("uloga");
    localStorage.removeItem("korisnik");
    return <Navigate to="/login?sesija=istekla" replace />;
  }

  return children;
}

// ─── Timer neaktivnosti ────────────────────────────
function SesijaTimer() {
  const navigate = useNavigate();
  const [upozorenje, setUpozorenje] = useState(false);
  const [preostaloSekundi, setPreostaloSekundi] = useState(null);

  const resetujTimer = useCallback(() => {
    const expiry = localStorage.getItem("tokenExpiry");
    if (!expiry) return;
    localStorage.setItem("tokenExpiry", (Date.now() + SESSION_DURATION_MS).toString());
    setUpozorenje(false);
  }, []);

  useEffect(() => {
    const dogadjaji = ["mousemove", "keydown", "click", "scroll", "touchstart"];
    dogadjaji.forEach((d) => window.addEventListener(d, resetujTimer));

    const interval = setInterval(() => {
      const expiry = localStorage.getItem("tokenExpiry");
      if (!expiry) return;

      const preostalo = parseInt(expiry) - Date.now();

      if (preostalo <= 0) {
        clearInterval(interval);
        localStorage.removeItem("token");
        localStorage.removeItem("tokenExpiry");
        localStorage.removeItem("uloga");
        localStorage.removeItem("korisnik");
        navigate("/login?sesija=istekla");
      } else if (preostalo <= UPOZORENJE_MS) {
        setUpozorenje(true);
        setPreostaloSekundi(Math.floor(preostalo / 1000));
      }
    }, 1000);

    return () => {
      clearInterval(interval);
      dogadjaji.forEach((d) => window.removeEventListener(d, resetujTimer));
    };
  }, [navigate, resetujTimer]);

  if (!upozorenje) return null;

  return (
    <div className="sesija-upozorenje">
      ⚠️ Sesija ističe za <strong>{preostaloSekundi}</strong> sekundi zbog neaktivnosti.
    </div>
  );
}

// ─── Placeholder stranica ──────────────────────────
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

// ─── App ───────────────────────────────────────────
function App() {
  return (
    <BrowserRouter>
      <SesijaTimer />
      <Routes>
        {/* Javne rute */}
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<Navigate to="/login" replace />} />

        {/* Zaštićene rute */}
        <Route path="/dashboard" element={
          <ZasticenaRuta><Dashboard /></ZasticenaRuta>
        } />
        <Route path="/kalendar" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Kalendar termina" opis="Prikaz termina u kalendarskom prikazu." />
          </ZasticenaRuta>
        } />
        <Route path="/zakazivanje" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Zakaži termin" opis="Forma za odabir laboratorija, datuma i trajanja." />
          </ZasticenaRuta>
        } />
        <Route path="/rezervacije" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Rezervacije" opis="Lista aktivnih i prošlih rezervacija." />
          </ZasticenaRuta>
        } />
        <Route path="/oprema" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Oprema" opis="Lista opreme po laboratorijima." />
          </ZasticenaRuta>
        } />
       <Route path="/profil" element={
        <ZasticenaRuta>
          <Profil />
          </ZasticenaRuta>
        } />
        <Route path="/zahtjevi" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Zahtjevi studenata" opis="Lista zahtjeva s akcijama odobravanja i odbijanja." />
          </ZasticenaRuta>
        } />
        <Route path="/historija" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Historija studenata" opis="Tabelarni prikaz pohađanja vježbi." />
          </ZasticenaRuta>
        } />
        <Route path="/termini" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Upravljanje terminima" opis="CRUD okruženje za termine." />
          </ZasticenaRuta>
        } />
        <Route path="/kvarovi" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Kvarovi opreme" opis="Pregled prijavljenih kvarova i promjena statusa." />
          </ZasticenaRuta>
        } />
        <Route path="/korisnici" element={
          <ZasticenaRuta>
            <Korisnici />
          </ZasticenaRuta>
        } />
        <Route path="/objekti" element={
          <ZasticenaRuta>
            <PlaceholderStranica naslov="Objekti i kabineti" opis="CRUD za lokacije, laboratorije i radno vrijeme." />
          </ZasticenaRuta>
        } />

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;