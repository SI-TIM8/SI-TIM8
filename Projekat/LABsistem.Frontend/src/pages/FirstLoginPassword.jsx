import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/client";
import {
  clearSession,
  getRefreshToken,
  persistPasswordChangeRequirement,
} from "../auth/session";

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

function FirstLoginPassword() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    newPassword: "",
    confirmPassword: "",
  });
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  function handleChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  async function handleBackToLogin() {
    try {
      await api.post("/Auth/logout", { refreshToken: getRefreshToken() });
    } catch {
      // Best effort logout; local session cleanup still returns the user to login.
    } finally {
      clearSession();
      navigate("/login", { replace: true });
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setErrorMessage("");
    setSuccessMessage("");

    if (!form.newPassword || !form.confirmPassword) {
      setErrorMessage("Oba polja za novu lozinku su obavezna.");
      return;
    }

    setSubmitting(true);

    try {
      const response = await api.post("/Auth/change-password", {
        newPassword: form.newPassword,
        confirmPassword: form.confirmPassword,
      });

      persistPasswordChangeRequirement(false);
      setSuccessMessage(response.data?.message || "Lozinka je uspješno promijenjena.");
      setForm({
        newPassword: "",
        confirmPassword: "",
      });

      setTimeout(() => {
        navigate("/dashboard", { replace: true });
      }, 500);
    } catch (error) {
      setErrorMessage(
        extractErrorMessage(error, "Došlo je do greške pri promjeni lozinke.")
      );
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="auth-page">
      <div className="auth-card login-card forgot-password-card">
        <div className="auth-brand">LABsistem</div>
        <h1 className="login-title auth-card-title">Prva promjena lozinke</h1>
        <p className="auth-description">
          Već ste prijavljeni privremenom lozinkom koju je postavio administrator.
          Prije nastavka rada sada samo postavite svoju novu lozinku.
        </p>

        {errorMessage && (
          <p className="form-error auth-success-note">{errorMessage}</p>
        )}
        {successMessage && (
          <p className="form-success auth-success-note">{successMessage}</p>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="newPassword">Nova lozinka</label>
            <div className="password-field">
              <input
                id="newPassword"
                name="newPassword"
                type={showNewPassword ? "text" : "password"}
                value={form.newPassword}
                onChange={handleChange}
                autoComplete="new-password"
                placeholder="Unesite novu lozinku"
              />
              <button
                type="button"
                className="password-toggle-button"
                aria-label={showNewPassword ? "Sakrij lozinku" : "Prikaži lozinku"}
                aria-pressed={showNewPassword}
                onClick={() => setShowNewPassword((current) => !current)}
              >
                <PasswordVisibilityIcon visible={showNewPassword} />
              </button>
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Potvrda nove lozinke</label>
            <div className="password-field">
              <input
                id="confirmPassword"
                name="confirmPassword"
                type={showConfirmPassword ? "text" : "password"}
                value={form.confirmPassword}
                onChange={handleChange}
                autoComplete="new-password"
                placeholder="Ponovite novu lozinku"
              />
              <button
                type="button"
                className="password-toggle-button"
                aria-label={showConfirmPassword ? "Sakrij lozinku" : "Prikaži lozinku"}
                aria-pressed={showConfirmPassword}
                onClick={() => setShowConfirmPassword((current) => !current)}
              >
                <PasswordVisibilityIcon visible={showConfirmPassword} />
              </button>
            </div>
          </div>

          <button
            className="button"
            type="submit"
            style={{ width: "100%" }}
            disabled={submitting}
          >
            {submitting ? "Promjena..." : "Sačuvaj novu lozinku"}
          </button>

          <div className="auth-link-row" style={{ justifyContent: "center", marginTop: 16 }}>
            <button
              type="button"
              className="auth-inline-link"
              style={{ background: "none", border: "none", cursor: "pointer" }}
              onClick={handleBackToLogin}
            >
              Nazad na prijavu
            </button>
          </div>
        </form>
      </div>
    </main>
  );
}

export default FirstLoginPassword;
