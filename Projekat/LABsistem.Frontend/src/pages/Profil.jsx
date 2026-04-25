import { useState } from "react";
import Layout from "../components/Layout";

function Profil() {
  const korisnik = localStorage.getItem("korisnik") || "Korisnik";
  const uloga = localStorage.getItem("uloga") || "student";

  const ULOGA_LABELA = {
    student: "Student",
    profesor: "Profesor / Asistent",
    tehnicar: "Laboratorijski tehničar",
    admin: "Administrator",
  };

  const [forma, setForma] = useState({
    ime: korisnik,
    email: `${korisnik}@uni.ba`,
    trenutnaLozinka: "",
    novaLozinka: "",
    potvrda: "",
  });

  const [uspjeh, setUspjeh] = useState("");
  const [greska, setGreska] = useState("");

  const handlePromjena = (e) => {
    setForma({ ...forma, [e.target.name]: e.target.value });
    setUspjeh("");
    setGreska("");
  };

  const handleSacuvaj = (e) => {
    e.preventDefault();
    setGreska("");
    setUspjeh("");

    if (!forma.ime || !forma.email) {
      setGreska("Ime i email su obavezni.");
      return;
    }

    if (forma.novaLozinka && forma.novaLozinka !== forma.potvrda) {
      setGreska("Nova lozinka i potvrda se ne poklapaju.");
      return;
    }

    if (forma.novaLozinka && !forma.trenutnaLozinka) {
      setGreska("Unesite trenutnu lozinku da biste je promijenili.");
      return;
    }

    // TODO: zamijeniti sa pravim API pozivom
    // await api.put("/korisnik/profil", { ime: forma.ime, email: forma.email });

    localStorage.setItem("korisnik", forma.ime);
    setUspjeh("Profil je uspješno ažuriran.");
  };

  return (
    <Layout>
      <div className="page-header">
        <h1>Moj profil</h1>
        <p>Pregledajte i izmijenite vlastite podatke.</p>
      </div>

      <div style={{ display: "grid", gridTemplateColumns: "1fr 2fr", gap: 24 }}>

        {/* Lijeva kolona — info kartica */}
        <div className="card" style={{ textAlign: "center", height: "fit-content" }}>
          <div style={{
            width: 80,
            height: 80,
            borderRadius: "50%",
            background: "linear-gradient(135deg, #0f766e, #0e7490)",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            fontSize: 32,
            color: "white",
            margin: "0 auto 16px",
            fontWeight: 700,
          }}>
            {korisnik.charAt(0).toUpperCase()}
          </div>
          <h2 style={{ fontSize: 18, marginBottom: 6 }}>{forma.ime}</h2>
          <span className="badge plavo">{ULOGA_LABELA[uloga]}</span>

          <div style={{ marginTop: 24, textAlign: "left" }}>
            <p style={{ fontSize: 13, color: "#64748b", marginBottom: 8 }}>
              <strong>Email:</strong> {forma.email}
            </p>
            <p style={{ fontSize: 13, color: "#64748b", marginBottom: 8 }}>
              <strong>Uloga:</strong> {ULOGA_LABELA[uloga]}
            </p>
            <p style={{ fontSize: 13, color: "#64748b" }}>
              <strong>Status:</strong>{" "}
              <span className="badge zeleno">aktivan</span>
            </p>
          </div>
        </div>

        {/* Desna kolona — forma */}
        <div style={{ display: "flex", flexDirection: "column", gap: 24 }}>

          {/* Osnovni podaci */}
          <div className="card">
            <h2 style={{ fontSize: 16, marginBottom: 20 }}>Osnovni podaci</h2>

            {uspjeh && (
              <p style={{ color: "#065f46", background: "#d1fae5", padding: "10px 14px", borderRadius: 8, marginBottom: 16, fontSize: 14 }}>
                ✓ {uspjeh}
              </p>
            )}

            {greska && (
              <p style={{ color: "#dc2626", background: "#fee2e2", padding: "10px 14px", borderRadius: 8, marginBottom: 16, fontSize: 14 }}>
                ⚠️ {greska}
              </p>
            )}

            <form onSubmit={handleSacuvaj} noValidate>
              <div className="form-group">
                <label>Ime i prezime</label>
                <input
                  name="ime"
                  type="text"
                  value={forma.ime}
                  onChange={handlePromjena}
                  placeholder="Vaše ime i prezime"
                />
              </div>

              <div className="form-group">
                <label>Email adresa</label>
                <input
                  name="email"
                  type="email"
                  value={forma.email}
                  onChange={handlePromjena}
                  placeholder="vas@email.com"
                />
              </div>

              <div className="form-group">
                <label>Uloga</label>
                <input
                  type="text"
                  value={ULOGA_LABELA[uloga]}
                  disabled
                  style={{ background: "#f8fafc", color: "#94a3b8", cursor: "not-allowed" }}
                />
              </div>

              <button className="button" type="submit">
                Sačuvaj izmjene
              </button>
            </form>
          </div>

          {/* Promjena lozinke */}
          <div className="card">
            <h2 style={{ fontSize: 16, marginBottom: 20 }}>Promjena lozinke</h2>

            <form onSubmit={handleSacuvaj} noValidate>
              <div className="form-group">
                <label>Trenutna lozinka</label>
                <input
                  name="trenutnaLozinka"
                  type="password"
                  value={forma.trenutnaLozinka}
                  onChange={handlePromjena}
                  placeholder="********"
                />
              </div>

              <div className="form-group">
                <label>Nova lozinka</label>
                <input
                  name="novaLozinka"
                  type="password"
                  value={forma.novaLozinka}
                  onChange={handlePromjena}
                  placeholder="********"
                />
              </div>

              <div className="form-group">
                <label>Potvrda nove lozinke</label>
                <input
                  name="potvrda"
                  type="password"
                  value={forma.potvrda}
                  onChange={handlePromjena}
                  placeholder="********"
                />
              </div>

              <button className="button" type="submit">
                Promijeni lozinku
              </button>
            </form>
          </div>

        </div>
      </div>
    </Layout>
  );
}

export default Profil;