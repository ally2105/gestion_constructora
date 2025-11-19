import React from 'react';
import { Link, useNavigate } from 'react-router-dom'; // Añadido useNavigate
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
    <nav style={styles.navbar}>
      <div style={styles.brand}>
        <Link to="/products" style={styles.navLink}>Firmeza</Link>
      </div>
      <div style={styles.navLinks}>
        {user && (
          <>
            <Link to="/products" style={styles.navLink}>Productos</Link>
            <Link to="/cart" style={styles.navLink}>
              Carrito ({totalItems})
            </Link>
            <button onClick={handleLogout} style={styles.logoutButton}>Cerrar Sesión</button>
          </>
        )}
        {!user && (
          <>
            <Link to="/login" style={styles.navLink}>Iniciar Sesión</Link>
            <Link to="/register" style={styles.navLink}>Registrarse</Link>
          </>
        )}
      </div>
    </nav>
  );
};

const styles = {
  navbar: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#333',
    padding: '10px 20px',
    color: 'white',
  },
  brand: {
    fontSize: '1.5em',
    fontWeight: 'bold',
  },
  navLinks: {
    display: 'flex',
    gap: '20px',
    alignItems: 'center',
  },
  navLink: {
    color: 'white',
    textDecoration: 'none',
    fontSize: '1.1em',
  },
  logoutButton: {
    backgroundColor: '#dc3545',
    color: 'white',
    padding: '8px 12px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    fontSize: '1em',
  },
};

export default Navbar;
