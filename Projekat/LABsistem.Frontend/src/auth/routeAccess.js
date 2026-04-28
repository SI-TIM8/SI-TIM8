export const ROLE_LABELS = {
  student: "Student",
  profesor: "Profesor / Asistent",
  tehnicar: "Lab. tehnicar",
  admin: "Administrator",
};

export const NAVIGATION_BY_ROLE = {
  student: [
    { label: "Kalendar termina", path: "/kalendar" },
    { label: "Zakazi termin", path: "/zakazivanje" },
    { label: "Moje rezervacije", path: "/rezervacije" },
    { label: "Oprema", path: "/oprema" },
  ],
  profesor: [
    { label: "Zahtjevi studenata", path: "/zahtjevi" },
    { label: "Lista rezervacija", path: "/rezervacije" },
    { label: "Historija studenata", path: "/historija" },
    { label: "Kalendar termina", path: "/kalendar" },
  ],
  tehnicar: [
    { label: "Upravljanje terminima", path: "/termini" },
    { label: "Upravljanje opremom", path: "/oprema" },
    { label: "Kvarovi opreme", path: "/kvarovi" },
    { label: "Kalendar termina", path: "/kalendar" },
  ],
  admin: [
    { label: "Upravljanje korisnicima", path: "/korisnici" },
    { label: "Objekti i kabineti", path: "/objekti" },
    { label: "Kalendar termina", path: "/kalendar" },
  ],
};

export const ALLOWED_ROLES_BY_ROUTE = {
  "/dashboard": ["student", "profesor", "tehnicar", "admin"],
  "/kalendar": ["student", "profesor", "tehnicar", "admin"],
  "/profil": ["student", "profesor", "tehnicar", "admin"],
  "/o-aplikaciji": ["student", "profesor", "tehnicar", "admin"],
  "/zakazivanje": ["student"],
  "/rezervacije": ["student", "profesor"],
  "/oprema": ["student", "tehnicar"],
  "/zahtjevi": ["profesor"],
  "/historija": ["profesor"],
  "/termini": ["tehnicar"],
  "/kvarovi": ["tehnicar"],
  "/korisnici": ["admin"],
  "/objekti": ["admin"],
};

export function getCurrentRole() {
  return localStorage.getItem("uloga") || "student";
}

export function canAccessRoute(role, path) {
  const allowedRoles = ALLOWED_ROLES_BY_ROUTE[path];

  if (!allowedRoles) {
    return false;
  }

  return allowedRoles.includes(role);
}
