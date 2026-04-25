import { useState, useEffect } from "react";
import { BrowserRouter, Routes, Route, Navigate, useNavigate } from "react-router-dom";
import { pingApi } from "./api/client";
import Login from "./pages/Login";

const SESSION_DURATION_MS = 30 * 60 * 1000; // 30 minuta — mora biti isto kao u Login.jsx
const UPOZORENJE_MS = 5 * 60 * 1000; // upozorenje 5 minuta prije isteka

// Zaštićena ruta — ako nema tokena ili je istekao, šalje na /login
function ZasticenaRuta({ children }) {
  const token = localStorage.getItem("token");
  const expiry = localStorage.getItem("tokenExpiry");
  const aktivna = token && expiry && Date.now() < parseInt(expiry);

  if (!aktivna) {
    localStorage.removeItem("token");
    localStorage.removeItem("tokenExpiry");
    return <Navigate to="/login?sesija=istekla" replace />;
  }

  return children;
}

// Timer koji prati neaktivnost korisnika
function SesijaTimer() {
  const navigate = useNavigate();
  const [upozorenje, setUpozorenje] = useState(false);
  const [preostaloSekundi, setPreostaloSekundi] = useState(null);

  useEffect(() => {
    // Resetuje timer svaki put kada korisnik nešto uradi
    const resetujTimer = () => {
      const noviExpiry = Date.now() + SESSION_DURATION_MS;
      localStorage.setItem("tokenExpiry", noviExpiry.toString());
      setUpozorenje(false);
    };

    // Događaji koji se smatraju aktivnošću
    const dogadjaji = ["mousemove", "keydown", "click", "scroll", "touchstart"];
    dogadjaji.forEach((d) => window.addEventListener(d, resetujTimer));

    // Provjera svakih 1 sekundu
    const interval = setInterval(() => {
      const expiry = localStorage.getItem("tokenExpiry");
      if (!expiry) return;

      const preostalo = parseInt(expiry) - Date.now();

      if (preostalo <= 0) {
        clearInterval(interval);
        localStorage.removeItem("token");
        localStorage.removeItem("tokenExpiry");
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
  }, [navigate]);

  if (!upozorenje) return null;

  return (
    <div style={{
      position: "fixed",
      bottom: 24,
      right: 24,
      background: "#fff3cd",
      border: "1px solid #ffc107",
      borderRadius: 8,
      padding: "12px 16px",
      zIndex: 9999,
      boxShadow: "0 2px 8px rgba(0,0,0,0.15)"
    }}>
      ⚠️ Sesija ističe za <strong>{preostaloSekundi}</strong> sekundi zbog neaktivnosti.
    </div>
  );
}

// Privremena Dashboard stranica dok se ne napravi prava
function Dashboard() {
  const navigate = useNavigate();

  const handleOdjava = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("tokenExpiry");
    navigate("/login");
  };

  return (
    <main className="page">
      <section className="card">
        <h1>Dashboard</h1>
        <p>Dobrodošli u LABsistem. Sesija traje 30 minuta.</p>
        <button className="button" onClick={handleOdjava}>
          Odjavi se
        </button>
      </section>
      <SesijaTimer />
    </main>
  );
}

function Home() {
  const [status, setStatus] = useState("Frontend skeleton je spreman.");
  const [loading, setLoading] = useState(false);

  const handleApiCheck = async () => {
    setLoading(true);
    setStatus("Provjera API konekcije...");
    try {
      const code = await pingApi();
      setStatus(`API odgovor uspjesan (HTTP ${code}).`);
    } catch {
      setStatus("API trenutno nije dostupan.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="page">
      <section className="card">
        <h1>LABsistem Frontend</h1>
        <p>React + Axios skeleton za povezivanje sa ASP.NET Core API-jem.</p>
        <button className="button" type="button" onClick={handleApiCheck} disabled={loading}>
          {loading ? "Provjera..." : "Testiraj API konekciju"}
        </button>
        <p className="status">{status}</p>
      </section>
    </main>
  );
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route
          path="/dashboard"
          element={
            <ZasticenaRuta>
              <Dashboard />
            </ZasticenaRuta>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;