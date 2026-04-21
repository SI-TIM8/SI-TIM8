import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:8080",
  timeout: 6000
});

export async function pingApi() {
  const response = await api.get("/openapi/v1.json");
  return response.status;
}
