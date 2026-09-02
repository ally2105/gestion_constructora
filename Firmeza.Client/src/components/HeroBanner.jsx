import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/HeroBanner.css';

const HeroBanner = () => {
  const navigate = useNavigate();

  return (
    <section className="hero-banner" id="hero-banner">
      {/* Animated Background */}
      <div className="hero-bg-effects">
        <div className="hero-orb hero-orb-1"></div>
        <div className="hero-orb hero-orb-2"></div>
        <div className="hero-orb hero-orb-3"></div>
        <div className="hero-grid-pattern"></div>
      </div>

      <div className="hero-content">
        <div className="hero-text">
          <div className="hero-badge">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon>
            </svg>
            Envío gratis en compras +$200.000
          </div>
          <h1 className="hero-title">
            Construye tus <span className="hero-highlight">sueños</span> con los mejores materiales
          </h1>
          <p className="hero-subtitle">
            Encuentra cemento, acero, herramientas y todo lo que necesitas para tu proyecto al mejor precio con entrega a domicilio.
          </p>
          <div className="hero-actions">
            <button className="hero-cta-primary" onClick={() => {
              document.getElementById('products-section')?.scrollIntoView({ behavior: 'smooth' });
            }}>
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="9" cy="21" r="1"></circle>
                <circle cx="20" cy="21" r="1"></circle>
                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
              </svg>
              Comprar Ahora
            </button>
            <button className="hero-cta-secondary" onClick={() => navigate('/products')}>
              Ver Catálogo
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <line x1="5" y1="12" x2="19" y2="12"></line>
                <polyline points="12 5 19 12 12 19"></polyline>
              </svg>
            </button>
          </div>
        </div>

        {/* Stats */}
        <div className="hero-stats">
          <div className="hero-stat">
            <span className="hero-stat-number">500+</span>
            <span className="hero-stat-label">Productos</span>
          </div>
          <div className="hero-stat-divider"></div>
          <div className="hero-stat">
            <span className="hero-stat-number">2K+</span>
            <span className="hero-stat-label">Clientes</span>
          </div>
          <div className="hero-stat-divider"></div>
          <div className="hero-stat">
            <span className="hero-stat-number">24h</span>
            <span className="hero-stat-label">Entrega</span>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeroBanner;
