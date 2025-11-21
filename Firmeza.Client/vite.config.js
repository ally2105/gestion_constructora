import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5165', // Actualizado al puerto 5165
        changeOrigin: true,
        secure: false,
      },
    },
  },
  test: { // Configuración para Vitest
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/setupTests.js', // Opcional: archivo de configuración de pruebas
  },
});
