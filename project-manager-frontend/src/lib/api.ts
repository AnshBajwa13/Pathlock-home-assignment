import axios from 'axios';

// Use environment variable or fallback to localhost
// In production (Vercel), set VITE_API_URL to: https://pathlock-home-assignment-production-344b.up.railway.app/api
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

console.log('🔗 API Base URL:', API_BASE_URL);

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor to handle errors
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Log error for debugging
    console.error('API Error:', error.response?.status, error.response?.data || error.message);
    
    if (error.response?.status === 401) {
      const isLoginPage = window.location.pathname === '/login';
      
      // If on login page, just return the error (show "Invalid credentials")
      if (isLoginPage) {
        return Promise.reject(error);
      }
      
      // Otherwise, token expired - redirect to login
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    
    return Promise.reject(error);
  }
);

export default api;
