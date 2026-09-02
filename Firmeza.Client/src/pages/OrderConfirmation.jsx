import React from 'react';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import Footer from '../components/Footer';
import '../styles/OrderConfirmation.css';

const OrderConfirmation = () => {
  const location = useLocation();
  const navigate = useNavigate();

  const state = location.state || {
    orderNumber: 'FIR-789123',
    total: 0,
    shippingAddress: 'Dirección confirmada',
    customerName: 'Cliente',
    paymentMethod: 'MercadoPago',
  };

  return (
    <div className="confirmation-page-wrapper">
      <div className="confirmation-container">
        <div className="confirmation-card">
          {/* Animated Success Icon */}
          <div className="success-icon-wrapper">
            <div className="success-icon-bg">
              <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
            </div>
          </div>

          <h1 className="confirmation-title">¡Gracias por tu compra, {state.customerName}!</h1>
          <p className="confirmation-subtitle">
            Tu pedido <strong className="order-code">#{state.orderNumber}</strong> ha sido recibido exitosamente y está siendo procesado.
          </p>

          {/* Details Box */}
          <div className="confirmation-details-box">
            <div className="detail-row">
              <span className="detail-label">Número de Pedido:</span>
              <span className="detail-value">#{state.orderNumber}</span>
            </div>
            <div className="detail-row">
              <span className="detail-label">Estado del Pago:</span>
              <span className="detail-value status-approved">Aprobado</span>
            </div>
            <div className="detail-row">
              <span className="detail-label">Método de Pago:</span>
              <span className="detail-value">{state.paymentMethod}</span>
            </div>
            <div className="detail-row">
              <span className="detail-label">Dirección de Entrega:</span>
              <span className="detail-value">{state.shippingAddress}</span>
            </div>
            <div className="detail-divider"></div>
            <div className="detail-row total">
              <span className="detail-label">Total Pagado:</span>
              <span className="detail-value total">${(state.total || 0).toLocaleString('es-CO')} COP</span>
            </div>
          </div>

          <div className="email-notice">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
              <polyline points="22,6 12,13 2,6"></polyline>
            </svg>
            <span>Hemos enviado el comprobante en PDF y el resumen de tu compra a tu correo electrónico.</span>
          </div>

          {/* Actions */}
          <div className="confirmation-actions">
            <Link to="/products" className="btn btn-primary">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="9" cy="21" r="1"></circle>
                <circle cx="20" cy="21" r="1"></circle>
                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
              </svg>
              Volver a la Tienda
            </Link>
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
};

export default OrderConfirmation;
