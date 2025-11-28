import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5166', // URL completa de la API
});

// Interceptor para añadir el token JWT a cada solicitud
api.interceptors.request.use(
  (config) => {
    console.log("Interceptor de Axios: Ejecutando para la URL:", config.url);
    const token = localStorage.getItem('token');
    if (token) {
      console.log("Interceptor de Axios: Token encontrado. Añadiendo cabecera Authorization.");
      config.headers['Authorization'] = `Bearer ${token}`;
    } else {
      console.log("Interceptor de Axios: No se encontró token.");
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
