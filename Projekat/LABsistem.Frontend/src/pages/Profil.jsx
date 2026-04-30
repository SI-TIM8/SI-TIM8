import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

const FULL_NAME_MIN_LENGTH = 2;
const FULL_NAME_MAX_LENGTH = 100;
const EMAIL_MIN_LENGTH = 5;
const EMAIL_MAX_LENGTH = 254;
const USERNAME_MIN_LENGTH = 3;
const USERNAME_MAX_LENGTH = 30;
const PASSWORD_MIN_LENGTH = 8;
const PASSWORD_MAX_LENGTH = 64;

const ULOGA_LABELA = {
  student: "Student",
  profesor: "Profesor / Asistent",
  tehnicar: "Laboratorijski tehnicar",
  admin: "Administrator",
};

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

function isValidUsername(value) {
  return /^[A-Za-z0-9]+$/.test(value);
}

function isValidFullName(value) {
  return /^\p{L}+(?:\s+\p{L}+)*$/u.test(value);
}

function isValidEmail(value) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
}

function validateProfileField(name, value) {
  const normalizedValue = value.trim();

  if (name === "imePrezime") {
    if (!normalizedValue) {
      return "Ime i prezime je obavezno.";
    }

    if (normalizedValue.length < FULL_NAME_MIN_LENGTH) {
      return "Ime mora imati najmanje 2 karaktera.";
    }

    if (normalizedValue.length > FULL_NAME_MAX_LENGTH) {
      return "Ime i prezime moze imati najvise 100 karaktera.";
    }

    if (!isValidFullName(normalizedValue)) {
      return "Ime i prezime moze sadrzavati samo slova i razmake.";
    }
  }

  if (name === "email") {
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
  }

  if (name === "username") {
    if (!normalizedValue) {
      return "Korisnicko ime je obavezno.";
    }

    if (normalizedValue.length < USERNAME_MIN_LENGTH) {
      return "Korisnicko ime mora imati najmanje 3 karaktera.";
    }

    if (normalizedValue.length > USERNAME_MAX_LENGTH) {
      return "Korisnicko ime moze imati najvise 30 karaktera.";
    }

    if (!isValidUsername(normalizedValue)) {
      return "Korisnicko ime moze sadrzavati samo slova i brojeve, bez razmaka i specijalnih znakova.";
    }
  }

  return "";
}

function getProfileErrors(data) {
  return {
    imePrezime: validateProfileField("imePrezime", data.imePrezime),
    email: validateProfileField("email", data.email),
    username: validateProfileField("username", data.username),
  };
}

function validatePasswordField(name, value, allValues) {
  const normalizedValue = value.trim();

  if (name === "currentPassword") {
    if (!normalizedValue) {
      return "Trenutna lozinka je obavezna.";
    }
  }

  if (name === "newPassword") {
    if (!normalizedValue) {
      return "Nova lozinka je obavezna.";
    }

    if (normalizedValue.length < PASSWORD_MIN_LENGTH) {
      return "Lozinka mora imati najmanje 8 karaktera.";
    }

    if (normalizedValue.length > PASSWORD_MAX_LENGTH) {
      return "Lozinka moze imati najvise 64 karaktera.";
    }
  }

  if (name === "confirmPassword") {
    if (!normalizedValue) {
      return "Potvrda nove lozinke je obavezna.";
    }

    if (normalizedValue !== allValues.newPassword.trim()) {
      return "Nova lozinka i potvrda se ne poklapaju.";
    }
  }

  return "";
}

function getPasswordErrors(data) {
  return {
    currentPassword: validatePasswordField("currentPassword", data.currentPassword, data),
    newPassword: validatePasswordField("newPassword", data.newPassword, data),
    confirmPassword: validatePasswordField("confirmPassword", data.confirmPassword, data),
  };
}

function hasErrors(errors) {
  return Object.values(errors).some(Boolean);
}

