import { useState, useEffect } from 'react';
import { login, register, refreshToken } from '../api/authApi';

export const useAuth = () => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Check for existing token on initial load
    const checkAuth = async () => {
      try {
        const token = localStorage.getItem('token');
        const refreshToken = localStorage.getItem('refreshToken');
        
        if (token && refreshToken) {
          // Verify token or refresh if expired
          // This is simplified - in production you'd need to properly verify the token
          const userData = parseJwt(token);
          setUser(userData);
        }
      } catch (err) {
        console.error('Auth check failed:', err);
        logout();
      } finally {
        setLoading(false);
      }
    };

    checkAuth();
  }, []);

  const parseJwt = (token) => {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (e) {
      return null;
    }
  };

  const handleLogin = async (email, password) => {
    try {
      setLoading(true);
      const { token, refreshToken } = await login(email, password);
      
      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
      
      const userData = parseJwt(token);
      setUser(userData);
      setError(null);
      return true;
    } catch (err) {
      setError(err.response?.data?.message || 'Login failed');
      return false;
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async (email, password) => {
    try {
      setLoading(true);
      const { token, refreshToken } = await register(email, password);
      
      localStorage.setItem('token', token);
      localStorage.setItem('refreshToken', refreshToken);
      
      const userData = parseJwt(token);
      setUser(userData);
      setError(null);
      return true;
    } catch (err) {
      setError(err.response?.data?.message || 'Registration failed');
      return false;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setUser(null);
  };

  return {
    user,
    loading,
    error,
    login: handleLogin,
    register: handleRegister,
    logout,
  };
};