import React, { useState } from 'react';
import { useCart } from '../context/CartContext';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import toast from 'react-hot-toast';
import '../styles/CartPage.css';

const CartPage = () => {
  const { cartItems, updateQuantity, removeFromCart, calculateTotals, clearCart } = useCart();
  const navigate = useNavigate();
  const { subtotal, tax, total } = calculateTotals();
  const [isProcessing, setIsProcessing] = useState(false);

  const handleCheckout = async () => {
    if (cartItems.length === 0) {
      toast.error('El carrito está vacío.');
      return;
    }
    setIsProcessing(true);
    
    const clienteId = 1; // Placeholder

    try {
      for (const item of cartItems) {
        const ventaData = {
          clienteId: clienteId,
          fechaVenta: new Date().toISOString(),
          productoId: item.id,
          cantidad: item.quantity,
        };
        await api.post('/api/Ventas', ventaData);
      }
      
      toast.success('¡Compra realizada con éxito!');
      clearCart();
      setTimeout(() => navigate('/products'), 2000);

    } catch (err) {
      console.error('Error al procesar la compra:', err.response?.data || err.message);
      toast.error('Error al procesar la compra.');
    } finally {
      setIsProcessing(false);
    }
  };

  const EmptyCart = () => (
    <div className="empty-cart-container">
      <h3>Tu carrito está vacío</h3>
      <p>Parece que aún no has añadido ningún producto. ¡Explora nuestro catálogo!</p>
      <button className="btn" onClick={() => navigate('/products')}>Ver Productos</button>
    </div>
  );

  return (
    <div className="main-container">
      <h1 className="page-title">Tu Carrito de Compras</h1>
      {cartItems.length === 0 ? (
        <EmptyCart />
      ) : (
        <div className="cart-layout">
          <div className="cart-items-list">
            {cartItems.map((item) => (
              <div key={item.id} className="cart-item">
                <div className="cart-item-details">
                  <h5 className="cart-item-title">{item.nombre}</h5>
                  <p className="cart-item-price">${item.precio.toFixed(2)} c/u</p>
                </div>
                <div className="quantity-controls">
                  <button onClick={() => updateQuantity(item.id, item.quantity - 1)} className="quantity-btn">-</button>
                  <span className="item-quantity">{item.quantity}</span>
                  <button onClick={() => updateQuantity(item.id, item.quantity + 1)} className="quantity-btn">+</button>
                </div>
                <p className="item-subtotal">${(item.precio * item.quantity).toFixed(2)}</p>
                <button onClick={() => removeFromCart(item.id)} className="remove-btn" title="Eliminar producto">
                  &times;
                </button>
              </div>
            ))}
          </div>
          <div className="cart-summary">
            <h3 className="summary-title">Resumen de Compra</h3>
            <div className="summary-row">
              <span>Subtotal:</span>
              <span>${subtotal.toFixed(2)}</span>
            </div>
            <div className="summary-row">
              <span>IVA (19%):</span>
              <span>${tax.toFixed(2)}</span>
            </div>
            <hr style={{ borderColor: 'var(--color-border)', margin: '20px 0' }} />
            <div className="summary-row total">
              <span>Total:</span>
              <span>${total.toFixed(2)}</span>
            </div>
            <button onClick={handleCheckout} className="btn" style={{ width: '100%' }} disabled={isProcessing}>
              {isProcessing ? 'Procesando...' : 'Proceder al Pago'}
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default CartPage;
