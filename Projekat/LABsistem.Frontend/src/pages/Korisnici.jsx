import { useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

function extractErrorMessage(error, fallbackMessage) {
  const responseData = error?.response?.data;

  if (typeof responseData === "string" && responseData.trim()) {
    return responseData;
  }

  if (typeof responseData?.message === "string" && responseData.message.trim()) {
    return responseData.message;
  }

  return fallbackMessage;
}

function Korisnici() {
  const [imePrezime, setImePrezime] = useState("");
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [uloga, setUloga] = useState("2");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ type: "", text: "" });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage({ type: "", text: "" });

    try {
      await api.post(`/Auth/create-user?uloga=${uloga}`, {
        imePrezime,
        email,
        username,
        password,
      });

      setMessage({ type: "success", text: "Korisnik uspjesno kreiran." });
      setImePrezime("");
      setEmail("");
      setUsername("");
      setPassword("");
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Greska pri kreiranju korisnika."),
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje korisnicima</h1>
        <p>Dodajte nove profesore ili tehnicare u sistem.</p>
      </div>

      <div className="card" style={{ maxWidth: "600px" }}>
        <h2>Kreiraj novog korisnika</h2>
        {message.text && (
          <p
            style={{
              color: message.type === "success" ? "#16a34a" : "#dc2626",
              marginBottom: 16,
              fontSize: 14,
              fontWeight: "500",
            }}
          >
            {message.text}
          </p>
        )}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Ime i prezime</label>
            <input
              type="text"
              value={imePrezime}
              onChange={(e) => setImePrezime(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Korisnicko ime</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Lozinka</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              placeholder="Min. 8 znakova, veliko slovo, broj i znak"
            />
          </div>

          <div className="form-group">
            <label>Uloga</label>
            <select value={uloga} onChange={(e) => setUloga(e.target.value)}>
              <option value="2">Profesor / Asistent</option>
              <option value="4">Lab. Tehnicar</option>
            </select>
          </div>

          <button className="button" type="submit" disabled={loading}>
            {loading ? "Kreiranje..." : "Kreiraj korisnika"}
          </button>
        </form>
      </div>
    </Layout>
  );
}

export default Korisnici;
