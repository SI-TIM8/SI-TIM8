import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import api from "../api/client";

function getTokenExpiry(token) {
  try {
    const [, payload] = token.split(".");
    const decodedPayload = JSON.parse(atob(payload));
    if (!decodedPayload.exp) {
      return null;
    }

    return decodedPayload.exp * 1000;
  } catch {
    return null;
  }
}

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [greska, setGreska] = useState("");
  const navigate = useNavigate();
  const location = useLocation();

  const sesijaIstekla = new URLSearchParams(location.search).get("sesija") === "istekla";

  useEffect(() => {
    const token = localStorage.getItem("token");
    const expiry = localStorage.getItem("tokenExpiry");
    if (token && expiry && Date.now() < parseInt(expiry)) {
      navigate("/dashboard");
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setGreska("");

    if (!username || !password) {
      setGreska("Unesite username i lozinku.");
      return;
    }

    try {
      const response = await api.post("/Auth/login", {
        username,
        password,
      });

      const { token, role, username: authenticatedUsername } = response.data;
      const expiry = getTokenExpiry(token);

      localStorage.setItem("token", token);
      if (expiry) {
        localStorage.setItem("tokenExpiry", expiry.toString());
      } else {
        localStorage.removeItem("tokenExpiry");
      }
      localStorage.setItem("uloga", role.toLowerCase());
      localStorage.setItem("korisnik", authenticatedUsername);

      navigate("/dashboard");
    } catch (error) {
      const backendMessage = error.response?.data;
      setGreska(
        typeof backendMessage === "string"
          ? backendMessage
          : "Prijava nije uspjela. Provjerite username i lozinku."
      );
    }
  };

  return (
    <main className="page">
      <div className="login-card">
        <h1>LABsistem</h1>
        <p className="subtitle">Prijavite se za pristup sistemu.</p>

        {sesijaIstekla && (
          <p style={{ color: "#dc2626", marginBottom: 16, fontSize: 14 }}>
            ⚠️ Vaša sesija je istekla. Prijavite se ponovo.
          </p>
        )}

        {greska && (
          <p style={{ color: "#dc2626", marginBottom: 16, fontSize: 14 }}>
            {greska}
          </p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              placeholder="Unesite username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Lozinka</label>
            <input
              id="password"
              type="password"
              placeholder="********"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button className="button" type="submit" style={{ width: "100%" }}>
            Prijavi se
          </button>
        </form>
      </div>
    </main>
  );
}

export default Login;
