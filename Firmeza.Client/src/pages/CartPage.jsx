import React, { useState } from 'react';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import { useNavigate, Link } from 'react-router-dom';
import Footer from '../components/Footer';
import toast from 'react-hot-toast';
import '../styles/CartPage.css';

const CartPage = () => {
  const { cartItems, updateQuantity, removeFromCart, calculateTotals, clearCart } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();
  const { subtotal, tax, total } = calculateTotals();
  const [couponCode, setCouponCode] = useState('');
  const [discount, setDiscount] = useState(0);

  const handleApplyCoupon = (e) => {
    e.preventDefault();
    if (!couponCode.trim()) return;
    if (couponCode.toUpperCase() === 'FIRMEZA10') {
      setDiscount(0.1); // 10% discount
      toast.success('¡Cupón del 10% aplicado!');
    } else {
      toast.error('Cupón no válido. Prueba con "FIRMEZA10".');
    }
  };

  const finalDiscount = total * discount;
  const finalTotal = total - finalDiscount;

  const handleProceedToCheckout = () => {
    if (cartItems.length === 0) {
      toast.error('El carrito está vacío.');
      return;
    }
    if (!user || !user.id) {
      toast.error('Debes iniciar sesión para realizar una compra.');
      navigate('/login');
      return;
    }
    navigate('/checkout', { state: { discount, couponCode } });
  };

  return (
    <div className="cart-page-wrapper">
      <div className="cart-container">
        <div className="cart-header">
          <h1 className="cart-title">Tu Carrito de Compras</h1>
          <p className="cart-subtitle">
            {cartItems.length > 0
              ? `Tienes ${cartItems.reduce((acc, i) => acc + i.quantity, 0)} artículo(s) seleccionados`
              : 'Tu carrito está actualmente vacío'}
          </p>
        </div>

        {cartItems.length === 0 ? (
          <div className="empty-cart-card">
            <div className="empty-cart-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="72" height="72" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="9" cy="21" r="1"></circle>
                <circle cx="20" cy="21" r="1"></circle>
                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
              </svg>
            </div>
            <h3>¡Empieza a construir!</h3>
            <p>Aún no has agregado ningún producto a tu carrito. Revisa nuestro catálogo.</p>
            <button className="btn btn-primary" onClick={() => navigate('/products')}>
              Explorar Productos
            </button>
          </div>
        ) : (
          <div className="cart-grid-layout">
            {/* Products List */}
            <div className="cart-items-section">
              <div className="cart-items-header">
                <span>Producto</span>
                <span>Precio Unitario</span>
                <span>Cantidad</span>
                <span>Subtotal</span>
              </div>

              <div className="cart-items-list">
                {cartItems.map((item) => {
                  const itemSubtotal = item.precio * item.quantity;
                  return (
                    <div key={item.id} className="cart-item-card" id={`cart-item-${item.id}`}>
                      {/* Product Info */}
                      <div className="cart-item-info">
                        <div className="cart-item-thumbnail">
                          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"></path>
                          </svg>
                        </div>
                        <div>
                          <h4 className="cart-item-title">{item.nombre}</h4>
                          <span className="cart-item-stock-info">
                            {item.stock > 0 ? `Stock disponible: ${item.stock}` : 'Agotado'}
                          </span>
                        </div>
                      </div>

                      {/* Price */}
                      <div className="cart-item-unit-price">
                        ${item.precio.toLocaleString('es-CO')}
                      </div>

                      {/* Quantity Controls */}
                      <div className="cart-quantity-selector">
                        <button
                          onClick={() => updateQuantity(item.id, item.quantity - 1)}
                          className="qty-btn"
                          aria-label="Disminuir"
                        >
                          -
                        </button>
                        <span className="qty-value">{item.quantity}</span>
                        <button
                          onClick={() => updateQuantity(item.id, item.quantity + 1)}
                          className="qty-btn"
                          disabled={item.quantity >= item.stock}
                          aria-label="Aumentar"
                        >
                          +
                        </button>
                      </div>

                      {/* Item Total & Remove */}
                      <div className="cart-item-total-area">
                        <span className="cart-item-total-price">
                          ${itemSubtotal.toLocaleString('es-CO')}
                        </span>
                        <button
                          onClick={() => removeFromCart(item.id)}
                          className="cart-remove-btn"
                          title="Eliminar producto"
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <polyline points="3 6 5 6 21 6"></polyline>
                            <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                          </svg>
                        </button>
                      </div>
                    </div>
                  );
                })}
              </div>

              {/* Action Buttons */}
              <div className="cart-actions-bar">
                <button className="btn btn-secondary" onClick={() => navigate('/products')}>
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <line x1="19" y1="12" x2="5" y2="12"></line>
                    <polyline points="12 19 5 12 12 5"></polyline>
                  </svg>
                  Seguir Comprando
                </button>

                <button className="btn-clear-cart" onClick={clearCart}>
                  Vaciar Carrito
                </button>
              </div>
            </div>

            {/* Order Summary Sidebar */}
            <div className="cart-summary-card">
              <h3 className="summary-card-title">Resumen del Pedido</h3>

              {/* Coupon Form */}
              <form onSubmit={handleApplyCoupon} className="coupon-form">
                <input
                  type="text"
                  placeholder="Código de cupón (ej: FIRMEZA10)"
                  value={couponCode}
                  onChange={(e) => setCouponCode(e.target.value)}
                  className="form-control coupon-input"
                />
                <button type="submit" className="btn btn-secondary coupon-btn">
                  Aplicar
                </button>
              </form>

              <div className="summary-breakdown">
                <div className="summary-row">
                  <span>Subtotal:</span>
                  <span>${subtotal.toLocaleString('es-CO')}</span>
                </div>
                <div className="summary-row">
                  <span>IVA (19%):</span>
                  <span>${tax.toLocaleString('es-CO')}</span>
                </div>
                {discount > 0 && (
                  <div className="summary-row discount-row">
                    <span>Descuento (10%):</span>
                    <span>-${finalDiscount.toLocaleString('es-CO')}</span>
                  </div>
                )}
                <div className="summary-row shipping-row">
                  <span>Envío:</span>
                  <span className="free-badge">Gratis</span>
                </div>
                <div className="summary-divider"></div>
                <div className="summary-row total-row">
                  <span>Total estimado:</span>
                  <span className="total-price">${finalTotal.toLocaleString('es-CO')}</span>
                </div>
              </div>

              <button
                className="btn btn-primary checkout-proceed-btn"
                onClick={handleProceedToCheckout}
              >
                Proceder al Pago
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <line x1="5" y1="12" x2="19" y2="12"></line>
                  <polyline points="12 5 19 12 12 19"></polyline>
                </svg>
              </button>

              <div className="security-guarantee">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                </svg>
                <span>Procesado de forma segura con MercadoPago</span>
              </div>
            </div>
          </div>
        )}
      </div>

      <Footer />
    </div>
  );
};

export default CartPage;
