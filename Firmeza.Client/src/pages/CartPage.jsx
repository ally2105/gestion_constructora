import React, { useState } from 'react';
import { useCart } from '../context/CartContext';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import toast from 'react-hot-toast';

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

      toast.success('¡Compra realizada con éxito!', {
        icon: '🎉',
        style: {
          borderRadius: '10px',
          background: '#1e293b',
          color: '#fff',
        },
      });
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
    <div className="text-center" style={{ padding: '60px 20px' }}>
      <svg xmlns="http://www.w3.org/2000/svg" width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round" style={{ color: 'var(--color-text-secondary)', marginBottom: '24px', opacity: 0.5 }}>
        <circle cx="9" cy="21" r="1"></circle>
        <circle cx="20" cy="21" r="1"></circle>
        <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
      </svg>
      <h3 style={{ fontSize: '1.5rem', marginBottom: '16px', color: 'var(--color-text-main)' }}>Tu carrito está vacío</h3>
      <p style={{ color: 'var(--color-text-secondary)', marginBottom: '32px' }}>Parece que aún no has añadido ningún producto. ¡Explora nuestro catálogo!</p>
      <button className="btn btn-primary" onClick={() => navigate('/products')}>
        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          <rect x="3" y="3" width="7" height="7"></rect>
          <rect x="14" y="3" width="7" height="7"></rect>
          <rect x="14" y="14" width="7" height="7"></rect>
          <rect x="3" y="14" width="7" height="7"></rect>
        </svg>
        Ver Productos
      </button>
    </div>
  );

  return (
    <div className="main-container">
      <h1 className="page-title">Tu Carrito de Compras</h1>

      {cartItems.length === 0 ? (
        <EmptyCart />
      ) : (
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 350px', gap: '40px', alignItems: 'start' }} className="cart-layout-responsive">
          <div className="table-container">
            <table className="table-premium">
              <thead>
                <tr>
                  <th>Producto</th>
                  <th>Precio</th>
                  <th>Cantidad</th>
                  <th>Total</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {cartItems.map((item) => (
                  <tr key={item.id}>
                    <td>
                      <div style={{ fontWeight: '600' }}>{item.nombre}</div>
                    </td>
                    <td style={{ color: 'var(--color-text-secondary)' }}>${item.precio.toFixed(2)}</td>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                        <button
                          onClick={() => updateQuantity(item.id, item.quantity - 1)}
                          className="btn btn-secondary"
                          style={{ padding: '4px 8px', minWidth: '32px' }}
                        >-</button>
                        <span style={{ fontWeight: '600', minWidth: '20px', textAlign: 'center' }}>{item.quantity}</span>
                        <button
                          onClick={() => updateQuantity(item.id, item.quantity + 1)}
                          className="btn btn-secondary"
                          style={{ padding: '4px 8px', minWidth: '32px' }}
                        >+</button>
                      </div>
                    </td>
                    <td style={{ color: 'var(--color-primary)', fontWeight: '700' }}>
                      ${(item.precio * item.quantity).toFixed(2)}
                    </td>
                    <td>
                      <button
                        onClick={() => removeFromCart(item.id)}
                        className="btn btn-danger"
                        style={{ padding: '8px', borderRadius: '8px' }}
                        title="Eliminar producto"
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                          <polyline points="3 6 5 6 21 6"></polyline>
                          <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                        </svg>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="cart-summary">
            <h3 style={{ fontSize: '1.25rem', marginBottom: '24px', color: 'var(--color-text-main)' }}>Resumen de Compra</h3>

            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '12px' }}>
              <span style={{ color: 'var(--color-text-secondary)' }}>Subtotal:</span>
              <span style={{ fontWeight: '600' }}>${subtotal.toFixed(2)}</span>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '24px' }}>
              <span style={{ color: 'var(--color-text-secondary)' }}>IVA (19%):</span>
              <span style={{ fontWeight: '600' }}>${tax.toFixed(2)}</span>
            </div>

            <div style={{ borderTop: '1px solid var(--color-border)', margin: '20px 0', paddingTop: '20px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span style={{ fontSize: '1.1rem', color: 'var(--color-text-main)' }}>Total:</span>
              <span style={{ fontSize: '2rem', fontWeight: '800', color: 'var(--color-primary)' }}>${total.toFixed(2)}</span>
            </div>

            <button
              onClick={handleCheckout}
              className="btn btn-primary"
              style={{ width: '100%', marginTop: '16px', padding: '16px' }}
              disabled={isProcessing}
            >
              {isProcessing ? (
                <>
                  <svg className="animate-spin" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" style={{ marginRight: '8px' }}>
                    <path d="M21 12a9 9 0 1 1-6.219-8.56"></path>
                  </svg>
                  Procesando...
                </>
              ) : (
                <>
                  Proceder al Pago
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <line x1="5" y1="12" x2="19" y2="12"></line>
                    <polyline points="12 5 19 12 12 19"></polyline>
                  </svg>
                </>
              )}
            </button>

            <div style={{ marginTop: '24px', textAlign: 'center' }}>
              <Link to="/products" style={{ color: 'var(--color-text-secondary)', textDecoration: 'none', fontSize: '0.9rem', display: 'inline-flex', alignItems: 'center', gap: '6px' }}>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <polyline points="15 18 9 12 15 6"></polyline>
                </svg>
                Continuar Comprando
              </Link>
            </div>
          </div>
        </div>
      )}

      <style>{`
        @media (max-width: 900px) {
          .cart-layout-responsive {
            grid-template-columns: 1fr !important;
          }
          .table-container {
            overflow-x: auto;
          }
        }
        .animate-spin {
          animation: spin 1s linear infinite;
        }
        @keyframes spin {
          from { transform: rotate(0deg); }
          to { transform: rotate(360deg); }
        }
      `}</style>
    </div>
  );
};

export default CartPage;
