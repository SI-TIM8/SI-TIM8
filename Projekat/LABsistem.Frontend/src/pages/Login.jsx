import { useState, useEffect } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import api from "../api/client";

const SESSION_DURATION_MS = 30 * 60 * 1000;

const PASSWORD_REGEX = /^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/;

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [greska, setGreska] = useState("");
  const [ucitava, setUcitava] = useState(false);
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

    if (!PASSWORD_REGEX.test(password)) {
      setGreska("Lozinka mora imati min. 8 znakova, jedno veliko slovo, broj i specijalni znak.");
      return;
    }

    setUcitava(true);
    try {
      const response = await api.post("/api/auth/login", { username, password });
      const { token, userId, username: korisnikUsername, role } = response.data;

      const expiry = Date.now() + SESSION_DURATION_MS;
      localStorage.setItem("token", token);
      localStorage.setItem("tokenExpiry", expiry.toString());
      localStorage.setItem("uloga", role.toLowerCase());
      localStorage.setItem("korisnik", korisnikUsername);
      localStorage.setItem("userId", userId.toString());

      navigate("/dashboard");
    } catch (err) {
      const poruka = err.response?.data;
      if (typeof poruka === "string") {
        setGreska(poruka);
      } else {
        setGreska("Pogrešni kredencijali. Pokušajte ponovo.");
      }
    } finally {
      setUcitava(false);
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
              placeholder="vaš_username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              autoComplete="username"
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
              autoComplete="current-password"
            />
          </div>

          <button className="button" type="submit" style={{ width: "100%" }} disabled={ucitava}>
            {ucitava ? "Prijavljivanje..." : "Prijavi se"}
          </button>
        </form>

        <p style={{ textAlign: "center", marginTop: 16, fontSize: 14, color: "#64748b" }}>
          Nemate nalog?{" "}
          <Link to="/register" style={{ color: "#0f766e", fontWeight: 600 }}>
            Registrujte se
          </Link>
        </p>
      </div>
    </main>
  );
}

export default Login;