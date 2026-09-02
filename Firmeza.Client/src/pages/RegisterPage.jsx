import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import toast from 'react-hot-toast';
import '../styles/forms.css';

const RegisterPage = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    nombre: '',
    identificacion: '',
    fechaNacimiento: '',
    direccion: '',
    telefono: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setIsLoading(true);

    try {
      await api.post('/api/Clientes', formData);
      setSuccess('¡Registro completado! Redirigiendo al inicio de sesión...');
      toast.success('Cuenta creada exitosamente.');
      setTimeout(() => navigate('/login'), 1800);
    } catch (err) {
      console.error('Error en el registro:', err);
      if (err.response) {
        const apiError = err.response.data;
        if (apiError.errors) {
          const errorMessages = Object.values(apiError.errors).flat().join(' ');
          setError(`Validación: ${errorMessages}`);
        } else if (apiError.message) {
          setError(apiError.message);
        } else {
          setError(`Error ${err.response.status}: No se pudo completar el registro.`);
        }
      } else {
        setError('No se pudo conectar con el servidor backend.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="auth-card" style={{ maxWidth: '1050px' }}>
        {/* Left Side Banner */}
        <div className="auth-banner">
          <div className="auth-banner-content">
            <Link to="/products" className="auth-brand-logo">
              <svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                <path d="M3 21h18" />
                <path d="M5 21V7l8-4 8 4v14" />
                <path d="M17 21v-8H7v8" />
              </svg>
              <span>Firmeza</span>
            </Link>

            <h2 className="auth-banner-title">
              Crea tu cuenta de cliente en segundos
            </h2>
            <p className="auth-banner-text">
              Regístrate para comprar materiales de construcción, guardar tus direcciones y recibir comprobantes directamente en tu e-mail.
            </p>

            <div className="auth-features">
              <div className="auth-feature-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                  <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
                <span>Catálogo exclusivo y precios actualizados</span>
              </div>
              <div className="auth-feature-item">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                  <polyline points="20 6 9 17 4 12"></polyline>
                </svg>
                <span>Proceso de pago rápido con MercadoPago</span>
              </div>
            </div>
          </div>
        </div>

        {/* Right Side Form */}
        <div className="auth-form-area" style={{ padding: 'var(--space-8)' }}>
          <div className="auth-header">
            <h2 className="auth-title">Crear Cuenta</h2>
            <p className="auth-subtitle">Completa tus datos para registrarte</p>
          </div>

          {error && (
            <div className="auth-alert auth-alert-error">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="12" cy="12" r="10"></circle>
                <line x1="12" y1="8" x2="12" y2="12"></line>
              </svg>
              <span>{error}</span>
            </div>
          )}

          {success && (
            <div className="auth-alert auth-alert-success">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
              <span>{success}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '14px' }}>
            <div className="form-group" style={{ gridColumn: '1 / -1', marginBottom: '0' }}>
              <label htmlFor="nombre" className="form-label">Nombre Completo</label>
              <input type="text" id="nombre" name="nombre" className="form-control" value={formData.nombre} onChange={handleChange} required placeholder="Juan Pérez" />
            </div>

            <div className="form-group" style={{ marginBottom: '0' }}>
              <label htmlFor="identificacion" className="form-label">Identificación (C.C. / NIT)</label>
              <input type="text" id="identificacion" name="identificacion" className="form-control" value={formData.identificacion} onChange={handleChange} required placeholder="123456789" />
            </div>

            <div className="form-group" style={{ marginBottom: '0' }}>
              <label htmlFor="fechaNacimiento" className="form-label">Fecha de Nacimiento</label>
              <input type="date" id="fechaNacimiento" name="fechaNacimiento" className="form-control" value={formData.fechaNacimiento} onChange={handleChange} required />
            </div>

            <div className="form-group" style={{ gridColumn: '1 / -1', marginBottom: '0' }}>
              <label htmlFor="email" className="form-label">Correo Electrónico</label>
              <input type="email" id="email" name="email" className="form-control" value={formData.email} onChange={handleChange} required placeholder="juan@ejemplo.com" />
            </div>

            <div className="form-group" style={{ gridColumn: '1 / -1', marginBottom: '0' }}>
              <label htmlFor="password" className="form-label">Contraseña</label>
              <input type="password" id="password" name="password" className="form-control" value={formData.password} onChange={handleChange} required placeholder="••••••••" />
            </div>

            <div className="form-group" style={{ marginBottom: '0' }}>
              <label htmlFor="direccion" className="form-label">Dirección (Opcional)</label>
              <input type="text" id="direccion" name="direccion" className="form-control" value={formData.direccion} onChange={handleChange} placeholder="Calle 123 #45-67" />
            </div>

            <div className="form-group" style={{ marginBottom: '0' }}>
              <label htmlFor="telefono" className="form-label">Teléfono (Opcional)</label>
              <input type="tel" id="telefono" name="telefono" className="form-control" value={formData.telefono} onChange={handleChange} placeholder="+57 300 123 4567" />
            </div>

            <button type="submit" className="btn btn-primary btn-form-submit" style={{ gridColumn: '1 / -1', marginTop: '10px' }} disabled={isLoading}>
              {isLoading ? 'Registrando...' : 'Crear mi Cuenta'}
            </button>
          </form>

          <div className="form-link">
            <p>
              ¿Ya tienes cuenta? <Link to="/login">Inicia sesión aquí</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
