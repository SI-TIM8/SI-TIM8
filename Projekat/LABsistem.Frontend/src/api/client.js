import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5222/api",
  timeout: 6000
});

// Automatski dodaje JWT token na svaki zahtjev
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Hvata 401 i odjavljuje korisnika
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("tokenExpiry");
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
