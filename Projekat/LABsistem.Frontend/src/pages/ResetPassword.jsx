import { useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { resetPassword, verifyResetToken } from "../api/client";

const PASSWORD_MIN_LENGTH = 8;
const PASSWORD_MAX_LENGTH = 64;
const INVALID_RESET_TOKEN_MESSAGE =
  "Link za resetovanje lozinke nije validan ili je istekao. Zatra\u017eite novi link.";
const RESET_PASSWORD_SUCCESS_MESSAGE =
  "Lozinka je uspje\u0161no promijenjena. Sada se mo\u017eete prijaviti.";
const RESET_PASSWORD_DESCRIPTION =
  "Unesite novu lozinku za svoj nalog i potvrdite je prije \u010duvanja promjene.";
const RESET_PASSWORD_REQUIREMENTS =
  "Lozinka mora imati izme\u0111u 8 i 64 karaktera.";
const REQUEST_NEW_LINK_LABEL = "Zatra\u017ei novi link";
const SHOW_PASSWORD_LABEL = "Prika\u017ei lozinku";
const HIDE_PASSWORD_LABEL = "Sakrij lozinku";
const RESET_PASSWORD_VALIDATION_MESSAGE =
  "Molimo ispravite ozna\u010dena polja.";

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

function validateNewPassword(value) {
  const normalizedValue = value.trim();

  if (!normalizedValue) {
    return "Nova lozinka je obavezna.";
  }

  if (normalizedValue.length < PASSWORD_MIN_LENGTH) {
    return "Lozinka mora imati najmanje 8 karaktera.";
  }

  if (normalizedValue.length > PASSWORD_MAX_LENGTH) {
    return "Lozinka moze imati najvise 64 karaktera.";
  }

  return "";
}

function validateConfirmPassword(password, confirmPassword) {
  const normalizedConfirmPassword = confirmPassword.trim();

  if (!normalizedConfirmPassword) {
    return "Potvrda nove lozinke je obavezna.";
  }

  if (password.trim() !== normalizedConfirmPassword) {
    return "Nova lozinka i potvrda se ne poklapaju.";
  }

  return "";
}

function ResetPassword() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") || "";

  const [form, setForm] = useState({
    newPassword: "",
    confirmPassword: "",
  });
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [touched, setTouched] = useState({});
  const [submitted, setSubmitted] = useState(false);
  const [verifying, setVerifying] = useState(true);
  const [tokenValid, setTokenValid] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const errors = useMemo(
    () => ({
      newPassword: validateNewPassword(form.newPassword),
      confirmPassword: validateConfirmPassword(
        form.newPassword,
        form.confirmPassword
      ),
    }),
    [form.confirmPassword, form.newPassword]
  );

  useEffect(() => {
    async function verifyTokenRequest() {
      if (!token) {
        setTokenValid(false);
        setErrorMessage(INVALID_RESET_TOKEN_MESSAGE);
        setVerifying(false);
        return;
      }

      try {
        const response = await verifyResetToken(token);
        if (response.data?.valid) {
          setTokenValid(true);
          setErrorMessage("");
        } else {
          setTokenValid(false);
          setErrorMessage(response.data?.message || INVALID_RESET_TOKEN_MESSAGE);
        }
      } catch (error) {
        setTokenValid(false);
        setErrorMessage(extractErrorMessage(error, INVALID_RESET_TOKEN_MESSAGE));
      } finally {
        setVerifying(false);
      }
    }

    verifyTokenRequest();
  }, [token]);

  function handleChange(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
    setSuccessMessage("");
  }

  function handleBlur(event) {
    const { name } = event.target;
    setTouched((current) => ({ ...current, [name]: true }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setSubmitted(true);
    setTouched({
      newPassword: true,
      confirmPassword: true,
    });
    setSuccessMessage("");

    if (!tokenValid) {
      setErrorMessage(INVALID_RESET_TOKEN_MESSAGE);
      return;
    }

    if (errors.newPassword || errors.confirmPassword) {
      setErrorMessage(RESET_PASSWORD_VALIDATION_MESSAGE);
      return;
    }

    setSubmitting(true);
    setErrorMessage("");

    try {
      const response = await resetPassword(
        token,
        form.newPassword.trim(),
        form.confirmPassword.trim()
      );

      setSuccessMessage(response.data?.message || RESET_PASSWORD_SUCCESS_MESSAGE);
      setTokenValid(false);
      setForm({
        newPassword: "",
        confirmPassword: "",
      });
    } catch (error) {
      setErrorMessage(extractErrorMessage(error, INVALID_RESET_TOKEN_MESSAGE));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="auth-page">
      <div className="auth-card login-card forgot-password-card">
        <div className="auth-brand">LABsistem</div>
        <h1 className="login-title auth-card-title">Postavi novu lozinku</h1>
        <p className="auth-description">{RESET_PASSWORD_DESCRIPTION}</p>

        {verifying && (
          <p className="auth-muted-copy">Provjera reset linka u toku...</p>
        )}
        {successMessage && (
          <p className="form-success auth-success-note">{successMessage}</p>
        )}
        {!successMessage && errorMessage && (
          <p className="form-error auth-success-note">{errorMessage}</p>
        )}

        {!verifying && !successMessage && tokenValid && (
          <form onSubmit={handleSubmit} noValidate>
            <div className="form-group">
              <label htmlFor="newPassword">Nova lozinka</label>
              <div className="password-field">
                <input
                  id="newPassword"
                  name="newPassword"
                  type={showNewPassword ? "text" : "password"}
                  placeholder="********"
                  value={form.newPassword}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  minLength={PASSWORD_MIN_LENGTH}
                  maxLength={PASSWORD_MAX_LENGTH}
                  className={
                    touched.newPassword || submitted
                      ? errors.newPassword
                        ? "input-error"
                        : ""
                      : ""
                  }
                />
                <button
                  type="button"
                  className="password-toggle-button"
                  aria-label={showNewPassword ? HIDE_PASSWORD_LABEL : SHOW_PASSWORD_LABEL}
                  aria-pressed={showNewPassword}
                  onClick={() => setShowNewPassword((current) => !current)}
                >
                  <PasswordVisibilityIcon visible={showNewPassword} />
                </button>
              </div>
              {(touched.newPassword || submitted) && errors.newPassword && (
                <p className="field-error">{errors.newPassword}</p>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="confirmPassword">Potvrda nove lozinke</label>
              <div className="password-field">
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type={showConfirmPassword ? "text" : "password"}
                  placeholder="********"
                  value={form.confirmPassword}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  minLength={PASSWORD_MIN_LENGTH}
                  maxLength={PASSWORD_MAX_LENGTH}
                  className={
                    touched.confirmPassword || submitted
                      ? errors.confirmPassword
                        ? "input-error"
                        : ""
                      : ""
                  }
                />
                <button
                  type="button"
                  className="password-toggle-button"
                  aria-label={
                    showConfirmPassword ? HIDE_PASSWORD_LABEL : SHOW_PASSWORD_LABEL
                  }
                  aria-pressed={showConfirmPassword}
                  onClick={() => setShowConfirmPassword((current) => !current)}
                >
                  <PasswordVisibilityIcon visible={showConfirmPassword} />
                </button>
              </div>
              {(touched.confirmPassword || submitted) && errors.confirmPassword && (
                <p className="field-error">{errors.confirmPassword}</p>
              )}
            </div>

            <div className="users-field-hint">{RESET_PASSWORD_REQUIREMENTS}</div>

            <button
              className="button"
              type="submit"
              style={{ width: "100%", marginTop: "14px" }}
              disabled={submitting}
            >
              {submitting ? "Resetovanje..." : "Resetuj lozinku"}
            </button>
          </form>
        )}

        <div className="auth-footer-links">
          <Link className="auth-secondary-link" to="/forgot-password">
            {REQUEST_NEW_LINK_LABEL}
          </Link>
          <Link className="auth-secondary-link" to="/login">
            Nazad na prijavu
          </Link>
        </div>
      </div>
    </main>
  );
}

export default ResetPassword;
