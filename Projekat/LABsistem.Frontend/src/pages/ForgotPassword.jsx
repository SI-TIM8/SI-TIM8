import { useState } from "react";
import { Link } from "react-router-dom";
import { forgotPassword } from "../api/client";

const EMAIL_MIN_LENGTH = 5;
const EMAIL_MAX_LENGTH = 254;
const GENERIC_SUCCESS_MESSAGE =
  "Ako nalog sa ovim emailom postoji, poslali smo instrukcije za resetovanje lozinke.";
const FORGOT_PASSWORD_DESCRIPTION =
  "Unesite email adresu povezanu sa va\u0161im nalogom i poslat \u0107emo vam link za resetovanje lozinke.";
const FORGOT_PASSWORD_SUBMIT_LABEL = "Po\u0161alji link za resetovanje";
const FORGOT_PASSWORD_ERROR_FALLBACK =
  "Trenutno nije mogu\u0107e poslati link za resetovanje lozinke.";

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

function isValidEmail(value) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
}

function validateEmail(value) {
  const normalizedValue = value.trim();

  if (!normalizedValue) {
    return "Email je obavezan.";
  }

  if (normalizedValue.length < EMAIL_MIN_LENGTH) {
    return "Email mora imati najmanje 5 karaktera.";
  }

  if (normalizedValue.length > EMAIL_MAX_LENGTH) {
    return "Email moze imati najvise 254 karaktera.";
  }

  if (!isValidEmail(normalizedValue)) {
    return "Email nije ispravan.";
  }

  return "";
}

function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [touched, setTouched] = useState(false);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const emailError = validateEmail(email);

  async function handleSubmit(event) {
    event.preventDefault();
    setTouched(true);
    setSubmitted(true);
    setErrorMessage("");

    if (emailError) {
      return;
    }

    setLoading(true);

    try {
      await forgotPassword(email.trim());
      setSuccessMessage(GENERIC_SUCCESS_MESSAGE);
    } catch (error) {
      setErrorMessage(
        extractErrorMessage(error, FORGOT_PASSWORD_ERROR_FALLBACK)
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="auth-page">
      <div className="auth-card login-card forgot-password-card">
        <div className="auth-brand">LABsistem</div>
        <h1 className="login-title auth-card-title">Zaboravljena lozinka</h1>
        <p className="auth-description">{FORGOT_PASSWORD_DESCRIPTION}</p>

        {successMessage && <p className="form-success auth-success-note">{successMessage}</p>}
        {errorMessage && <p className="form-error auth-success-note">{errorMessage}</p>}

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="forgot-email">Email adresa</label>
            <input
              id="forgot-email"
              type="email"
              placeholder="korisnik@example.com"
              value={email}
              onChange={(event) => {
                setEmail(event.target.value);
                setErrorMessage("");
              }}
              onBlur={() => setTouched(true)}
              minLength={EMAIL_MIN_LENGTH}
              maxLength={EMAIL_MAX_LENGTH}
              className={touched || submitted ? (emailError ? "input-error" : "") : ""}
            />
            {(touched || submitted) && emailError && (
              <p className="field-error">{emailError}</p>
            )}
          </div>

          <button className="button" type="submit" style={{ width: "100%" }} disabled={loading}>
            {loading ? "Slanje..." : FORGOT_PASSWORD_SUBMIT_LABEL}
          </button>
        </form>

        <div className="auth-footer-links">
          <Link className="auth-secondary-link" to="/login">
            Nazad na prijavu
          </Link>
        </div>
      </div>
    </main>
  );
}

export default ForgotPassword;
