import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7123/api",
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});


api.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error?.response?.status;
    const data = error?.response?.data;

    switch (status) {
      case 401:
        console.error("Unauthorized access:", data);
        break;
      case 403:
        console.error("Forbidden access:", data);
        break;
      case 404:
        console.error("Not found:", data);
        break;
      default:
        if (status >= 500) {
          console.error("Server error:", data);
        } else {
          console.error("API error:", data || error.message);
        }
    }

    if (!error.response) {
      console.error("Network error:", error.message);
    }

    return Promise.reject(error);
  }
);

export default api;
