import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";

const SESSION_DURATION_MS = 30 * 60 * 1000; // 30 minuta

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [greska, setGreska] = useState("");
  const navigate = useNavigate();
  const location = useLocation();

  // Prikaži poruku ako je sesija istekla
  const sesijaIstekla = new URLSearchParams(location.search).get("sesija") === "istekla";

  useEffect(() => {
    // Ako je već prijavljen, idi na dashboard
    const token = localStorage.getItem("token");
    const expiry = localStorage.getItem("tokenExpiry");
    if (token && expiry && Date.now() < parseInt(expiry)) {
      navigate("/dashboard");
    }
  }, [navigate]);

  const handleSubmit = (e) => {
    e.preventDefault();
    setGreska("");

    // TODO: zamijeniti sa pravim API pozivom kada backend bude gotov
    // const response = await api.post("/korisnik/login", { email, password });
    // const { token } = response.data;

    if (email && password) {
      // Mock token dok backend nije gotov
      const mockToken = "mock-jwt-token-12345";
      const expiry = Date.now() + SESSION_DURATION_MS;

      localStorage.setItem("token", mockToken);
      localStorage.setItem("tokenExpiry", expiry.toString());

      navigate("/dashboard");
    } else {
      setGreska("Unesite email i lozinku.");
    }
  };

  return (
    <main className="page">
      <section className="card">
        <h1>Prijava</h1>
        <p>Unesi svoje podatke za pristup LABsistem-u.</p>

        {sesijaIstekla && (
          <p style={{ color: "red", marginBottom: 12 }}>
            ⚠️ Vaša sesija je istekla. Prijavite se ponovo.
          </p>
        )}

        {greska && (
          <p style={{ color: "red", marginBottom: 12 }}>{greska}</p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div style={{ marginBottom: 12 }}>
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              required
              placeholder="ime@example.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              style={{ width: "100%", padding: 8, marginTop: 4 }}
            />
          </div>

          <div style={{ marginBottom: 12 }}>
            <label htmlFor="password">Lozinka</label>
            <input
              id="password"
              type="password"
              required
              placeholder="********"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              style={{ width: "100%", padding: 8, marginTop: 4 }}
            />
          </div>

          <button className="button" type="submit">Prijavi se</button>
        </form>
      </section>
    </main>
  );
}

export default Login;