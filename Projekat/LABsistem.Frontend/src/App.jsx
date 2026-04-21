import { useState } from "react";
import { pingApi } from "./api/client";

function App() {
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
      </section>
    </main>
  );
}

export default App;
