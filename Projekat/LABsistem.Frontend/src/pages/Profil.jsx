import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

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
  const [showPasswordForm, setShowPasswordForm] = useState(false);
  const [uspjeh, setUspjeh] = useState("");
  const [greska, setGreska] = useState("");
  const [passwordSuccess, setPasswordSuccess] = useState("");
  const [passwordError, setPasswordError] = useState("");

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

  const ulogaKlijent = localStorage.getItem("uloga") || "student";
  const prikazanaUloga = useMemo(() => {
    const roleKey = profil?.role?.toLowerCase?.() || ulogaKlijent;
    return ULOGA_LABELA[roleKey] || profil?.role || "Korisnik";
  }, [profil?.role, ulogaKlijent]);

  const handleOsnovniPodaciChange = (e) => {
    const { name, value } = e.target;
    setOsnovniPodaci((current) => ({ ...current, [name]: value }));
    setUspjeh("");
    setGreska("");
  };

  const handlePasswordChange = (e) => {
    const { name, value } = e.target;
    setLozinkaForma((current) => ({ ...current, [name]: value }));
    setPasswordSuccess("");
    setPasswordError("");
  };

  const handleSacuvaj = async (e) => {
    e.preventDefault();
    setSavingProfile(true);
    setUspjeh("");
    setGreska("");

    if (!isValidUsername(osnovniPodaci.username.trim())) {
      setGreska("Korisničko ime može sadržavati samo slova i brojeve, bez razmaka i specijalnih znakova.");
      setSavingProfile(false);
      return;
    }

    try {
      const response = await api.put("/Auth/profile", osnovniPodaci);
      const updatedProfile = response.data.profile;

      setProfil(updatedProfile);
      setOsnovniPodaci({
        imePrezime: updatedProfile.imePrezime,
        email: updatedProfile.email,
        username: updatedProfile.username,
      });
      localStorage.setItem("korisnik", updatedProfile.username);
      setUspjeh(response.data.message || "Profil je uspjesno azuriran.");
    } catch (error) {
      setGreska(extractErrorMessage(error, "Doslo je do greske pri cuvanju profila."));
    } finally {
      setSavingProfile(false);
    }
  };

  const handlePromjenaLozinke = async (e) => {
    e.preventDefault();
    setChangingPassword(true);
    setPasswordSuccess("");
    setPasswordError("");

    try {
      const response = await api.post("/Auth/change-password", lozinkaForma);
      setPasswordSuccess(response.data.message || "Lozinka je uspjesno promijenjena.");
      setLozinkaForma({
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
      setShowPasswordForm(false);
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
                />
              </div>

              <div className="form-group">
                <label htmlFor="email">Email adresa</label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  value={osnovniPodaci.email}
                  onChange={handleOsnovniPodaciChange}
                />
              </div>

              <div className="form-group">
                <label htmlFor="username">Korisnicko ime</label>
                <input
                  id="username"
                  name="username"
                  type="text"
                  value={osnovniPodaci.username}
                  onChange={handleOsnovniPodaciChange}
                  inputMode="text"
                  pattern="[A-Za-z0-9]+"
                  title="Koristite samo slova i brojeve, bez razmaka."
                />
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
                <button className="button" type="submit" disabled={savingProfile}>
                  {savingProfile ? "Cuvanje..." : "Sacuvaj izmjene"}
                </button>
                <button
                  className="button sekundarno"
                  type="button"
                  onClick={() => setShowPasswordForm((current) => !current)}
                >
                  {showPasswordForm ? "Zatvori promjenu lozinke" : "Promijeni lozinku"}
                </button>
              </div>
            </form>
          </div>

          {showPasswordForm && (
            <div className="card">
              <h2 style={{ fontSize: 16, marginBottom: 20 }}>Promjena lozinke</h2>

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
                    placeholder="********"
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="newPassword">Nova lozinka</label>
                  <input
                    id="newPassword"
                    name="newPassword"
                    type="password"
                    value={lozinkaForma.newPassword}
                    onChange={handlePasswordChange}
                    placeholder="********"
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="confirmPassword">Potvrda nove lozinke</label>
                  <input
                    id="confirmPassword"
                    name="confirmPassword"
                    type="password"
                    value={lozinkaForma.confirmPassword}
                    onChange={handlePasswordChange}
                    placeholder="********"
                  />
                </div>

                <button className="button" type="submit" disabled={changingPassword}>
                  {changingPassword ? "Promjena..." : "Sacuvaj novu lozinku"}
                </button>
              </form>
            </div>
          )}

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
    </Layout>
  );
}

export default Profil;
