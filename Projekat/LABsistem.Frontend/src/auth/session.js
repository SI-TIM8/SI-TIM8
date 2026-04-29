const ACCESS_TOKEN_KEY = "token";
const ACCESS_TOKEN_EXPIRY_KEY = "tokenExpiry";
const REFRESH_TOKEN_KEY = "refreshToken";
const REFRESH_TOKEN_EXPIRY_KEY = "refreshTokenExpiry";
const ROLE_KEY = "uloga";
const USERNAME_KEY = "korisnik";
const USER_EMAIL_KEY = "korisnikEmail";

export const REFRESH_THRESHOLD_MS = 2 * 60 * 1000;

export function clearSession() {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(ACCESS_TOKEN_EXPIRY_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_EXPIRY_KEY);
  localStorage.removeItem(ROLE_KEY);
  localStorage.removeItem(USERNAME_KEY);
  localStorage.removeItem(USER_EMAIL_KEY);
}

export function persistSession(session) {
  localStorage.setItem(ACCESS_TOKEN_KEY, session.token);
  localStorage.setItem(ACCESS_TOKEN_EXPIRY_KEY, Date.parse(session.accessTokenExpiresAtUtc).toString());
  localStorage.setItem(REFRESH_TOKEN_KEY, session.refreshToken);
  localStorage.setItem(REFRESH_TOKEN_EXPIRY_KEY, Date.parse(session.refreshTokenExpiresAtUtc).toString());
  localStorage.setItem(ROLE_KEY, session.role.toLowerCase());
  localStorage.setItem(USERNAME_KEY, session.username);
}

export function getAccessToken() {
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function getAccessTokenExpiry() {
  const value = localStorage.getItem(ACCESS_TOKEN_EXPIRY_KEY);
  return value ? Number.parseInt(value, 10) : null;
}

export function getRefreshToken() {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function getRefreshTokenExpiry() {
  const value = localStorage.getItem(REFRESH_TOKEN_EXPIRY_KEY);
  return value ? Number.parseInt(value, 10) : null;
}

export function hasActiveAccessToken() {
  const token = getAccessToken();
  const expiry = getAccessTokenExpiry();
  return Boolean(token && expiry && Date.now() < expiry);
}

export function shouldRefreshAccessToken(thresholdMs = REFRESH_THRESHOLD_MS) {
  const token = getAccessToken();
  const accessTokenExpiry = getAccessTokenExpiry();
  const refreshToken = getRefreshToken();
  const refreshTokenExpiry = getRefreshTokenExpiry();

  if (!token || !accessTokenExpiry || !refreshToken || !refreshTokenExpiry) {
    return false;
  }

  if (Date.now() >= refreshTokenExpiry) {
    return false;
  }

  return accessTokenExpiry - Date.now() <= thresholdMs;
}
