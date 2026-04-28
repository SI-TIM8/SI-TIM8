import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
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
  const [usernameOrEmail, setUsernameOrEmail] = useState("");
  const [password, setPassword] = useState("");
  const [greska, setGreska] = useState("");
  const navigate = useNavigate();
  const location = useLocation();

  const sesijaIstekla = new URLSearchParams(location.search).get("sesija") === "istekla";

  useEffect(() => {
    const token = localStorage.getItem("token");
    const expiry = localStorage.getItem("tokenExpiry");
    if (token && expiry && Date.now() < Number.parseInt(expiry, 10)) {
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
      localStorage.setItem("korisnikEmail", usernameOrEmail.includes("@") ? usernameOrEmail : "");

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
