import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import toast from 'react-hot-toast';

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
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    try {
      await api.post('/api/Clientes', formData);
      setSuccess('Registro exitoso. Serás redirigido al inicio de sesión.');
      toast.success('Registro exitoso. Por favor, inicia sesión.');
      setTimeout(() => navigate('/login'), 2000);
    } catch (err) {
      console.error('Error en el registro:', err);
      if (err.response) {
        const apiError = err.response.data;
        if (apiError.errors) {
          const errorMessages = Object.values(apiError.errors).flat().join(' ');
          setError(`Error de validación: ${errorMessages}`);
          toast.error(`Error de validación: ${errorMessages}`);
        } else if (apiError.message) {
          setError(apiError.message);
          toast.error(apiError.message);
        } else {
          setError(`Error ${err.response.status}: ${JSON.stringify(apiError)}`);
          toast.error(`Error ${err.response.status}: ${JSON.stringify(apiError)}`);
        }
      } else if (err.request) {
        setError('No se pudo conectar con el servidor. ¿Está la API en ejecución?');
        toast.error('No se pudo conectar con el servidor. ¿Está la API en ejecución?');
      } else {
        setError(`Error en la configuración de la solicitud: ${err.message}`);
        toast.error(`Error en la configuración de la solicitud: ${err.message}`);
      }
    }
  };

  return (
    <div className="main-container" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '90vh', padding: '40px 20px' }}>
      <div className="form-container" style={{ maxWidth: '600px', width: '100%' }}>
        <div style={{ textAlign: 'center', marginBottom: '32px' }}>
          <svg xmlns="http://www.w3.org/2000/svg" width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" style={{ color: 'var(--color-primary)', marginBottom: '16px' }}>
            <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
            <circle cx="8.5" cy="7" r="4"></circle>
            <line x1="20" y1="8" x2="20" y2="14"></line>
            <line x1="23" y1="11" x2="17" y2="11"></line>
          </svg>
          <h2 className="page-title" style={{ fontSize: '2rem', marginBottom: '8px' }}>Crear Cuenta</h2>
          <p className="page-subtitle" style={{ marginBottom: '0', fontSize: '1rem' }}>Únete a Firmeza para gestionar tus compras</p>
        </div>

        {error && (
          <div style={{
            background: 'rgba(239, 68, 68, 0.1)',
            border: '1px solid rgba(239, 68, 68, 0.2)',
            color: 'var(--color-danger)',
            padding: '12px',
            borderRadius: '8px',
            marginBottom: '24px',
            fontSize: '0.9rem',
            display: 'flex',
            alignItems: 'center',
            gap: '8px'
          }}>
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="12" cy="12" r="10"></circle>
              <line x1="12" y1="8" x2="12" y2="12"></line>
              <line x1="12" y1="16" x2="12.01" y2="16"></line>
            </svg>
            {error}
          </div>
        )}

        {success && (
          <div style={{
            background: 'rgba(16, 185, 129, 0.1)',
            border: '1px solid rgba(16, 185, 129, 0.2)',
            color: 'var(--color-success)',
            padding: '12px',
            borderRadius: '8px',
            marginBottom: '24px',
            fontSize: '0.9rem',
            display: 'flex',
            alignItems: 'center',
            gap: '8px'
          }}>
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
            {success}
          </div>
        )}

        <form onSubmit={handleSubmit} style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
          <div className="form-group" style={{ gridColumn: '1 / -1' }}>
            <label htmlFor="nombre" className="form-label">Nombre Completo</label>
            <input type="text" id="nombre" name="nombre" className="form-control" value={formData.nombre} onChange={handleChange} required placeholder="Juan Pérez" />
          </div>

          <div className="form-group">
            <label htmlFor="identificacion" className="form-label">Identificación</label>
            <input type="text" id="identificacion" name="identificacion" className="form-control" value={formData.identificacion} onChange={handleChange} required placeholder="123456789" />
          </div>

          <div className="form-group">
            <label htmlFor="fechaNacimiento" className="form-label">Fecha de Nacimiento</label>
            <input type="date" id="fechaNacimiento" name="fechaNacimiento" className="form-control" value={formData.fechaNacimiento} onChange={handleChange} required />
          </div>

          <div className="form-group" style={{ gridColumn: '1 / -1' }}>
            <label htmlFor="email" className="form-label">Correo Electrónico</label>
            <input type="email" id="email" name="email" className="form-control" value={formData.email} onChange={handleChange} required placeholder="juan@ejemplo.com" />
          </div>

          <div className="form-group" style={{ gridColumn: '1 / -1' }}>
            <label htmlFor="password" className="form-label">Contraseña</label>
            <input type="password" id="password" name="password" className="form-control" value={formData.password} onChange={handleChange} required placeholder="••••••••" />
          </div>

          <div className="form-group" style={{ gridColumn: '1 / -1' }}>
            <label htmlFor="direccion" className="form-label">Dirección (Opcional)</label>
            <input type="text" id="direccion" name="direccion" className="form-control" value={formData.direccion} onChange={handleChange} placeholder="Calle Principal #123" />
          </div>

          <div className="form-group" style={{ gridColumn: '1 / -1' }}>
            <label htmlFor="telefono" className="form-label">Teléfono (Opcional)</label>
            <input type="tel" id="telefono" name="telefono" className="form-control" value={formData.telefono} onChange={handleChange} placeholder="+57 300 123 4567" />
          </div>

          <button type="submit" className="btn btn-primary" style={{ gridColumn: '1 / -1', marginTop: '10px' }}>
            Crear Cuenta
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <line x1="5" y1="12" x2="19" y2="12"></line>
              <polyline points="12 5 19 12 12 19"></polyline>
            </svg>
          </button>
        </form>

        <div style={{ marginTop: '24px', textAlign: 'center', borderTop: '1px solid var(--color-border)', paddingTop: '24px' }}>
          <p style={{ color: 'var(--color-text-secondary)', fontSize: '0.9rem', margin: 0 }}>
            ¿Ya tienes cuenta? <Link to="/login" style={{ color: 'var(--color-primary)', textDecoration: 'none', fontWeight: '600' }}>Inicia sesión aquí</Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
