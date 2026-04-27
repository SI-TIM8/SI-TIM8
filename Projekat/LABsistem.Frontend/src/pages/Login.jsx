import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import api from "../api/client";

const SESSION_DURATION_MS = 60 * 60 * 1000; // 60 minutes matching backend JWT expiry

function Login() {
  const [isLogin, setIsLogin] = useState(true);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [imePrezime, setImePrezime] = useState("");
  const [greska, setGreska] = useState("");
  const [uspeh, setUspeh] = useState("");
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

  const validatePassword = (pass) => {
    if (pass.length < 8) return "Lozinka mora imati najmanje 8 znakova.";
    if (!/[A-Z]/.test(pass)) return "Lozinka mora imati barem jedno veliko slovo.";
    if (!/[0-9]/.test(pass)) return "Lozinka mora imati barem jedan broj.";
    if (!/[^a-zA-Z0-9]/.test(pass)) return "Lozinka mora imati barem jedan specijalan znak.";
    return null;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setGreska("");
    setUspeh("");

    if (isLogin) {
      if (!username || !password) {
        setGreska("Unesite korisničko ime i lozinku.");
        return;
      }

      try {
        const response = await api.post("/Auth/login", { username, password });
        const { token, role, username: resUsername } = response.data;

        const expiry = Date.now() + SESSION_DURATION_MS;
        localStorage.setItem("token", token);
        localStorage.setItem("tokenExpiry", expiry.toString());
        localStorage.setItem("uloga", role.toLowerCase());
        localStorage.setItem("korisnik", resUsername);

        navigate("/dashboard");
      } catch (err) {
        setGreska(err.response?.data || "Pogrešno korisničko ime ili lozinka.");
      }
    } else {
      if (!username || !password || !email || !imePrezime) {
        setGreska("Sva polja su obavezna.");
        return;
      }

      const passError = validatePassword(password);
      if (passError) {
        setGreska(passError);
        return;
      }

      try {
        await api.post("/Auth/register", {
          imePrezime,
          email,
          username,
          password
        });
        setUspeh("Registracija uspješna! Sada se možete prijaviti.");
        setIsLogin(true);
      } catch (err) {
        setGreska(err.response?.data || "Greška pri registraciji.");
      }
    }
  };

  return (
    <main className="page">
      <div className="login-card">
        <h1>LABsistem</h1>
        <div className="tabs">
          <button 
            className={`tab-btn ${isLogin ? 'active' : ''}`} 
            onClick={() => { setIsLogin(true); setGreska(""); setUspeh(""); }}
          >
            Log in
          </button>
          <button 
            className={`tab-btn ${!isLogin ? 'active' : ''}`} 
            onClick={() => { setIsLogin(false); setGreska(""); setUspeh(""); }}
          >
            Sign in
          </button>
        </div>

        <p className="subtitle">
          {isLogin ? "Prijavite se za pristup sistemu." : "Napravite novi račun kao student."}
        </p>

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

        {uspeh && (
          <p style={{ color: "#16a34a", marginBottom: 16, fontSize: 14 }}>
            {uspeh}
          </p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          {!isLogin && (
            <>
              <div className="form-group">
                <label htmlFor="imePrezime">Ime i Prezime</label>
                <input
                  id="imePrezime"
                  type="text"
                  placeholder="Ime Prezime"
                  value={imePrezime}
                  onChange={(e) => setImePrezime(e.target.value)}
                />
              </div>
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
            </>
          )}

          <div className="form-group">
            <label htmlFor="username">Korisničko ime</label>
            <input
              id="username"
              type="text"
              placeholder="username"
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
            {isLogin ? "Prijavi se" : "Registruj se"}
          </button>
        </form>
      </div>
    </main>
  );
}

export default Login;
