import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";

const SESSION_DURATION_MS = 30 * 60 * 1000;

function Login() {
  const [email, setEmail] = useState("");
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

  const handleSubmit = (e) => {
    e.preventDefault();
    setGreska("");

    if (!email || !password) {
      setGreska("Unesite email i lozinku.");
      return;
    }

    // TODO: zamijeniti sa pravim API pozivom
    // const response = await api.post("/korisnik/login", { email, password });
    // const { token, uloga, ime } = response.data;

    const mockToken = "mock-jwt-token-12345";
    const expiry = Date.now() + SESSION_DURATION_MS;

    localStorage.setItem("token", mockToken);
    localStorage.setItem("tokenExpiry", expiry.toString());
    localStorage.setItem("uloga", "student"); // privremeno
    localStorage.setItem("korisnik", email.split("@")[0]);

    navigate("/dashboard");
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
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              placeholder="ime@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
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