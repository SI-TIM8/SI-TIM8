import { useEffect, useMemo, useState } from "react";
import Layout from "../components/Layout";
import api from "../api/client";
import { getCurrentUserId } from "../auth/session";

const ROLE_OPTIONS = [
  { key: "student", label: "Student", value: "3", apiRole: "Student" },
  { key: "profesor", label: "Profesor / Asistent", value: "2", apiRole: "Profesor" },
  { key: "tehnicar", label: "Tehnicar", value: "4", apiRole: "Tehnicar" },
  { key: "admin", label: "Admin", value: "1", apiRole: "Admin" },
];

const STATUS_OPTIONS = [
  { key: "all", label: "Svi" },
  { key: "active", label: "Aktivni" },
  { key: "inactive", label: "Deaktivirani" },
];

const ROLE_LABELS = {
  Student: "Student",
  Profesor: "Profesor / Asistent",
  Tehnicar: "Tehnicar",
  Admin: "Admin",
};

const INITIAL_FORM_STATE = {
  imePrezime: "",
  email: "",
  username: "",
  newPassword: "",
  uloga: "2",
};

const INITIAL_TOUCHED_STATE = {
  imePrezime: false,
  email: false,
  username: false,
  uloga: false,
  newPassword: false,
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

function getRoleKey(role) {
  return (role || "").toLowerCase();
}

function getRoleValueFromApiRole(role) {
  return ROLE_OPTIONS.find((option) => option.apiRole === role)?.value || "2";
}

function getRoleLabel(role) {
  return ROLE_LABELS[role] || role;
}

function getUserStatusLabel(user) {
  return user?.isActive ? "Aktivan" : "Deaktiviran";
}

function validateUserForm(formState, modalMode) {
  const errors = {};
  const fullName = formState.imePrezime.trim();
  const email = formState.email.trim();
  const username = formState.username.trim();
  const password = formState.newPassword;

  if (!fullName) {
    errors.imePrezime = "Ime i prezime je obavezno";
  } else if (fullName.length < 2) {
    errors.imePrezime = "Ime mora imati najmanje 2 karaktera";
  } else if (fullName.length > 100) {
    errors.imePrezime = "Ime i prezime moze imati najvise 100 karaktera";
  } else if (!isValidFullName(fullName)) {
    errors.imePrezime = "Dozvoljena su samo slova i razmaci";
  }

  if (!email) {
    errors.email = "Email adresa je obavezna";
  } else if (email.length < 5) {
    errors.email = "Email mora imati najmanje 5 karaktera";
  } else if (email.length > 254) {
    errors.email = "Email moze imati najvise 254 karaktera";
  } else if (!isValidEmail(email)) {
    errors.email = "Email nije ispravan";
  }

  if (!username) {
    errors.username = "Korisnicko ime je obavezno";
  } else if (username.length < 3) {
    errors.username = "Korisnicko ime mora imati najmanje 3 karaktera";
  } else if (username.length > 30) {
    errors.username = "Korisnicko ime moze imati najvise 30 karaktera";
  } else if (!isValidUsername(username)) {
    errors.username = "Dozvoljena su samo slova i brojevi bez razmaka";
  }

  if (!formState.uloga || !ROLE_OPTIONS.some((role) => role.value === formState.uloga)) {
    errors.uloga = "Uloga je obavezna";
  }

  if (modalMode === "create") {
    if (!password.trim()) {
      errors.newPassword = "Lozinka je obavezna";
    } else if (password.trim().length < 8) {
      errors.newPassword = "Lozinka mora imati najmanje 8 karaktera";
    } else if (password.trim().length > 64) {
      errors.newPassword = "Lozinka moze imati najvise 64 karaktera";
    }
  } else if (password.trim() && password.trim().length < 8) {
    errors.newPassword = "Lozinka mora imati najmanje 8 karaktera";
  } else if (password.trim() && password.trim().length > 64) {
    errors.newPassword = "Lozinka moze imati najvise 64 karaktera";
  }

  return errors;
}

function Korisnici() {
  const [users, setUsers] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [activeRole, setActiveRole] = useState("");
  const [statusFilter, setStatusFilter] = useState("active");
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState("create");
  const [editingUser, setEditingUser] = useState(null);
  const [savingUser, setSavingUser] = useState(false);
  const [formState, setFormState] = useState(INITIAL_FORM_STATE);
  const [formTouched, setFormTouched] = useState(INITIAL_TOUCHED_STATE);
  const [message, setMessage] = useState({ type: "", text: "" });
  const [formMessage, setFormMessage] = useState({ type: "", text: "" });
  const [confirmState, setConfirmState] = useState({
    open: false,
    type: "",
    payload: null,
  });
  const [hasSubmitted, setHasSubmitted] = useState(false);

  const currentUserId = getCurrentUserId();
  const currentUsername = localStorage.getItem("korisnik");
  const formErrors = useMemo(() => validateUserForm(formState, modalMode), [formState, modalMode]);
  const isFormSubmittable = Object.keys(formErrors).length === 0;

  useEffect(() => {
    loadUsers();
  }, []);

  useEffect(() => {
    if (!modalOpen && !confirmState.open) {
      document.body.classList.remove("modal-open");
      return undefined;
    }

    const handleEscape = (event) => {
      if (event.key === "Escape") {
        if (confirmState.open) {
          closeConfirm();
          return;
        }

        closeModal();
      }
    };

    window.addEventListener("keydown", handleEscape);
    document.body.classList.add("modal-open");

    return () => {
      window.removeEventListener("keydown", handleEscape);
      document.body.classList.remove("modal-open");
    };
  }, [modalOpen, confirmState.open]);

  async function loadUsers() {
    setLoading(true);
    setMessage({ type: "", text: "" });

    try {
      const response = await api.get("/Auth/users");
      setUsers(response.data);
    } catch (error) {
      setMessage({
        type: "error",
        text: extractErrorMessage(error, "Korisnike trenutno nije moguce ucitati."),
      });
    } finally {
      setLoading(false);
    }
  }

  function resetFormState() {
    setFormState(INITIAL_FORM_STATE);
    setFormTouched(INITIAL_TOUCHED_STATE);
    setFormMessage({ type: "", text: "" });
    setHasSubmitted(false);
  }

  function openCreateModal() {
    setModalMode("create");
    setEditingUser(null);
    resetFormState();
    setModalOpen(true);
  }

  function openEditModal(user) {
    setModalMode("edit");
    setEditingUser(user);
    setFormTouched(INITIAL_TOUCHED_STATE);
    setFormMessage({ type: "", text: "" });
    setHasSubmitted(false);
    setFormState({
      imePrezime: user.imePrezime,
      email: user.email,
      username: user.username,
      newPassword: "",
      uloga: getRoleValueFromApiRole(user.role),
    });
    setModalOpen(true);
  }

  function closeModal() {
    setModalOpen(false);
    setSavingUser(false);
    setEditingUser(null);
    resetFormState();
  }

  function openConfirm(type, payload) {
    setConfirmState({ open: true, type, payload });
  }

  function closeConfirm() {
    setConfirmState({ open: false, type: "", payload: null });
  }

  function handleRoleChipClick(roleKey) {
    setActiveRole((currentRole) => (currentRole === roleKey ? "" : roleKey));
  }

  function handleStatusChipClick(statusKey) {
    setStatusFilter(statusKey);
  }

  function handleFormChange(event) {
    const { name, value } = event.target;
    setFormState((current) => ({ ...current, [name]: value }));
    setFormMessage({ type: "", text: "" });
  }

  function handleFieldBlur(event) {
    const { name } = event.target;
    setFormTouched((current) => ({ ...current, [name]: true }));
  }

  function isCurrentUser(user) {
    return (
      (currentUserId !== null && user.userId === currentUserId) ||
      (currentUsername && user.username === currentUsername)
    );
  }

  function getEditTooltip(user) {
    if (isCurrentUser(user)) {
      return "Ne mozete uredjivati vlastiti nalog kroz ovaj panel.";
    }

    return "Uredi korisnika";
  }

  function getDeactivateTooltip(user) {
    if (isCurrentUser(user)) {
      return "Ne mozete deaktivirati svoj nalog.";
    }

    if (user.role === "Admin" && user.isActive) {
      return "Prvo uklonite administratorsku ulogu prije deaktivacije korisnika.";
    }

    return user.isActive ? "Deaktiviraj korisnika" : "Aktiviraj korisnika";
  }

  function shouldShowFieldError(fieldName) {
    return Boolean(formErrors[fieldName]) && (formTouched[fieldName] || hasSubmitted);
  }

  async function submitCreate() {
    await api.post(`/Auth/create-user?uloga=${formState.uloga}`, {
      imePrezime: formState.imePrezime.trim(),
      email: formState.email.trim(),
      username: formState.username.trim(),
      password: formState.newPassword,
    });

    setFormMessage({ type: "success", text: "Korisnik je uspjesno kreiran." });
    await loadUsers();
    setTimeout(() => {
      closeModal();
    }, 500);
  }

  async function submitEdit() {
    const response = await api.put(`/Auth/users/${editingUser.userId}`, {
      imePrezime: formState.imePrezime.trim(),
      email: formState.email.trim(),
      username: formState.username.trim(),
      uloga: Number.parseInt(formState.uloga, 10),
      newPassword: formState.newPassword,
    });

    setFormMessage({
      type: "success",
      text: response.data.message || "Korisnik je uspjesno azuriran.",
    });
    await loadUsers();
    setTimeout(() => {
      closeModal();
    }, 500);
  }

  async function handleCreateOrEdit(event) {
    event.preventDefault();
    setHasSubmitted(true);
    setFormTouched({
      imePrezime: true,
      email: true,
      username: true,
      uloga: true,
      newPassword: true,
    });
    setSavingUser(true);
    setFormMessage({ type: "", text: "" });

    if (Object.keys(formErrors).length > 0) {
      setFormMessage({
        type: "error",
        text: "Molimo ispravite oznacena polja.",
      });
      setSavingUser(false);
      return;
    }

    if (modalMode === "edit" && editingUser) {
      const previousRoleValue = getRoleValueFromApiRole(editingUser.role);
      const wasAdmin = previousRoleValue === "1";
      const willBeAdmin = formState.uloga === "1";

      if (!wasAdmin && willBeAdmin) {
        openConfirm("promote-admin", {
          user: editingUser,
          submit: submitEdit,
        });
        setSavingUser(false);
        return;
      }

      if (wasAdmin && !willBeAdmin) {
        openConfirm("demote-admin", {
          user: editingUser,
          submit: submitEdit,
        });
        setSavingUser(false);
        return;
      }
    }

    try {
      if (modalMode === "create") {
        await submitCreate();
      } else {
        await submitEdit();
      }
    } catch (error) {
      setFormMessage({
        type: "error",
        text: extractErrorMessage(
          error,
          modalMode === "create"
            ? "Greska pri kreiranju korisnika."
            : "Greska pri azuriranju korisnika."
        ),
      });
    } finally {
      setSavingUser(false);
    }
  }

  async function handleConfirmAction() {
    const { type, payload } = confirmState;
    if (!payload) {
      closeConfirm();
      return;
    }

    try {
      if (type === "deactivate") {
        const response = await api.post(`/Auth/users/${payload.user.userId}/deactivate`);
        setMessage({ type: "success", text: response.data.message || "Korisnik je deaktiviran." });
        await loadUsers();
      } else if (type === "activate") {
        const response = await api.post(`/Auth/users/${payload.user.userId}/activate`);
        setMessage({ type: "success", text: response.data.message || "Korisnik je aktiviran." });
        await loadUsers();
      } else if (type === "promote-admin" || type === "demote-admin") {
        await payload.submit();
      }
    } catch (error) {
      const fallbackMessage =
        type === "deactivate"
          ? "Greska pri deaktivaciji korisnika."
          : type === "activate"
            ? "Greska pri aktivaciji korisnika."
            : "Greska pri cuvanju izmjena.";

      if (type === "promote-admin" || type === "demote-admin") {
        setFormMessage({
          type: "error",
          text: extractErrorMessage(error, fallbackMessage),
        });
      } else {
        setMessage({
          type: "error",
          text: extractErrorMessage(error, fallbackMessage),
        });
      }
    } finally {
      closeConfirm();
    }
  }

  function handleDeactivateOrActivate(user) {
    if (user.isActive) {
      openConfirm("deactivate", { user });
      return;
    }

    openConfirm("activate", { user });
  }

  const filteredUsers = useMemo(() => {
    const normalizedSearch = searchTerm.trim().toLowerCase();

    return users.filter((user) => {
      const normalizedRole = getRoleKey(user.role);
      const roleMatches = !activeRole || normalizedRole === activeRole;

      const statusMatches =
        statusFilter === "all" ||
        (statusFilter === "active" && user.isActive) ||
        (statusFilter === "inactive" && !user.isActive);

      const searchMatches =
        !normalizedSearch ||
        user.imePrezime?.toLowerCase().includes(normalizedSearch) ||
        user.email?.toLowerCase().includes(normalizedSearch) ||
        user.username?.toLowerCase().includes(normalizedSearch);

      return roleMatches && statusMatches && searchMatches;
    });
  }, [activeRole, searchTerm, statusFilter, users]);

  const confirmationContent = useMemo(() => {
    if (!confirmState.open || !confirmState.payload?.user) {
      if (confirmState.type === "promote-admin") {
        return {
          title: "Dodijeliti administratorsku ulogu?",
          description: "Da li ste sigurni da zelite dodijeliti administratorsku ulogu ovom korisniku?",
          actionLabel: "Potvrdi",
          buttonClass: "button",
        };
      }

      if (confirmState.type === "demote-admin") {
        return {
          title: "Ukloniti administratorsku ulogu?",
          description: "Da li ste sigurni da zelite ukloniti administratorsku ulogu ovom korisniku?",
          actionLabel: "Potvrdi",
          buttonClass: "button opasno",
        };
      }

      return null;
    }

    const { user } = confirmState.payload;

    if (confirmState.type === "deactivate") {
      return {
        title: "Deaktivirati korisnika?",
        description: `Korisnik "${user.imePrezime}" ce izgubiti pristup sistemu. Svi njegovi aktivni refresh tokeni ce biti opozvani i vise se nece moci prijaviti.`,
        actionLabel: "Deaktiviraj korisnika",
        buttonClass: "button opasno",
      };
    }

    if (confirmState.type === "activate") {
      return {
        title: "Aktivirati korisnika?",
        description: `Korisniku "${user.imePrezime}" ce ponovo biti omogucen pristup sistemu. Za nastavak rada morat ce se ponovo prijaviti.`,
        actionLabel: "Aktiviraj korisnika",
        buttonClass: "button",
      };
    }

    if (confirmState.type === "promote-admin") {
      return {
        title: "Dodijeliti administratorsku ulogu?",
        description: "Da li ste sigurni da zelite dodijeliti administratorsku ulogu ovom korisniku?",
        actionLabel: "Sacuvaj izmjene",
        buttonClass: "button",
      };
    }

    return {
      title: "Ukloniti administratorsku ulogu?",
      description: "Da li ste sigurni da zelite ukloniti administratorsku ulogu ovom korisniku?",
      actionLabel: "Sacuvaj izmjene",
      buttonClass: "button opasno",
    };
  }, [confirmState]);

  return (
    <Layout>
      <div className="page-header">
        <h1>Upravljanje korisnicima</h1>
      </div>

      <div className="users-page">
        <div className="card users-toolbar">
          <button className="button users-create-button" type="button" onClick={openCreateModal}>
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

          <div className="users-status-filter-group" role="group" aria-label="Filteri po statusu">
            {STATUS_OPTIONS.map((status) => {
              const isSelected = statusFilter === status.key;
              const statusClass =
                status.key === "active"
                  ? " status-active"
                  : status.key === "inactive"
                    ? " status-inactive"
                    : " status-all";

              return (
                <button
                  key={status.key}
                  type="button"
                  className={`users-chip users-status-chip${statusClass}${isSelected ? " active" : ""}`}
                  onClick={() => handleStatusChipClick(status.key)}
                >
                  {status.label}
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
              placeholder="Pretrazite po imenu/mailu"
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
            <span>Korisnicko ime</span>
            <span>Uloga</span>
            <span>Status</span>
            <span>Akcije</span>
          </div>

          {loading ? (
            <div className="users-empty-state">Ucitavanje korisnika u toku...</div>
          ) : filteredUsers.length ? (
            <div className="users-list">
              {filteredUsers.map((user) => {
                const selfUser = isCurrentUser(user);
                const editDisabled = selfUser;
                const deactivateDisabled = selfUser || (user.role === "Admin" && user.isActive);

                return (
                  <div
                    className={`users-list-row users-list-item${user.isActive ? "" : " is-muted"}`}
                    key={user.userId}
                  >
                    <span>{user.imePrezime}</span>
                    <span>{user.email}</span>
                    <span>{user.username}</span>
                    <span>
                      <span className="badge sivo">
                        {getRoleLabel(user.role)}
                      </span>
                    </span>
                    <span>
                      <span className={`badge ${user.isActive ? "zeleno" : "crveno"}`}>
                        {getUserStatusLabel(user)}
                      </span>
                    </span>
                    <span>
                      <div className="users-actions">
                        <button
                          type="button"
                          className="users-action-btn"
                          title={getEditTooltip(user)}
                          disabled={editDisabled}
                          onClick={() => openEditModal(user)}
                        >
                          <span aria-hidden="true">✎</span>
                          Uredi
                        </button>
                        <button
                          type="button"
                          className={`users-action-btn ${user.isActive ? "warn" : "success"}`}
                          title={getDeactivateTooltip(user)}
                          disabled={deactivateDisabled}
                          onClick={() => handleDeactivateOrActivate(user)}
                        >
                          <span aria-hidden="true">{user.isActive ? "⛔" : "↺"}</span>
                          {user.isActive ? "Deaktiviraj" : "Aktiviraj"}
                        </button>
                      </div>
                    </span>
                  </div>
                );
              })}
            </div>
          ) : (
            <div className="users-empty-state">
              Nema korisnika koji odgovaraju odabranim filterima.
            </div>
          )}
        </div>
      </div>

      {modalOpen && (
        <div className="users-modal-overlay" role="presentation" onClick={closeModal}>
          <div
            className="users-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="manage-user-title"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="users-modal-header">
              <div>
                <h2 id="manage-user-title">
                  {modalMode === "create" ? "Dodaj novog korisnika" : "Uredi korisnika"}
                </h2>
                <p>
                  {modalMode === "create"
                    ? "Popunite osnovne podatke i odaberite ulogu korisnika."
                    : "Azurirajte osnovne podatke korisnika. Lozinku mijenjate samo ako unesete novu vrijednost."}
                </p>
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

            {formMessage.text && (
              <p className={formMessage.type === "error" ? "form-error" : "form-success"}>
                {formMessage.text}
              </p>
            )}

            <form onSubmit={handleCreateOrEdit} noValidate>
              <div className="form-group">
                <label htmlFor="imePrezime">Ime i prezime</label>
                <input
                  id="imePrezime"
                  name="imePrezime"
                  type="text"
                  value={formState.imePrezime}
                  onChange={handleFormChange}
                  onBlur={handleFieldBlur}
                  className={shouldShowFieldError("imePrezime") ? "input-error" : ""}
                  minLength={2}
                  maxLength={100}
                  required
                />
                {shouldShowFieldError("imePrezime") && (
                  <p className="field-error">{formErrors.imePrezime}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="email">Email adresa</label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  value={formState.email}
                  onChange={handleFormChange}
                  onBlur={handleFieldBlur}
                  className={shouldShowFieldError("email") ? "input-error" : ""}
                  minLength={5}
                  maxLength={254}
                  required
                />
                {shouldShowFieldError("email") && (
                  <p className="field-error">{formErrors.email}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="username">Korisnicko ime</label>
                <input
                  id="username"
                  name="username"
                  type="text"
                  value={formState.username}
                  onChange={handleFormChange}
                  onBlur={handleFieldBlur}
                  inputMode="text"
                  className={shouldShowFieldError("username") ? "input-error" : ""}
                  minLength={3}
                  maxLength={30}
                  required
                />
                {shouldShowFieldError("username") && (
                  <p className="field-error">{formErrors.username}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="uloga">Uloga</label>
                <select
                  id="uloga"
                  name="uloga"
                  value={formState.uloga}
                  onChange={handleFormChange}
                  onBlur={handleFieldBlur}
                  className={shouldShowFieldError("uloga") ? "input-error" : ""}
                >
                  {ROLE_OPTIONS.map((role) => (
                    <option key={role.value} value={role.value}>
                      {role.label}
                    </option>
                  ))}
                </select>
                {shouldShowFieldError("uloga") && (
                  <p className="field-error">{formErrors.uloga}</p>
                )}
              </div>

              <div className="form-group">
                <label htmlFor="newPassword">
                  {modalMode === "create" ? "Lozinka" : "Nova lozinka (opcionalno)"}
                </label>
                <input
                  id="newPassword"
                  name="newPassword"
                  type="password"
                  value={formState.newPassword}
                  onChange={handleFormChange}
                  onBlur={handleFieldBlur}
                  className={shouldShowFieldError("newPassword") ? "input-error" : ""}
                  required={modalMode === "create"}
                  minLength={modalMode === "create" ? 8 : undefined}
                  maxLength={64}
                  placeholder={
                    modalMode === "create"
                      ? "Minimalno 8 karaktera"
                      : "Ostavite prazno ako ne zelite promijeniti lozinku"
                  }
                />
                {modalMode === "edit" && !shouldShowFieldError("newPassword") && (
                  <p className="users-field-hint">
                    Ostavite prazno ako ne zelite promijeniti lozinku.
                  </p>
                )}
                {shouldShowFieldError("newPassword") && (
                  <p className="field-error">{formErrors.newPassword}</p>
                )}
              </div>

              <div className="users-modal-actions">
                <button className="button" type="submit" disabled={savingUser || !isFormSubmittable}>
                  {savingUser
                    ? modalMode === "create"
                      ? "Kreiranje..."
                      : "Cuvanje..."
                    : modalMode === "create"
                      ? "Kreiraj korisnika"
                      : "Sacuvaj izmjene"}
                </button>
                <button className="button sekundarno" type="button" onClick={closeModal}>
                  Odustani
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {confirmState.open && confirmationContent && (
        <div className="users-modal-overlay" role="presentation" onClick={closeConfirm}>
          <div
            className="users-modal users-confirm-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="confirm-title"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="users-modal-header">
              <div>
                <h2 id="confirm-title">{confirmationContent.title}</h2>
                <p>{confirmationContent.description}</p>
              </div>
              <button
                type="button"
                className="users-modal-close"
                aria-label="Zatvori potvrdu"
                onClick={closeConfirm}
              >
                ×
              </button>
            </div>

            {confirmState.payload?.user && (
              <div className="users-confirm-details">
                <p><strong>Ime i prezime:</strong> {confirmState.payload.user.imePrezime}</p>
                <p><strong>Email:</strong> {confirmState.payload.user.email}</p>
                <p><strong>Uloga:</strong> {getRoleLabel(confirmState.payload.user.role)}</p>
              </div>
            )}

            <div className="users-modal-actions">
              <button className="button sekundarno" type="button" onClick={closeConfirm}>
                Odustani
              </button>
              <button className={confirmationContent.buttonClass} type="button" onClick={handleConfirmAction}>
                {confirmationContent.actionLabel}
              </button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}

export default Korisnici;
