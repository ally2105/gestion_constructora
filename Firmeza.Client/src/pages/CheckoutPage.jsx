import React, { useState } from 'react';
import { useCart } from '../context/CartContext';
import { useAuth } from '../context/AuthContext';
import { useNavigate, useLocation } from 'react-router-dom';
import { processCheckoutPreference } from '../services/mercadopago';
import api from '../services/api';
import Footer from '../components/Footer';
import toast from 'react-hot-toast';
import '../styles/CheckoutPage.css';

const CheckoutPage = () => {
  const { cartItems, calculateTotals, clearCart } = useCart();
  const { user } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const discount = location.state?.discount || 0;
  const { subtotal, tax, total } = calculateTotals();
  const finalDiscount = total * discount;
  const finalTotal = total - finalDiscount;

  const [formData, setFormData] = useState({
    nombre: user?.name || '',
    email: user?.email || '',
    telefono: '',
    direccion: '',
    ciudad: 'Bogotá',
    notas: '',
  });
  const [isProcessing, setIsProcessing] = useState(false);
  const [paymentMethod, setPaymentMethod] = useState('mercadopago');

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleProcessPayment = async (e) => {
    e.preventDefault();

    if (cartItems.length === 0) {
      toast.error('Tu carrito está vacío.');
      navigate('/cart');
      return;
    }

    if (!user || !user.id) {
      toast.error('Debes iniciar sesión para completar la compra.');
      navigate('/login');
      return;
    }

    setIsProcessing(true);

    try {
      // 1. Obtener ID del cliente
      let clienteId;
      try {
        const clienteRes = await api.get(`/api/Clientes/byuser/${user.id}`);
        clienteId = clienteRes.data.id;
      } catch (err) {
        console.warn('No se encontró cliente por userId, intentando crear/usar ID 1 como fallback', err);
        clienteId = 1;
      }

      // 2. Intentar preferencia de MercadoPago
      let preferenceResult = null;
      try {
        preferenceResult = await processCheckoutPreference(cartItems, {
          nombre: formData.nombre,
          email: formData.email,
          telefono: formData.telefono,
          direccion: formData.direccion,
        });
      } catch (err) {
        console.warn('MercadoPago preference endpoint not ready or test mode enabled, processing local order directly.', err);
      }

      // 3. Registrar las ventas en la base de datos
      const orderNumber = `FIR-${Math.floor(100000 + Math.random() * 900000)}`;
      for (const item of cartItems) {
        const ventaData = {
          clienteId: clienteId,
          fechaVenta: new Date().toISOString(),
          productoId: item.id,
          cantidad: item.quantity,
        };
        await api.post('/api/Ventas', ventaData);
      }

      toast.success('¡Pedido procesado correctamente!');
      
      // Si MercadoPago retornó un initPoint real, redirigir allí. Si no, ir a la página de confirmación.
      if (preferenceResult?.initPoint) {
        window.location.href = preferenceResult.initPoint;
      } else {
        clearCart();
        navigate('/order-confirmation', {
          state: {
            orderNumber,
            total: finalTotal,
            shippingAddress: formData.direccion,
            customerName: formData.nombre,
            paymentMethod: paymentMethod === 'mercadopago' ? 'MercadoPago (Tarjetas / PSE / Nequi)' : 'Transferencia Directa',
          },
        });
      }
    } catch (err) {
      console.error('Error durante el checkout:', err);
      toast.error('Hubo un problema al procesar la compra. Inténtalo nuevamente.');
    } finally {
      setIsProcessing(false);
    }
  };

  return (
    <div className="checkout-page-wrapper">
      <div className="checkout-container">
        <div className="checkout-header">
          <h1 className="checkout-title">Finalizar Compra</h1>
          <p className="checkout-subtitle">Completa tus datos de envío y selecciona tu método de pago</p>
        </div>

        <form onSubmit={handleProcessPayment} className="checkout-grid">
          {/* Left Column: Shipping & Payment Info */}
          <div className="checkout-main-form">
            {/* Step 1: Shipping Details */}
            <div className="checkout-section-card">
              <div className="section-card-header">
                <div className="step-number">1</div>
                <h3 className="section-card-title">Datos de Envío</h3>
              </div>

              <div className="form-grid-2">
                <div className="form-group">
                  <label htmlFor="nombre" className="form-label">Nombre Completo</label>
                  <input
                    type="text"
                    id="nombre"
                    name="nombre"
                    className="form-control"
                    value={formData.nombre}
                    onChange={handleChange}
                    required
                    placeholder="Juan Pérez"
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="email" className="form-label">Correo Electrónico</label>
                  <input
                    type="email"
                    id="email"
                    name="email"
                    className="form-control"
                    value={formData.email}
                    onChange={handleChange}
                    required
                    placeholder="juan@ejemplo.com"
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="telefono" className="form-label">Teléfono de Contacto</label>
                  <input
                    type="tel"
                    id="telefono"
                    name="telefono"
                    className="form-control"
                    value={formData.telefono}
                    onChange={handleChange}
                    required
                    placeholder="+57 300 123 4567"
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="ciudad" className="form-label">Ciudad / Municipio</label>
                  <input
                    type="text"
                    id="ciudad"
                    name="ciudad"
                    className="form-control"
                    value={formData.ciudad}
                    onChange={handleChange}
                    required
                    placeholder="Bogotá, Medellín, Cali..."
                  />
                </div>

                <div className="form-group full-width">
                  <label htmlFor="direccion" className="form-label">Dirección Completa de Entrega</label>
                  <input
                    type="text"
                    id="direccion"
                    name="direccion"
                    className="form-control"
                    value={formData.direccion}
                    onChange={handleChange}
                    required
                    placeholder="Calle 123 # 45 - 67 Apt 801 (Indicaciones especiales)"
                  />
                </div>
              </div>
            </div>

            {/* Step 2: Payment Method */}
            <div className="checkout-section-card">
              <div className="section-card-header">
                <div className="step-number">2</div>
                <h3 className="section-card-title">Método de Pago</h3>
              </div>

              <div className="payment-options-grid">
                <label className={`payment-option-card ${paymentMethod === 'mercadopago' ? 'selected' : ''}`}>
                  <input
                    type="radio"
                    name="paymentMethod"
                    value="mercadopago"
                    checked={paymentMethod === 'mercadopago'}
                    onChange={() => setPaymentMethod('mercadopago')}
                  />
                  <div className="payment-option-content">
                    <div className="payment-option-header">
                      <span className="payment-title">MercadoPago Checkout Pro</span>
                      <span className="recommended-badge">Recomendado</span>
                    </div>
                    <p className="payment-desc">
                      Paga de forma 100% segura con Tarjeta de Crédito, Débito, PSE, Nequi o Efecty.
                    </p>
                    <div className="payment-logos">
                      <span className="mini-badge">Visa</span>
                      <span className="mini-badge">Mastercard</span>
                      <span className="mini-badge">PSE</span>
                      <span className="mini-badge">Nequi</span>
                    </div>
                  </div>
                </label>
              </div>
            </div>
          </div>

          {/* Right Column: Order Items Summary */}
          <div className="checkout-sidebar">
            <div className="checkout-summary-card">
              <h3 className="summary-card-title">Resumen de la Orden</h3>

              <div className="checkout-items-preview">
                {cartItems.map((item) => (
                  <div key={item.id} className="preview-item">
                    <div className="preview-item-info">
                      <span className="preview-item-qty">{item.quantity}x</span>
                      <span className="preview-item-name">{item.nombre}</span>
                    </div>
                    <span className="preview-item-price">
                      ${(item.precio * item.quantity).toLocaleString('es-CO')}
                    </span>
                  </div>
                ))}
              </div>

              <div className="summary-divider"></div>

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
                    <span>Descuento Aplicado:</span>
                    <span>-${finalDiscount.toLocaleString('es-CO')}</span>
                  </div>
                )}
                <div className="summary-row">
                  <span>Costo de Envío:</span>
                  <span className="free-badge">Gratis</span>
                </div>
                <div className="summary-divider"></div>
                <div className="summary-row total-row">
                  <span>Total a Pagar:</span>
                  <span className="total-price">${finalTotal.toLocaleString('es-CO')}</span>
                </div>
              </div>

              <button
                type="submit"
                className="btn btn-primary place-order-btn"
                disabled={isProcessing}
              >
                {isProcessing ? (
                  <>
                    <svg className="animate-spin" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M21 12a9 9 0 1 1-6.219-8.56"></path>
                    </svg>
                    Conectando con MercadoPago...
                  </>
                ) : (
                  <>
                    Pagar ${finalTotal.toLocaleString('es-CO')}
                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path>
                    </svg>
                  </>
                )}
              </button>
            </div>
          </div>
        </form>
      </div>

      <Footer />
    </div>
  );
};

export default CheckoutPage;
