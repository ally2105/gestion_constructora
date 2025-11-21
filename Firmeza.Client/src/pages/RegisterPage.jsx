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
    <div className="form-container">
      <div className="form-card">
        <h2 className="form-title">Registro de Cliente</h2>
        {error && <div className="form-error">{error}</div>}
        {success && <div className="form-success">{success}</div>}
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="email">Email:</label>
            <input type="email" id="email" name="email" className="form-control" value={formData.email} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="password">Contraseña:</label>
            <input type="password" id="password" name="password" className="form-control" value={formData.password} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="nombre">Nombre Completo:</label>
            <input type="text" id="nombre" name="nombre" className="form-control" value={formData.nombre} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="identificacion">Identificación:</label>
            <input type="text" id="identificacion" name="identificacion" className="form-control" value={formData.identificacion} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="fechaNacimiento">Fecha de Nacimiento:</label>
            <input type="date" id="fechaNacimiento" name="fechaNacimiento" className="form-control" value={formData.fechaNacimiento} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="direccion">Dirección (Opcional):</label>
            <input type="text" id="direccion" name="direccion" className="form-control" value={formData.direccion} onChange={handleChange} />
          </div>
          <div className="form-group">
            <label htmlFor="telefono">Teléfono (Opcional):</label>
            <input type="tel" id="telefono" name="telefono" className="form-control" value={formData.telefono} onChange={handleChange} />
          </div>
          <button type="submit" className="btn btn-form-submit">
            Registrarse
          </button>
        </form>
        <p className="form-link">
          ¿Ya tienes cuenta? <Link to="/login">Inicia sesión aquí</Link>
        </p>
      </div>
    </div>
  );
};

export default RegisterPage;
