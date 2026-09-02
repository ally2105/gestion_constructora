import React, { useState, useEffect } from 'react';
import { NavLink, useNavigate, useLocation } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
  const { cartItems } = useCart();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [scrolled, setScrolled] = useState(false);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const totalItems = cartItems.reduce((acc, item) => acc + item.quantity, 0);

  useEffect(() => {
    const handleScroll = () => setScrolled(window.scrollY > 20);
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    setMobileMenuOpen(false);
  }, [location]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <>
      <header className={`navbar-header ${scrolled ? 'navbar-scrolled' : ''}`} id="main-navbar">
        <div className="navbar-container">
          {/* Logo */}
          <NavLink to="/products" className="navbar-logo">
            <div className="logo-mark">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <path d="M3 21h18" />
                <path d="M5 21V7l8-4 8 4v14" />
                <path d="M17 21v-8H7v8" />
              </svg>
            </div>
            <span className="logo-text">Firmeza</span>
          </NavLink>

          {/* Desktop Nav */}
          <nav className="navbar-menu">
            {user ? (
              <>
                <NavLink to="/products" className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}>
                  <svg xmlns="http://www.w3.org/2000/svg" width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M20.59 13.41l-7.17 7.17a2 2 0 0 1-2.83 0L2 12V2h10l8.59 8.59a2 2 0 0 1 0 2.82z"></path>
                    <line x1="7" y1="7" x2="7.01" y2="7"></line>
                  </svg>
                  Productos
                </NavLink>

                <NavLink to="/cart" className={({ isActive }) => `nav-item cart-link ${isActive ? 'active' : ''}`}>
                  <div className="cart-icon-wrapper">
                    <svg xmlns="http://www.w3.org/2000/svg" width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <circle cx="9" cy="21" r="1"></circle>
                      <circle cx="20" cy="21" r="1"></circle>
                      <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                    </svg>
                    {totalItems > 0 && (
                      <span className="cart-badge" key={totalItems}>
                        {totalItems > 99 ? '99+' : totalItems}
                      </span>
                    )}
                  </div>
                  Carrito
                </NavLink>

                <div className="nav-divider"></div>

                <div className="user-menu">
                  <div className="user-avatar">
                    {(user.name || 'U').charAt(0).toUpperCase()}
                  </div>
                  <span className="user-name">{user.name || 'Usuario'}</span>
                  <button onClick={handleLogout} className="btn-logout" title="Cerrar Sesión" id="btn-logout">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
                      <polyline points="16 17 21 12 16 7"></polyline>
                      <line x1="21" y1="12" x2="9" y2="12"></line>
                    </svg>
                  </button>
                </div>
              </>
            ) : (
              <>
                <NavLink to="/login" className="nav-item" id="nav-login">
                  Iniciar Sesión
                </NavLink>
                <NavLink to="/register" className="btn btn-primary nav-register-btn" id="nav-register">
                  Registrarse
                </NavLink>
              </>
            )}
          </nav>

          {/* Mobile Menu Toggle */}
          <button
            className={`mobile-menu-toggle ${mobileMenuOpen ? 'active' : ''}`}
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            aria-label="Menú"
            id="mobile-menu-toggle"
          >
            <span></span>
            <span></span>
            <span></span>
          </button>
        </div>
      </header>

      {/* Mobile Menu Overlay */}
      {mobileMenuOpen && (
        <div className="mobile-overlay" onClick={() => setMobileMenuOpen(false)}></div>
      )}

      {/* Mobile Drawer */}
      <div className={`mobile-drawer ${mobileMenuOpen ? 'open' : ''}`}>
        <div className="mobile-drawer-header">
          <span className="mobile-drawer-title">Menú</span>
          <button className="mobile-close-btn" onClick={() => setMobileMenuOpen(false)}>
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <line x1="18" y1="6" x2="6" y2="18"></line>
              <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
          </button>
        </div>

        {user && (
          <div className="mobile-user-section">
            <div className="user-avatar mobile-avatar">
              {(user.name || 'U').charAt(0).toUpperCase()}
            </div>
            <div>
              <p className="mobile-user-name">{user.name || 'Usuario'}</p>
              <p className="mobile-user-email">{user.email}</p>
            </div>
          </div>
        )}

        <nav className="mobile-nav">
          {user ? (
            <>
              <NavLink to="/products" className="mobile-nav-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M20.59 13.41l-7.17 7.17a2 2 0 0 1-2.83 0L2 12V2h10l8.59 8.59a2 2 0 0 1 0 2.82z"></path>
                  <line x1="7" y1="7" x2="7.01" y2="7"></line>
                </svg>
                Productos
              </NavLink>

              <NavLink to="/cart" className="mobile-nav-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="9" cy="21" r="1"></circle>
                  <circle cx="20" cy="21" r="1"></circle>
                  <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
                </svg>
                Carrito
                {totalItems > 0 && <span className="mobile-badge">{totalItems}</span>}
              </NavLink>

              <div className="mobile-nav-divider"></div>

              <button className="mobile-nav-item mobile-logout" onClick={handleLogout}>
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
                  <polyline points="16 17 21 12 16 7"></polyline>
                  <line x1="21" y1="12" x2="9" y2="12"></line>
                </svg>
                Cerrar Sesión
              </button>
            </>
          ) : (
            <>
              <NavLink to="/login" className="mobile-nav-item">Iniciar Sesión</NavLink>
              <NavLink to="/register" className="mobile-nav-item mobile-register">Registrarse</NavLink>
            </>
          )}
        </nav>
      </div>
    </>
  );
};

export default Navbar;
