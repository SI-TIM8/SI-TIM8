import Layout from "../components/Layout";

function AboutApp() {
  return (
    <Layout>
      <div className="page-header">
        <h1>O aplikaciji</h1>
        <p>LABsistem je centralno mjesto za upravljanje laboratorijskim resursima i terminima.</p>
      </div>

      <div className="about-app-grid">
        <section className="card about-app-section">
          <h2>Namjena sistema</h2>
          <p>
            LABsistem olaksava organizaciju laboratorijskih termina, pristup opremi
            i pregled aktivnosti korisnika na jednom mjestu. Aplikacija je
            prilagodjena studentima, profesorima, tehnicarima i administratorima.
          </p>
        </section>

        <section className="card about-app-section">
          <h2>Sta korisnici mogu raditi</h2>
          <ul className="about-app-list">
            <li>pregledati dostupne termine i resurse</li>
            <li>upravljati rezervacijama i zahtjevima</li>
            <li>pratiti profil i nedavne aktivnosti</li>
            <li>administrirati korisnike, objekte i raspolozivost sistema</li>
          </ul>
        </section>

        <section className="card about-app-section">
          <h2>Organizacija rada</h2>
          <p>
            Svaka uloga u sistemu dobija prilagodjen meni i pristup samo onim
            stranicama koje su joj potrebne. Time se radni tok pojednostavljuje,
            a pregled sistema ostaje jasan i fokusiran.
          </p>
        </section>

        <section className="card about-app-section">
          <h2>Sigurnost i pristup</h2>
          <p>
            Prijava koristi JWT autentifikaciju, a odjava odmah ponistava aktivni
            token. Pristup rutama je kontrolisan po ulogama kako bi svaka sekcija
            bila dostupna samo odgovarajucim korisnicima.
          </p>
        </section>
      </div>
    </Layout>
  );
}

export default AboutApp;
