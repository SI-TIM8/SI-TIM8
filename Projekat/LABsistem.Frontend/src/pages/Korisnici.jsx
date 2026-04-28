import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";

const ROLE_OPTIONS = [
  { key: "student", label: "Student", value: "3" },
  { key: "profesor", label: "Profesor / Asistent", value: "2" },
  { key: "tehnicar", label: "Tehničar", value: "4" },
  { key: "admin", label: "Admin", value: "1" },
];

const ROLE_LABELS = {
  Student: "Student",
  Profesor: "Profesor / Asistent",
  Tehnicar: "Tehničar",
  Admin: "Admin",
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

const INITIAL_FORM_STATE = {
  imePrezime: "",
  email: "",
  username: "",
  password: "",
  uloga: "2",
};

function isValidUsername(value) {
  return /^[A-Za-z0-9]+$/.test(value);
}

function Korisnici() {
  const [users, setUsers] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [activeRole, setActiveRole] = useState("");
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [creatingUser, setCreatingUser] = useState(false);
  const [formState, setFormState] = useState(INITIAL_FORM_STATE);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [createMessage, setCreateMessage] = useState({ type: "", text: "" });

  useEffect(() => {
    loadUsers();
  }, []);

  useEffect(() => {
    if (!modalOpen) {
      return undefined;
    }

    const handleEscape = (event) => {
      if (event.key === "Escape") {
        closeModal();
      }
    };

    window.addEventListener("keydown", handleEscape);
    document.body.classList.add("modal-open");

    return () => {
      window.removeEventListener("keydown", handleEscape);
      document.body.classList.remove("modal-open");
    };
  }, [modalOpen]);

  async function loadUsers() {
    setLoading(true);
    setMessage({ type: "", text: "" });

    try {
      const response = await api.get("/Auth/users");
      setUsers(response.data);
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Korisnike trenutno nije moguće učitati."),
      });
    } finally {
      setLoading(false);
    }
  }

  function openModal() {
    setCreateMessage({ type: "", text: "" });
    setFormState(INITIAL_FORM_STATE);
    setModalOpen(true);
  }

  function closeModal() {
    setModalOpen(false);
    setCreatingUser(false);
    setCreateMessage({ type: "", text: "" });
    setFormState(INITIAL_FORM_STATE);
  }

  function handleRoleChipClick(roleKey) {
    setActiveRole((currentRole) => (currentRole === roleKey ? "" : roleKey));
  }

  function handleFormChange(event) {
    const { name, value } = event.target;
    setFormState((current) => ({ ...current, [name]: value }));
    setCreateMessage({ type: "", text: "" });
  }

  async function handleCreateUser(event) {
    event.preventDefault();
    setCreatingUser(true);
    setCreateMessage({ type: "", text: "" });

    if (!isValidUsername(formState.username.trim())) {
      setCreateMessage({
        type: "error",
        text: "Korisničko ime može sadržavati samo slova i brojeve, bez razmaka i specijalnih znakova.",
      });
      setCreatingUser(false);
      return;
    }

    try {
      await api.post(`/Auth/create-user?uloga=${formState.uloga}`, {
        imePrezime: formState.imePrezime,
        email: formState.email,
        username: formState.username,
        password: formState.password,
      });

      setCreateMessage({ type: "success", text: "Korisnik je uspješno kreiran." });
      await loadUsers();

      setTimeout(() => {
        closeModal();
      }, 500);
    } catch (error) {
      setCreateMessage({
        type: "error",
        text: extractErrorMessage(error, "Greška pri kreiranju korisnika."),
      });
    } finally {
      setCreatingUser(false);
    }
  }

  const filteredUsers = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLowerCase();

    return users.filter((user) => {
      const normalizedRole = (user.role || "").toLowerCase();
      const roleMatches = !activeRole || normalizedRole === activeRole;

      const searchMatches =
        !normalizedSearch ||
        user.imePrezime?.toLowerCase().includes(normalizedSearch) ||
        user.email?.toLowerCase().includes(normalizedSearch) ||
        user.username?.toLowerCase().includes(normalizedSearch);

      return roleMatches && searchMatches;
    });
  }, [activeRole, searchTerm, users]);

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje korisnicima</h1>
      </div>

      <div className="users-page">
        <div className="card users-toolbar">
          <button className="button users-create-button" type="button" onClick={openModal}>
            <span className="users-create-icon">+</span>
            Dodaj novog korisnika
          </button>

          <div className="users-filter-group" role="group" aria-label="Filteri po ulozi">
            {ROLE_OPTIONS.map((role) => {
              const isActive = activeRole === role.key;

              return (
                <button
                  key={role.key}
                  type="button"
                  className={`users-chip${isActive ? " active" : ""}`}
                  onClick={() => handleRoleChipClick(role.key)}
                >
                  {role.label}
                </button>
              );
            })}
          </div>

          <div className="users-search">
            <span className="users-search-icon" aria-hidden="true" />
            <input
              type="text"
              value={searchTerm}
              onChange={(event) => setSearchTerm(event.target.value)}
              placeholder="Pretražite po imenu/mailu"
              aria-label="Pretraga korisnika"
            />
          </div>
        </div>

        <div className="card users-list-card">
          {message.text && (
            <p className={message.type === "error" ? "form-error" : "form-success"}>
              {message.text}
            </p>
          )}

          <div className="users-list-header users-list-row">
            <span>Ime i prezime</span>
            <span>Email</span>
            <span>Korisničko ime</span>
            <span>Uloga</span>
          </div>

          {loading ? (
            <div className="users-empty-state">Učitavanje korisnika u toku...</div>
          ) : filteredUsers.length ? (
            <div className="users-list">
              {filteredUsers.map((user) => (
                <div className="users-list-row users-list-item" key={user.userId}>
                  <span>{user.imePrezime}</span>
                  <span>{user.email}</span>
                  <span>{user.username}</span>
                  <span>
                    <span className="badge sivo">
                      {ROLE_LABELS[user.role] || user.role}
                    </span>
                  </span>
                </div>
              ))}
            </div>
          ) : (
            <div className="users-empty-state">
              Nema korisnika koji odgovaraju odabranim filterima.
            </div>
          )}
        </div>
      </div>

      {modalOpen && (
        <div
          className="users-modal-overlay"
          role="presentation"
          onClick={closeModal}
        >
          <div
            className="users-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="create-user-title"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="users-modal-header">
              <div>
                <h2 id="create-user-title">Dodaj novog korisnika</h2>
                <p>Popunite osnovne podatke i odaberite ulogu korisnika.</p>
              </div>
              <button
                type="button"
                className="users-modal-close"
                aria-label="Zatvori modal"
                onClick={closeModal}
              >
                ×
              </button>
            </div>

            {createMessage.text && (
              <p className={createMessage.type === "error" ? "form-error" : "form-success"}>
                {createMessage.text}
              </p>
            )}

            <form onSubmit={handleCreateUser} noValidate>
              <div className="form-group">
                <label htmlFor="imePrezime">Ime i prezime</label>
                <input
                  id="imePrezime"
                  name="imePrezime"
                  type="text"
                  value={formState.imePrezime}
                  onChange={handleFormChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="email">Email adresa</label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  value={formState.email}
                  onChange={handleFormChange}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="username">Korisničko ime</label>
                <input
                  id="username"
                  name="username"
                  type="text"
                  value={formState.username}
                  onChange={handleFormChange}
                  inputMode="text"
                  pattern="[A-Za-z0-9]+"
                  title="Koristite samo slova i brojeve, bez razmaka."
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="password">Lozinka</label>
                <input
                  id="password"
                  name="password"
                  type="password"
                  value={formState.password}
                  onChange={handleFormChange}
                  required
                  placeholder="Min. 8 znakova, veliko slovo, broj i specijalan znak"
                />
              </div>

              <div className="form-group">
                <label htmlFor="uloga">Uloga</label>
                <select
                  id="uloga"
                  name="uloga"
                  value={formState.uloga}
                  onChange={handleFormChange}
                >
                  {ROLE_OPTIONS.map((role) => (
                    <option key={role.value} value={role.value}>
                      {role.label}
                    </option>
                  ))}
                </select>
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={creatingUser}>
                  {creatingUser ? "Kreiranje..." : "Kreiraj korisnika"}
                </button>
                <button className="button sekundarno" type="button" onClick={closeModal}>
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

export default Korisnici;
