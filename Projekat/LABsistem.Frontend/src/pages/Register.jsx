import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import api from "../api/client";

const SESSION_DURATION_MS = 30 * 60 * 1000;

const USERNAME_REGEX = /^[a-zA-Z0-9._]{3,20}$/;
const PASSWORD_REGEX = /^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/;

function Register() {
  const [forma, setForma] = useState({
    imePrezime: "",
    email: "",
    username: "",
    password: "",
    potvrdaLozinke: "",
  });
  const [greska, setGreska] = useState("");
  const [ucitava, setUcitava] = useState(false);
  const navigate = useNavigate();

  const handlePromjena = (e) => {
    setForma({ ...forma, [e.target.name]: e.target.value });
    setGreska("");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setGreska("");

    if (!forma.imePrezime || !forma.email || !forma.username || !forma.password || !forma.potvrdaLozinke) {
      setGreska("Sva polja su obavezna.");
      return;
    }

    if (!USERNAME_REGEX.test(forma.username)) {
      setGreska("Username mora imati 3–20 znakova i smije sadržavati samo slova, brojeve, tačku i donju crtu.");
      return;
    }

    if (!PASSWORD_REGEX.test(forma.password)) {
      setGreska("Lozinka mora imati min. 8 znakova, jedno veliko slovo, broj i specijalni znak (npr. !, @, #).");
      return;
    }

    if (forma.password !== forma.potvrdaLozinke) {
      setGreska("Lozinke se ne poklapaju.");
      return;
    }

    setUcitava(true);
    try {
      const response = await api.post("/api/auth/register", {
        imePrezime: forma.imePrezime,
        email: forma.email,
        username: forma.username,
        password: forma.password,
        potvrdaLozinke: forma.potvrdaLozinke,
      });

      const { token, userId, username, role } = response.data;

      const expiry = Date.now() + SESSION_DURATION_MS;
      localStorage.setItem("token", token);
      localStorage.setItem("tokenExpiry", expiry.toString());
      localStorage.setItem("uloga", role.toLowerCase());
      localStorage.setItem("korisnik", username);
      localStorage.setItem("userId", userId.toString());

      navigate("/dashboard");
    } catch (err) {
      const poruka = err.response?.data;
      if (typeof poruka === "string") {
        setGreska(poruka);
      } else {
        setGreska("Registracija nije uspjela. Pokušajte ponovo.");
      }
    } finally {
      setUcitava(false);
    }
  };

  return (
    <main className="page">
      <div className="login-card" style={{ maxWidth: 480 }}>
        <h1>LABsistem</h1>
        <p className="subtitle">Kreirajte novi nalog.</p>

        {greska && (
          <p style={{ color: "#dc2626", marginBottom: 16, fontSize: 14 }}>
            {greska}
          </p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="imePrezime">Ime i prezime</label>
            <input
              id="imePrezime"
              name="imePrezime"
              type="text"
              placeholder="Vaše ime i prezime"
              value={forma.imePrezime}
              onChange={handlePromjena}
              autoComplete="name"
            />
          </div>

          <div className="form-group">
            <label htmlFor="email">Email adresa</label>
            <input
              id="email"
              name="email"
              type="email"
              placeholder="ime@example.com"
              value={forma.email}
              onChange={handlePromjena}
              autoComplete="email"
            />
          </div>

          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              name="username"
              type="text"
              placeholder="vaš_username (3–20 znakova)"
              value={forma.username}
              onChange={handlePromjena}
              autoComplete="username"
            />
            <small style={{ color: "#64748b", fontSize: 12 }}>
              Dozvoljeni znakovi: slova, brojevi, tačka (.), donja crta (_)
            </small>
          </div>

          <div className="form-group">
            <label htmlFor="password">Lozinka</label>
            <input
              id="password"
              name="password"
              type="password"
              placeholder="Min. 8 znakova"
              value={forma.password}
              onChange={handlePromjena}
              autoComplete="new-password"
            />
            <small style={{ color: "#64748b", fontSize: 12 }}>
              Mora sadržavati: veliko slovo, broj, specijalni znak
            </small>
          </div>

          <div className="form-group">
            <label htmlFor="potvrdaLozinke">Potvrdite lozinku</label>
            <input
              id="potvrdaLozinke"
              name="potvrdaLozinke"
              type="password"
              placeholder="Unesite lozinku ponovo"
              value={forma.potvrdaLozinke}
              onChange={handlePromjena}
              autoComplete="new-password"
            />
          </div>

          <button className="button" type="submit" style={{ width: "100%" }} disabled={ucitava}>
            {ucitava ? "Registracija..." : "Registruj se"}
          </button>
        </form>

        <p style={{ textAlign: "center", marginTop: 16, fontSize: 14, color: "#64748b" }}>
          Već imate nalog?{" "}
          <Link to="/login" style={{ color: "#0f766e", fontWeight: 600 }}>
            Prijavite se
          </Link>
        </p>
      </div>
    </main>
  );
}

export default Register;
