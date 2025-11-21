import React from 'react';
import { useCart } from '../context/CartContext';
import toast from 'react-hot-toast';

const ProductCard = ({ product }) => {
  const { addToCart } = useCart();

  const handleAddToCart = () => {
    addToCart(product);
    toast.success(`${product.nombre} añadido al carrito!`);
  };

  return (
    <div className="product-card">
      <h3 className="product-title">{product.nombre}</h3>
      <p className="product-description">{product.descripcion}</p>
      <div className="product-info">
        <span className="product-price" style={{ color: 'var(--color-text-main)' }}>${product.precio.toFixed(2)}</span> {/* Color cambiado a var(--color-text-main) */}
        <span className="product-stock">Stock: {product.stock}</span>
      </div>
      <button 
        className="btn" 
        onClick={handleAddToCart}
        disabled={product.stock === 0}
      >
        {product.stock > 0 ? 'Agregar al Carrito' : 'Agotado'}
      </button>
    </div>
  );
};

export default ProductCard;