function Profil() {
  const [profil, setProfil] = useState(null);
  const [osnovniPodaci, setOsnovniPodaci] = useState({
    imePrezime: "",
    email: "",
    username: "",
  });
  const [lozinkaForma, setLozinkaForma] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [loading, setLoading] = useState(true);
  const [savingProfile, setSavingProfile] = useState(false);
  const [changingPassword, setChangingPassword] = useState(false);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const [uspjeh, setUspjeh] = useState("");
  const [greska, setGreska] = useState("");
  const [passwordSuccess, setPasswordSuccess] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [profileTouched, setProfileTouched] = useState({});
  const [passwordTouched, setPasswordTouched] = useState({});
  const [profileSubmitted, setProfileSubmitted] = useState(false);
  const [passwordSubmitted, setPasswordSubmitted] = useState(false);

  useEffect(() => {
    async function loadProfile() {
      try {
        const response = await api.get("/Auth/profile");
        const fetchedProfile = response.data;

        setProfil(fetchedProfile);
        setOsnovniPodaci({
          imePrezime: fetchedProfile.imePrezime,
          email: fetchedProfile.email,
          username: fetchedProfile.username,
        });
      } catch (error) {
        setGreska(extractErrorMessage(error, "Profil trenutno nije moguce ucitati."));
      } finally {
        setLoading(false);
      }
    }

    loadProfile();
  }, []);

  useEffect(() => {
    if (showPasswordModal) {
      document.body.classList.add("modal-open");
      return () => document.body.classList.remove("modal-open");
    }

    document.body.classList.remove("modal-open");
    return undefined;
  }, [showPasswordModal]);

  const ulogaKlijent = localStorage.getItem("uloga") || "student";
  const prikazanaUloga = useMemo(() => {
    const roleKey = profil?.role?.toLowerCase?.() || ulogaKlijent;
    return ULOGA_LABELA[roleKey] || profil?.role || "Korisnik";
  }, [profil?.role, ulogaKlijent]);
  const profileErrors = useMemo(() => getProfileErrors(osnovniPodaci), [osnovniPodaci]);
  const passwordErrors = useMemo(() => getPasswordErrors(lozinkaForma), [lozinkaForma]);
  const profileFormInvalid = hasErrors(profileErrors);
  const passwordFormInvalid = hasErrors(passwordErrors);

  const handleOsnovniPodaciChange = (e) => {
    const { name, value } = e.target;
    setOsnovniPodaci((current) => ({ ...current, [name]: value }));
    setUspjeh("");
    setGreska("");
  };

  const handleProfileBlur = (e) => {
    const { name } = e.target;
    setProfileTouched((current) => ({ ...current, [name]: true }));
  };

  const handlePasswordChange = (e) => {
    const { name, value } = e.target;
    setLozinkaForma((current) => ({ ...current, [name]: value }));
    setPasswordSuccess("");
    setPasswordError("");
  };

  const handlePasswordBlur = (e) => {
    const { name } = e.target;
    setPasswordTouched((current) => ({ ...current, [name]: true }));
  };

  const closePasswordModal = () => {
    setShowPasswordModal(false);
    setPasswordTouched({});
    setPasswordSubmitted(false);
    setPasswordError("");
    setPasswordSuccess("");
    setLozinkaForma({
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    });
  };

  const handleSacuvaj = async (e) => {
    e.preventDefault();
    setProfileSubmitted(true);
    setProfileTouched({
      imePrezime: true,
      email: true,
      username: true,
    });

    if (profileFormInvalid) {
      setUspjeh("");
      setGreska("Molimo ispravite oznacena polja.");
      return;
    }

    setSavingProfile(true);
    setUspjeh("");
    setGreska("");

    try {
      const response = await api.put("/Auth/profile", {
        imePrezime: osnovniPodaci.imePrezime.trim(),
        email: osnovniPodaci.email.trim(),
        username: osnovniPodaci.username.trim(),
      });
      const updatedProfile = response.data.profile;

      setProfil(updatedProfile);
      setOsnovniPodaci({
        imePrezime: updatedProfile.imePrezime,
        email: updatedProfile.email,
        username: updatedProfile.username,
      });
      localStorage.setItem("korisnik", updatedProfile.username);
      localStorage.setItem("korisnikEmail", updatedProfile.email);
      setUspjeh(response.data.message || "Profil je uspjesno azuriran.");
    } catch (error) {
      setGreska(extractErrorMessage(error, "Doslo je do greske pri cuvanju profila."));
    } finally {
      setSavingProfile(false);
    }
  };

  const handlePromjenaLozinke = async (e) => {
    e.preventDefault();
    setPasswordSubmitted(true);
    setPasswordTouched({
      currentPassword: true,
      newPassword: true,
      confirmPassword: true,
    });

    if (passwordFormInvalid) {
      setPasswordSuccess("");
      setPasswordError("Molimo ispravite oznacena polja.");
      return;
    }

    setChangingPassword(true);
    setPasswordSuccess("");
    setPasswordError("");

    try {
      const response = await api.post("/Auth/change-password", {
        currentPassword: lozinkaForma.currentPassword.trim(),
        newPassword: lozinkaForma.newPassword.trim(),
        confirmPassword: lozinkaForma.confirmPassword.trim(),
      });
      setPasswordSuccess(response.data.message || "Lozinka je uspjesno promijenjena.");
      closePasswordModal();
    } catch (error) {
      setPasswordError(extractErrorMessage(error, "Doslo je do greske pri promjeni lozinke."));
    } finally {
      setChangingPassword(false);
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="page-header">
          <h1>Moj profil</h1>
          <p>Ucitavanje profila u toku...</p>
        </div>
      </Layout>
    );
  }

  if (!profil) {
    return (
      <Layout>
        <div className="page-header">
          <h1>Moj profil</h1>
          <p>{greska || "Profil nije dostupan."}</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="page-header">
        <h1>Moj profil</h1>
        <p>Pregledajte i azurirajte podatke svog korisnickog naloga.</p>
      </div>

      <div className="profil-layout">
        <div className="card profil-summary-card">
          <div className="profil-avatar">
            {profil.imePrezime.charAt(0).toUpperCase()}
          </div>
          <h2>{profil.imePrezime}</h2>
          <p className="profil-username">@{profil.username}</p>
          <span className="badge plavo">{prikazanaUloga}</span>

          <div className="profil-summary-list">
            <p><strong>Email:</strong> {profil.email}</p>
            <p><strong>Uloga:</strong> {prikazanaUloga}</p>
            <p>
              <strong>Status:</strong> <span className="badge zeleno">{profil.status}</span>
            </p>
          </div>
        </div>

        <div className="profil-content">
          <div className="card">
            <h2 style={{ fontSize: 16, marginBottom: 20 }}>Osnovni podaci</h2>

            {uspjeh && <p className="form-success">{uspjeh}</p>}
            {greska && <p className="form-error">{greska}</p>}

            <form onSubmit={handleSacuvaj} noValidate>
              <div className="form-group">
                <label htmlFor="imePrezime">Ime i prezime</label>
                <input
                  id="imePrezime"
                  name="imePrezime"
                  type="text"
                  value={osnovniPodaci.imePrezime}
                  onChange={handleOsnovniPodaciChange}
                  onBlur={handleProfileBlur}
                  minLength={FULL_NAME_MIN_LENGTH}
                  maxLength={FULL_NAME_MAX_LENGTH}
                  className={profileTouched.imePrezime || profileSubmitted ? (profileErrors.imePrezime ? "input-error" : "") : ""}
                />
                {(profileTouched.imePrezime || profileSubmitted) && profileErrors.imePrezime && (
                  <p className="field-error">{profileErrors.imePrezime}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="email">Email adresa</label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  value={osnovniPodaci.email}
                  onChange={handleOsnovniPodaciChange}
                  onBlur={handleProfileBlur}
                  minLength={EMAIL_MIN_LENGTH}
                  maxLength={EMAIL_MAX_LENGTH}
                  className={profileTouched.email || profileSubmitted ? (profileErrors.email ? "input-error" : "") : ""}
                />
                {(profileTouched.email || profileSubmitted) && profileErrors.email && (
                  <p className="field-error">{profileErrors.email}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="username">Korisnicko ime</label>
                <input
                  id="username"
                  name="username"
                  type="text"
                  value={osnovniPodaci.username}
                  onChange={handleOsnovniPodaciChange}
                  onBlur={handleProfileBlur}
                  inputMode="text"
                  minLength={USERNAME_MIN_LENGTH}
                  maxLength={USERNAME_MAX_LENGTH}
                  pattern="[A-Za-z0-9]+"
                  title="Koristite samo slova i brojeve, bez razmaka."
                  className={profileTouched.username || profileSubmitted ? (profileErrors.username ? "input-error" : "") : ""}
                />
                {(profileTouched.username || profileSubmitted) && profileErrors.username && (
                  <p className="field-error">{profileErrors.username}</p>
                )}
              </div>

              <div className="form-group">
                <label>Uloga</label>
                <input
                  type="text"
                  value={prikazanaUloga}
                  disabled
                  style={{ background: "#f8fafc", color: "#94a3b8", cursor: "not-allowed" }}
                />
              </div>

              <div className="profil-actions">
                <button className="button" type="submit" disabled={savingProfile || profileFormInvalid}>
                  {savingProfile ? "Cuvanje..." : "Sačuvaj"}
                </button>
                <button
                  className="button sekundarno"
                  type="button"
                  onClick={() => setShowPasswordModal(true)}
                >
                  Promijeni lozinku
                </button>
              </div>
            </form>
          </div>

          <div className="card">
            <h2 style={{ fontSize: 16, marginBottom: 20 }}>Nedavna aktivnost</h2>
            {profil.recentActivities?.length ? (
              <div className="activity-list">
                {profil.recentActivities.map((activity, index) => (
                  <div key={`${activity.title}-${index}`} className="activity-item">
                    <div className="activity-dot" />
                    <div>
                      <strong>{activity.title}</strong>
                      <p>{activity.description}</p>
                      <span>{activity.meta}</span>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p style={{ color: "#64748b" }}>
                Trenutno nema zabiljezene aktivnosti za prikaz.
              </p>
            )}
          </div>
        </div>
      </div>

      {showPasswordModal && (
        <div
          className="users-modal-overlay"
          onClick={(event) => {
            if (event.target === event.currentTarget) {
              closePasswordModal();
            }
          }}
        >
          <div className="users-modal profil-password-modal">
            <div className="users-modal-header">
              <div>
                <h2>Promijeni lozinku</h2>
                <p>Unesite trenutnu lozinku i postavite novu lozinku za svoj nalog.</p>
              </div>
              <button
                type="button"
                className="users-modal-close"
                onClick={closePasswordModal}
                aria-label="Zatvori promjenu lozinke"
              >
                ×
              </button>
            </div>

            {passwordSuccess && <p className="form-success">{passwordSuccess}</p>}
            {passwordError && <p className="form-error">{passwordError}</p>}

            <form onSubmit={handlePromjenaLozinke} noValidate>
              <div className="form-group">
                <label htmlFor="currentPassword">Trenutna lozinka</label>
                <input
                  id="currentPassword"
                  name="currentPassword"
                  type="password"
                  value={lozinkaForma.currentPassword}
                  onChange={handlePasswordChange}
                  onBlur={handlePasswordBlur}
                  minLength={PASSWORD_MIN_LENGTH}
                  maxLength={PASSWORD_MAX_LENGTH}
                  placeholder="********"
                  className={passwordTouched.currentPassword || passwordSubmitted ? (passwordErrors.currentPassword ? "input-error" : "") : ""}
                />
                {(passwordTouched.currentPassword || passwordSubmitted) && passwordErrors.currentPassword && (
                  <p className="field-error">{passwordErrors.currentPassword}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="newPassword">Nova lozinka</label>
                <input
                  id="newPassword"
                  name="newPassword"
                  type="password"
                  value={lozinkaForma.newPassword}
                  onChange={handlePasswordChange}
                  onBlur={handlePasswordBlur}
                  minLength={PASSWORD_MIN_LENGTH}
                  maxLength={PASSWORD_MAX_LENGTH}
                  placeholder="********"
                  className={passwordTouched.newPassword || passwordSubmitted ? (passwordErrors.newPassword ? "input-error" : "") : ""}
                />
                {(passwordTouched.newPassword || passwordSubmitted) && passwordErrors.newPassword && (
                  <p className="field-error">{passwordErrors.newPassword}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="confirmPassword">Potvrda nove lozinke</label>
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  value={lozinkaForma.confirmPassword}
                  onChange={handlePasswordChange}
                  onBlur={handlePasswordBlur}
                  minLength={PASSWORD_MIN_LENGTH}
                  maxLength={PASSWORD_MAX_LENGTH}
                  placeholder="********"
                  className={passwordTouched.confirmPassword || passwordSubmitted ? (passwordErrors.confirmPassword ? "input-error" : "") : ""}
                />
                {(passwordTouched.confirmPassword || passwordSubmitted) && passwordErrors.confirmPassword && (
                  <p className="field-error">{passwordErrors.confirmPassword}</p>
                )}
              </div>

              <div className="users-field-hint">
                Lozinka mora imati izmedju 8 i 64 karaktera.
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={changingPassword || passwordFormInvalid}>
                  {changingPassword ? "Promjena..." : "Sacuvaj novu lozinku"}
                </button>
                <button
                  className="button sekundarno"
                  type="button"
                  onClick={closePasswordModal}
                  disabled={changingPassword}
                >
                  Odustani
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Layout>
  );
}

export default Profil;
