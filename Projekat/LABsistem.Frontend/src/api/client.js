import axios from "axios";
import {
  REFRESH_THRESHOLD_MS,
  clearSession,
  getAccessToken,
  getRefreshToken,
  persistSession,
  shouldRefreshAccessToken,
} from "../auth/session";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "/api",
  timeout: 6000
});

const authApi = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "/api",
  timeout: 6000
});

let refreshPromise = null;

async function refreshSession() {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    throw new Error("Missing refresh token.");
  }

  if (!refreshPromise) {
    refreshPromise = authApi
      .post("/Auth/refresh", { refreshToken })
      .then((response) => {
        persistSession(response.data);
        return response.data.token;
      })
      .catch((error) => {
        clearSession();
        throw error;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}

// Automatski dodaje JWT token na svaki zahtjev
api.interceptors.request.use(async (config) => {
  const isRefreshRequest = config.url?.includes("/Auth/refresh");
  const isLoginRequest = config.url?.includes("/Auth/login");

  if (!isRefreshRequest && !isLoginRequest && shouldRefreshAccessToken(REFRESH_THRESHOLD_MS)) {
    await refreshSession();
  }

  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Hvata 401 i odjavljuje korisnika
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    const token = getAccessToken();
    const refreshToken = getRefreshToken();
    const isRefreshRequest = originalRequest?.url?.includes("/Auth/refresh");
    const isLoginRequest = originalRequest?.url?.includes("/Auth/login");

    if (
      error.response?.status === 401 &&
      token &&
      refreshToken &&
      !isRefreshRequest &&
      !isLoginRequest &&
      !originalRequest?._retry
    ) {
      originalRequest._retry = true;

      try {
        const refreshedToken = await refreshSession();
        originalRequest.headers = originalRequest.headers || {};
        originalRequest.headers.Authorization = `Bearer ${refreshedToken}`;
        return api(originalRequest);
      } catch {
        clearSession();
        window.location.href = "/login?sesija=istekla";
        return Promise.reject(error);
      }
    }

    if (error.response?.status === 401 && token) {
      clearSession();
      window.location.href = "/login?sesija=istekla";
    }

    return Promise.reject(error);
  }
);

export async function pingApi() {
  const response = await api.get("/openapi/v1.json");
  return response.status;
}

export default api;
