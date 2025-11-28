import React from 'react';
import { useCart } from '../context/CartContext';
import toast from 'react-hot-toast';

const ProductCard = ({ product }) => {
  const { addToCart } = useCart();

  const handleAddToCart = () => {
    addToCart(product);
    toast.success(`${product.nombre} añadido al carrito!`, {
      icon: '🛒',
      style: {
        borderRadius: '10px',
        background: '#1e293b',
        color: '#fff',
      },
    });
  };

  return (
    <div className="product-card">
      <div className="product-header">
        <h3 className="product-title">{product.nombre}</h3>
        <span className="badge-stock" style={{
          backgroundColor: product.stock > 0 ? 'rgba(16, 185, 129, 0.1)' : 'rgba(239, 68, 68, 0.1)',
          color: product.stock > 0 ? 'var(--color-success)' : 'var(--color-danger)',
          border: `1px solid ${product.stock > 0 ? 'rgba(16, 185, 129, 0.2)' : 'rgba(239, 68, 68, 0.2)'}`
        }}>
          {product.stock > 0 ? `Stock: ${product.stock}` : 'Agotado'}
        </span>
      </div>

      <p className="product-description">{product.descripcion}</p>

      <div className="product-footer">
        <span className="product-price">${product.precio.toFixed(2)}</span>
        <button
          className="btn btn-primary"
          onClick={handleAddToCart}
          disabled={product.stock === 0}
          style={{
            padding: '8px 16px',
            fontSize: '0.9rem',
            opacity: product.stock === 0 ? 0.5 : 1,
            cursor: product.stock === 0 ? 'not-allowed' : 'pointer'
          }}
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <circle cx="9" cy="21" r="1"></circle>
            <circle cx="20" cy="21" r="1"></circle>
            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
          </svg>
          {product.stock > 0 ? 'Agregar' : 'Agotado'}
        </button>
      </div>
    </div>
  );
};

export default ProductCard;
