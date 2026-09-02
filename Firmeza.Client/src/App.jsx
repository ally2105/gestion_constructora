import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ProductsPage from './pages/ProductsPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import OrderConfirmation from './pages/OrderConfirmation';
import Navbar from './components/Navbar';
import ChatAssistant from './components/ChatAssistant';
import { Toaster } from 'react-hot-toast';
import './App.css';

// Componente para proteger rutas privadas (requiere autenticación)
const PrivateRoute = ({ children }) => {
  const { user } = useAuth();
  return user ? children : <Navigate to="/login" />;
};

// Componente para proteger rutas públicas (no permite acceso si ya está autenticado)
const PublicRoute = ({ children }) => {
  const { user } = useAuth();
  return user ? <Navigate to="/products" /> : children;
};

function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <Router>
          <div className="app-container">
            <Toaster
              position="bottom-right"
              toastOptions={{
                style: {
                  background: 'rgba(23, 32, 52, 0.95)',
                  color: '#f1f5f9',
                  border: '1px solid rgba(245, 158, 11, 0.2)',
                  backdropFilter: 'blur(12px)',
                  borderRadius: '12px',
                },
              }}
            />
            <Navbar />
            <main className="main-content">
              <Routes>
                <Route path="/login" element={<PublicRoute><LoginPage /></PublicRoute>} />
                <Route path="/register" element={<PublicRoute><RegisterPage /></PublicRoute>} />
                <Route path="/products" element={<PrivateRoute><ProductsPage /></PrivateRoute>} />
                <Route path="/cart" element={<PrivateRoute><CartPage /></PrivateRoute>} />
                <Route path="/checkout" element={<PrivateRoute><CheckoutPage /></PrivateRoute>} />
                <Route path="/order-confirmation" element={<PrivateRoute><OrderConfirmation /></PrivateRoute>} />
                <Route path="*" element={<Navigate to="/products" />} />
              </Routes>
            </main>
            <ChatAssistant />
          </div>
        </Router>
      </CartProvider>
    </AuthProvider>
  );
}

export default App;
