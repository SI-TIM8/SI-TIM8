import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { verifyEmail } from "../api/client";

const INVALID_VERIFICATION_MESSAGE =
  "Verifikacioni link nije validan ili je istekao. Zatražite novi link iz svog profila.";

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

function VerifyEmail() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") || "";

  const [loading, setLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    async function verifyEmailAddress() {
      if (!token) {
        setErrorMessage(INVALID_VERIFICATION_MESSAGE);
        setLoading(false);
        return;
      }

      try {
        const response = await verifyEmail(token);
        setSuccessMessage(
          response.data?.message || "Email adresa je uspješno verifikovana."
        );
      } catch (error) {
        setErrorMessage(
          extractErrorMessage(error, INVALID_VERIFICATION_MESSAGE)
        );
      } finally {
        setLoading(false);
      }
    }

    verifyEmailAddress();
  }, [token]);

  return (
    <main className="auth-page">
      <div className="auth-card login-card forgot-password-card">
        <div className="auth-brand">LABsistem</div>
        <h1 className="login-title auth-card-title">Verifikacija email adrese</h1>
        <p className="auth-description">
          Potvrdite da koristite validnu email adresu za svoj LABsistem nalog.
        </p>

        {loading && (
          <p className="auth-muted-copy">Provjera verifikacionog linka u toku...</p>
        )}
        {!loading && successMessage && (
          <p className="form-success auth-success-note">{successMessage}</p>
        )}
        {!loading && errorMessage && (
          <p className="form-error auth-success-note">{errorMessage}</p>
        )}

        <div className="auth-footer-links">
          <Link className="auth-secondary-link" to="/login">
            Nazad na prijavu
          </Link>
        </div>
      </div>
    </main>
  );
}

export default VerifyEmail;
