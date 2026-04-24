import { useState } from "react";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { pingApi } from "./api/client";
import Login from "./pages/Login";

function Home() {
  const [status, setStatus] = useState("Frontend skeleton je spreman.");
  const [loading, setLoading] = useState(false);

  const handleApiCheck = async () => {
    setLoading(true);
    setStatus("Provjera API konekcije...");

    try {
      const code = await pingApi();
      setStatus(`API odgovor uspjesan (HTTP ${code}).`);
    } catch (error) {
      setStatus("API trenutno nije dostupan. Provjeri da li je backend pokrenut na definisanom URL-u.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="page">
      <section className="card">
        <h1>LABsistem Frontend</h1>
        <p>
          React + Axios skeleton za povezivanje sa ASP.NET Core API-jem.
        </p>

        <button className="button" type="button" onClick={handleApiCheck} disabled={loading}>
          {loading ? "Provjera..." : "Testiraj API konekciju"}
        </button>

        <p className="status">{status}</p>

        <p style={{ marginTop: 16 }}>
          <Link to="/login">Idi na login →</Link>
        </p>
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
      </Routes>
    </BrowserRouter>
  );
}

export default App;
