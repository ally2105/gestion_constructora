import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5165';

const api = axios.create({
  baseURL: API_URL,
});

// Interceptor para añadir el token JWT a cada solicitud
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor de respuesta para manejar errores globales
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

// === MercadoPago API ===
export const createPaymentPreference = async (items, payer) => {
  const response = await api.post('/api/mercadopago/create-preference', { items, payer });
  return response.data;
};

export const getPaymentStatus = async (paymentId) => {
  const response = await api.get(`/api/mercadopago/payment-status/${paymentId}`);
  return response.data;
};

// === Chat API ===
export const sendChatMessage = async (message, cartContext = null) => {
  const response = await api.post('/api/chat/message', { message, cartContext });
  return response.data;
};

export const searchProductsChat = async (query) => {
  const response = await api.post('/api/chat/product-search', { query });
  return response.data;
};

export default api;
