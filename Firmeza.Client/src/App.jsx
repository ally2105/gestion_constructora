import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ProductsPage from './pages/ProductsPage';
import CartPage from './pages/CartPage';
import Navbar from './components/Navbar'; // Importar Navbar

// Componente para proteger rutas
const PrivateRoute = ({ children }) => {
  const { user } = useAuth();
  return user ? children : <Navigate to="/login" />;
};

function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <Router>
          <Navbar /> {/* Renderizar el Navbar aquí */}
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route 
              path="/products" 
              element={
                <PrivateRoute>
                  <ProductsPage />
                </PrivateRoute>
              } 
            />
            <Route 
              path="/cart" 
              element={
                <PrivateRoute>
                  <CartPage />
                </PrivateRoute>
              } 
            />
            <Route path="*" element={<Navigate to="/products" />} />
          </Routes>
        </Router>
      </CartProvider>
    </AuthProvider>
  );
}

export default App;
