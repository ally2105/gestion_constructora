import React, { createContext, useState, useContext, useEffect } from 'react';
import api from '../services/api';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));

  useEffect(() => {
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        // Extract user information from JWT claims
        // 'sub' contains the user ID (clienteId)
        // 'name' contains the user's name
        // 'email' contains the user's email
        setUser({
          id: decodedToken.sub, // User ID from JWT 'sub' claim
          name: decodedToken.name,
          email: decodedToken.email
        });
        api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      } catch (error) {
        console.error("AuthContext: Invalid token.", error);
        logout();
      }
    }
  }, [token]);

  const login = async (email, password) => {
    try {
      const response = await api.post('/api/auth/login', { email, password });
      const { token } = response.data;
      localStorage.setItem('token', token);
      setToken(token); // Esto disparará el useEffect para decodificar el token
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
