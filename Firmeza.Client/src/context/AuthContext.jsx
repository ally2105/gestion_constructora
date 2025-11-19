import React, { createContext, useState, useContext, useEffect } from 'react';
import api from '../services/api';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));

  useEffect(() => {
    if (token) {
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      setUser({ isAuthenticated: true }); 
    }
  }, [token]);

  const login = async (email, password) => {
    try {
      const response = await api.post('auth/login', { email, password }); // Cambiado
      const { token } = response.data;
      localStorage.setItem('token', token);
      setToken(token);
      setUser({ isAuthenticated: true });
      return true;
    } catch (error) {
      console.error('Error en el inicio de sesión:', error);
      logout();
      return false;
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setToken(null);
    setUser(null);
    delete api.defaults.headers.common['Authorization'];
  };

  const authContextValue = {
    user,
    login,
    logout,
  };

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  return useContext(AuthContext);
};
