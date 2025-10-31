import { create } from 'zustand';
import type { User } from '../types';

interface AuthState {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  login: (user: User, accessToken: string, refreshToken: string) => void;
  logout: () => void;
  loadFromStorage: () => void;
}

export const useAuthStore = create<AuthState>((set) => {
  // Load from localStorage on initialization
  const userStr = localStorage.getItem('user');
  const token = localStorage.getItem('accessToken');
  const initialUser = userStr ? JSON.parse(userStr) : null;
  const initialToken = token || null;
  const initialIsAuthenticated = !!(userStr && token);

  return {
    user: initialUser,
    accessToken: initialToken,
    isAuthenticated: initialIsAuthenticated,

    login: (user, accessToken, refreshToken) => {
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', refreshToken);
      set({ user, accessToken, isAuthenticated: true });
    },

    logout: () => {
      localStorage.removeItem('user');
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      set({ user: null, accessToken: null, isAuthenticated: false });
    },

    loadFromStorage: () => {
      const userStr = localStorage.getItem('user');
      const token = localStorage.getItem('accessToken');
      if (userStr && token) {
        const user = JSON.parse(userStr);
        set({ user, accessToken: token, isAuthenticated: true });
      }
    },
  };
});
