import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import api from "../api/client";
import { hasActiveAccessToken, persistSession } from "../auth/session";

function PasswordVisibilityIcon({ visible }) {
  if (visible) {
    return (
      <svg
        aria-hidden="true"
        viewBox="0 0 24 24"
        className="password-toggle-icon"
        fill="none"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      >
        <path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7Z" />
        <circle cx="12" cy="12" r="3" />
      </svg>
    );
  }

  return (
    <svg
      aria-hidden="true"
      viewBox="0 0 24 24"
      className="password-toggle-icon"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      <path d="m3 3 18 18" />
      <path d="M10.6 10.7A3 3 0 0 0 9 12a3 3 0 0 0 4.3 2.7" />
      <path d="M9.4 5.2A10.8 10.8 0 0 1 12 5c6.5 0 10 7 10 7a17.7 17.7 0 0 1-3 3.8" />
      <path d="M6.7 6.7C4.2 8.3 2.7 11 2 12c0 0 3.5 7 10 7 1.6 0 3-.4 4.3-1" />
    </svg>
  );
}

function Login() {
  const [usernameOrEmail, setUsernameOrEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [greska, setGreska] = useState("");
  const navigate = useNavigate();
  const location = useLocation();

  const sesijaIstekla = new URLSearchParams(location.search).get("sesija") === "istekla";

  useEffect(() => {
    if (hasActiveAccessToken()) {
      navigate("/dashboard");
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setGreska("");

    if (!usernameOrEmail || !password) {
      setGreska("Unesite korisnicko ime ili email adresu i lozinku.");
      return;
    }

    try {
      const response = await api.post("/Auth/login", {
        username: usernameOrEmail,
        password,
      });

      const { username: authenticatedUsername } = response.data;
      persistSession(response.data);
      localStorage.setItem("korisnikEmail", usernameOrEmail.includes("@") ? usernameOrEmail : "");
      localStorage.setItem("korisnik", authenticatedUsername);

      navigate("/dashboard");
    } catch (error) {
      const responseData = error.response?.data;
      const backendMessage =
        typeof responseData === "string"
          ? responseData
          : responseData?.message;

      setGreska(
        backendMessage || "Prijava nije uspjela. Provjerite korisnicko ime ili email adresu i lozinku."
      );
    }
  };

  return (
    <main className="page">
      <div className="login-card">
        <h1 className="login-title">Prijavite se sa svojim LABsistem korisničkim nalogom</h1>

        {sesijaIstekla && (
          <p style={{ color: "#dc2626", marginBottom: 16, fontSize: 14 }}>
            Sesija je istekla. Prijavite se ponovo.
          </p>
        )}

        {greska && (
          <p style={{ color: "#dc2626", marginBottom: 16, fontSize: 14 }}>
            {greska}
          </p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="username">Korisničko ime ili email adresa:</label>
            <input
              id="username"
              type="text"
              placeholder="Unesite korisničko ime ili email adresu"
              value={usernameOrEmail}
              onChange={(e) => setUsernameOrEmail(e.target.value)}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Lozinka:</label>
            <div className="password-field">
              <input
                id="password"
                type={showPassword ? "text" : "password"}
                placeholder="********"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              <button
                type="button"
                className="password-toggle-button"
                aria-label={showPassword ? "Sakrij lozinku" : "Prikazi lozinku"}
                aria-pressed={showPassword}
                onClick={() => setShowPassword((current) => !current)}
              >
                <PasswordVisibilityIcon visible={showPassword} />
              </button>
            </div>
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
