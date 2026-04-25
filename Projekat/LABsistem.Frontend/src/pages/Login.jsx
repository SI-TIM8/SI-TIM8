import { useState } from "react";
import { Link } from "react-router-dom";

function Login() {
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    setSubmitted(true);
  };

  return (
    <main className="page">
      <section className="card">
        <h1>Prijava</h1>
        <p>Unesi svoje podatke za pristup LABsistem-u.</p>

        <form onSubmit={handleSubmit} noValidate={false}>
          <div style={{ marginBottom: 12 }}>
            <label htmlFor="email">Email</label>
            <input
              id="email"
              name="email"
              type="email"
              required
              placeholder="ime@example.com"
              style={{ width: "100%", padding: 8, marginTop: 4 }}
            />
          </div>

          <div style={{ marginBottom: 12 }}>
            <label htmlFor="password">Lozinka</label>
            <input
              id="password"
              name="password"
              type="password"
              required
              placeholder="********"
              style={{ width: "100%", padding: 8, marginTop: 4 }}
            />
          </div>

          <button className="button" type="submit">Prijavi se</button>
        </form>

        {submitted && (
          <p className="status">Forma je validna.</p>
        )}

        <p style={{ marginTop: 16 }}>
          <Link to="/">← Nazad na početnu</Link>
        </p>
      </section>
    </main>
  );
}

export default Login;