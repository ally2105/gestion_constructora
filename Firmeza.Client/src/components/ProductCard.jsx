import React, { useState } from 'react';
import { useCart } from '../context/CartContext';
import toast from 'react-hot-toast';

const ProductCard = ({ product, viewMode = 'grid' }) => {
  const { addToCart } = useCart();
  const [isAdding, setIsAdding] = useState(false);

  const handleAddToCart = () => {
    if (product.stock === 0) return;
    setIsAdding(true);
    addToCart(product);
    toast.success(`${product.nombre} añadido al carrito`, {
      icon: '🛒',
      style: {
        borderRadius: '12px',
        background: 'rgba(23, 32, 52, 0.95)',
        color: '#f1f5f9',
        border: '1px solid rgba(245, 158, 11, 0.2)',
        backdropFilter: 'blur(12px)',
      },
    });
    setTimeout(() => setIsAdding(false), 600);
  };

  const getStockStatus = () => {
    if (product.stock === 0) return { text: 'Agotado', className: 'stock-out', icon: '✕' };
    if (product.stock <= 5) return { text: `¡Solo ${product.stock}!`, className: 'stock-low', icon: '⚡' };
    return { text: `Stock: ${product.stock}`, className: 'stock-ok', icon: '✓' };
  };

  const stockStatus = getStockStatus();

  const productGradients = [
    'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
    'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
    'linear-gradient(135deg, #a18cd1 0%, #fbc2eb 100%)',
    'linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%)',
    'linear-gradient(135deg, #89f7fe 0%, #66a6ff 100%)',
  ];
  const gradientIndex = (product.nombre || '').charCodeAt(0) % productGradients.length;

  // List View
  if (viewMode === 'list') {
    return (
      <div className="product-card-list" id={`product-${product.id}`}>
        <div className="product-image-list" style={{ background: productGradients[gradientIndex] }}>
          <div className="product-icon">
            <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
              <polyline points="3.27 6.96 12 12.01 20.73 6.96"></polyline>
              <line x1="12" y1="22.08" x2="12" y2="12"></line>
            </svg>
          </div>
        </div>

        <div className="product-info-list">
          <div className="product-header-list">
            <h3 className="product-title-list">{product.nombre}</h3>
            <span className={`stock-badge ${stockStatus.className}`}>
              <span className="stock-icon">{stockStatus.icon}</span>
              {stockStatus.text}
            </span>
          </div>
          <p className="product-description-list">{product.descripcion}</p>
        </div>

        <div className="product-actions-list">
          <span className="product-price-list">
            <span className="price-currency">$</span>
            {product.precio.toLocaleString('es-CO', { minimumFractionDigits: 0 })}
          </span>
          <button
            className={`btn-add-cart ${isAdding ? 'adding' : ''}`}
            onClick={handleAddToCart}
            disabled={product.stock === 0}
          >
            {isAdding ? (
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="9" cy="21" r="1"></circle>
                <circle cx="20" cy="21" r="1"></circle>
                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
              </svg>
            )}
            {product.stock > 0 ? (isAdding ? '¡Añadido!' : 'Agregar') : 'Agotado'}
          </button>
        </div>
      </div>
    );
  }

  // Grid View (default)
  return (
    <div className="product-card-grid" id={`product-${product.id}`}>
      {/* Image/Gradient Area */}
      <div className="product-image" style={{ background: productGradients[gradientIndex] }}>
        <div className="product-icon">
          <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
            <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
            <polyline points="3.27 6.96 12 12.01 20.73 6.96"></polyline>
            <line x1="12" y1="22.08" x2="12" y2="12"></line>
          </svg>
        </div>

        {/* Stock Badge */}
        <span className={`stock-badge ${stockStatus.className}`}>
          <span className="stock-icon">{stockStatus.icon}</span>
          {stockStatus.text}
        </span>

        {/* Quick Add Overlay */}
        {product.stock > 0 && (
          <div className="product-overlay">
            <button className="quick-add-btn" onClick={handleAddToCart}>
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <line x1="12" y1="5" x2="12" y2="19"></line>
                <line x1="5" y1="12" x2="19" y2="12"></line>
              </svg>
              Agregar al carrito
            </button>
          </div>
        )}
      </div>

      {/* Content */}
      <div className="product-content">
        <h3 className="product-title">{product.nombre}</h3>
        <p className="product-description">{product.descripcion}</p>

        <div className="product-footer">
          <div className="product-price-group">
            <span className="product-price">
              <span className="price-currency">$</span>
              {product.precio.toLocaleString('es-CO', { minimumFractionDigits: 0 })}
            </span>
          </div>
          <button
            className={`btn-add-cart btn-add-cart-sm ${isAdding ? 'adding' : ''}`}
            onClick={handleAddToCart}
            disabled={product.stock === 0}
            title={product.stock > 0 ? 'Agregar al carrito' : 'Producto agotado'}
          >
            {isAdding ? (
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="9" cy="21" r="1"></circle>
                <circle cx="20" cy="21" r="1"></circle>
                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
              </svg>
            )}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProductCard;
