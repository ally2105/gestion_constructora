import React from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
  const { cartItems } = useCart();
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const totalItems = cartItems.reduce((acc, item) => acc + item.quantity, 0);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="navbar-header">
      <div className="navbar-container">
        <NavLink to="/products" className="navbar-logo">Firmeza</NavLink>
        <nav className="navbar-menu">
          {user ? (
            <>
              <NavLink to="/products" className="nav-item">Productos</NavLink>
              <NavLink to="/cart" className="nav-item">
                Carrito 
                {totalItems > 0 && <span className="cart-badge">{totalItems}</span>}
              </NavLink>
              <div className="user-menu">
                <span className="user-name">Hola, {user.name || 'Usuario'}</span>
                <button onClick={handleLogout} className="btn-logout">Cerrar Sesión</button>
              </div>
            </>
          ) : (
            <>
              <NavLink to="/login" className="nav-item">Iniciar Sesión</NavLink>
              <NavLink to="/register" className="btn-register">Registrarse</NavLink>
            </>
          )}
        </nav>
      </div>
    </header>
  );
};

export default Navbar;
