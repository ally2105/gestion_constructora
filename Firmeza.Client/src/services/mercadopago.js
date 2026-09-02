import { createPaymentPreference as apiCreatePreference } from './api';

// MercadoPago Public Key desde variables de entorno
export const MERCADOPAGO_PUBLIC_KEY = import.meta.env.VITE_MERCADOPAGO_PUBLIC_KEY || 'TEST-0000000000000000-000000-00000000000000000000000000000000-000000000';

/**
 * Carga el SDK oficial de MercadoPago.js dinámicamente si no existe
 */
export const loadMercadoPagoSDK = () => {
  return new Promise((resolve, reject) => {
    if (window.MercadoPago) {
      resolve(window.MercadoPago);
      return;
    }
    const script = document.createElement('script');
    script.src = 'https://sdk.mercadopago.com/js/v2';
    script.type = 'text/javascript';
    script.async = true;
    script.onload = () => {
      resolve(window.MercadoPago);
    };
    script.onerror = () => {
      reject(new Error('No se pudo cargar el SDK de MercadoPago'));
    };
    document.body.appendChild(script);
  });
};

/**
 * Procesa la creación de la preferencia de pago en el backend
 * @param {Array} cartItems - Lista de productos en el carrito
 * @param {Object} customerData - Datos del comprador
 */
export const processCheckoutPreference = async (cartItems, customerData) => {
  try {
    const items = cartItems.map(item => ({
      id: item.id.toString(),
      title: item.nombre,
      description: item.descripcion || item.nombre,
      quantity: item.quantity,
      unit_price: item.precio,
      currency_id: 'COP'
    }));

    const payer = {
      name: customerData.nombre || 'Cliente',
      email: customerData.email,
      phone: {
        number: customerData.telefono || ''
      },
      address: {
        street_name: customerData.direccion || ''
      }
    };

    const result = await apiCreatePreference(items, payer);
    return result; // Devuelve { preferenceId, initPoint, sandboxInitPoint }
  } catch (error) {
    console.error('Error al generar la preferencia de pago en MercadoPago:', error);
    throw error;
  }
};
