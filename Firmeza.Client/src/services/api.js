import axios from 'axios';

const api = axios.create({
  baseURL: '/', // Cambiado a la raíz
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

export default api;
