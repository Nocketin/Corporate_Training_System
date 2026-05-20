import axios, { AxiosError } from 'axios';
import { toast } from 'react-toastify';
import { clearSession } from '../lib/auth';

// Default: same origin as the Vite page; vite.config proxies /api → gateway (localhost:8080).
// Set VITE_API_BASE_URL=http://localhost:8080 if you run without the dev proxy.
const baseURL = import.meta.env.VITE_API_BASE_URL ?? '';

export const api = axios.create({
  baseURL,
});

function extractProblemDetail(error: AxiosError): string | null {
  const payload = error.response?.data;
  if (!payload || typeof payload !== 'object') {
    return null;
  }

  if (typeof (payload as { detail?: unknown }).detail === 'string') {
    return (payload as { detail: string }).detail;
  }

  return null;
}

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error: unknown) => {
    if (!axios.isAxiosError(error)) {
      return Promise.reject(error);
    }

    const status = error.response?.status;
    const path = window.location.pathname;

    if (status === 401 || status === 403) {
      clearSession();
      if (!path.startsWith('/login') && path !== '/register') {
        toast.error('Сессия истекла. Войдите снова.');
        window.location.assign('/login');
      }
      return Promise.reject(error);
    }

    if (status === 503) {
      toast.error('Сервис временно недоступен. Попробуйте позже.');
      return Promise.reject(error);
    }

    const detail = extractProblemDetail(error);
    if (detail && status && status >= 400) {
      toast.error(detail);
    }

    return Promise.reject(error);
  },
);

export { baseURL };
