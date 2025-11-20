import React, { createContext, useState, useContext, useEffect } from 'react';
import api from '../services/api';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));

  useEffect(() => {
    if (token) {
      console.log("AuthContext: Token encontrado en localStorage. Configurando cabecera de axios.");
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      setUser({ isAuthenticated: true }); 
    } else {
      console.log("AuthContext: No se encontró token en localStorage.");
    }
  }, [token]);

  const login = async (email, password) => {
    try {
      console.log("AuthContext: Intentando iniciar sesión...");
      const response = await api.post('/api/auth/login', { email, password });
      const { token } = response.data;
      console.log("AuthContext: Token recibido de la API:", token);
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
    console.log("AuthContext: Cerrando sesión y limpiando token.");
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
