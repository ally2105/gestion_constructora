import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api'; // Usamos la instancia de axios configurada

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
      await api.post('/api/Clientes', formData); // Asegurarse de que la URL es correcta
      setSuccess('Registro exitoso. Por favor, inicia sesión.');
      setTimeout(() => navigate('/login'), 2000);
    } catch (err) {
      console.error('Error en el registro:', err);
      if (err.response) {
        // El servidor respondió con un código de estado fuera del rango 2xx
        const apiError = err.response.data;
        // Si la API devuelve un objeto de validación de ASP.NET Core
        if (apiError.errors) {
          const errorMessages = Object.values(apiError.errors).flat().join(' ');
          setError(`Error de validación: ${errorMessages}`);
        } else if (apiError.message) {
          setError(apiError.message);
        } else {
          setError(`Error ${err.response.status}: ${JSON.stringify(apiError)}`);
        }
      } else if (err.request) {
        // La solicitud fue hecha pero no se recibió respuesta
        setError('No se pudo conectar con el servidor. ¿Está la API en ejecución?');
      } else {
        // Algo sucedió al configurar la solicitud que provocó un error
        setError(`Error en la configuración de la solicitud: ${err.message}`);
      }
    }
  };

  return (
    <div style={styles.container}>
      <form onSubmit={handleSubmit} style={styles.form}>
        <h2 style={styles.title}>Registro de Cliente</h2>
        {error && <p style={styles.error}>{error}</p>}
        {success && <p style={styles.success}>{success}</p>}

        {/* ... (resto del formulario) ... */}
        <div style={styles.formGroup}>
          <label htmlFor="email" style={styles.label}>Email:</label>
          <input type="email" id="email" name="email" value={formData.email} onChange={handleChange} required style={styles.input} />
        </div>
        <div style={styles.formGroup}>
          <label htmlFor="password" style={styles.label}>Contraseña:</label>
          <input type="password" id="password" name="password" value={formData.password} onChange={handleChange} required style={styles.input} />
        </div>
        <div style={styles.formGroup}>
          <label htmlFor="nombre" style={styles.label}>Nombre Completo:</label>
          <input type="text" id="nombre" name="nombre" value={formData.nombre} onChange={handleChange} required style={styles.input} />
        </div>
        <div style={styles.formGroup}>
          <label htmlFor="identificacion" style={styles.label}>Identificación:</label>
          <input type="text" id="identificacion" name="identificacion" value={formData.identificacion} onChange={handleChange} required style={styles.input} />
        </div>
        <div style={styles.formGroup}>
          <label htmlFor="fechaNacimiento" style={styles.label}>Fecha de Nacimiento:</label>
          <input type="date" id="fechaNacimiento" name="fechaNacimiento" value={formData.fechaNacimiento} onChange={handleChange} required style={styles.input} />
        </div>
        <div style={styles.formGroup}>
          <label htmlFor="direccion" style={styles.label}>Dirección (Opcional):</label>
          <input type="text" id="direccion" name="direccion" value={formData.direccion} onChange={handleChange} style={styles.input} />
        </div>
        <div style={styles.formGroup}>
          <label htmlFor="telefono" style={styles.label}>Teléfono (Opcional):</label>
          <input type="tel" id="telefono" name="telefono" value={formData.telefono} onChange={handleChange} style={styles.input} />
        </div>
        <button type="submit" style={styles.button}>Registrarse</button>
        <p style={styles.linkText}>
          ¿Ya tienes cuenta? <Link to="/login" style={styles.link}>Inicia sesión aquí</Link>
        </p>
      </form>
    </div>
  );
};

const styles = {
  container: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    minHeight: '100vh',
    backgroundColor: '#f0f2f5',
  },
  form: {
    backgroundColor: '#ffffff',
    padding: '40px',
    borderRadius: '8px',
    boxShadow: '0 4px 12px rgba(0, 0, 0, 0.1)',
    width: '100%',
    maxWidth: '500px',
    textAlign: 'center',
  },
  title: {
    marginBottom: '20px',
    color: '#333',
  },
  formGroup: {
    marginBottom: '15px',
    textAlign: 'left',
  },
  label: {
    display: 'block',
    marginBottom: '5px',
    color: '#555',
    fontWeight: 'bold',
  },
  input: {
    width: 'calc(100% - 20px)',
    padding: '10px',
    borderRadius: '4px',
    border: '1px solid #ddd',
    fontSize: '16px',
  },
  button: {
    width: '100%',
    padding: '12px',
    borderRadius: '4px',
    border: 'none',
    backgroundColor: '#28a745',
    color: 'white',
    fontSize: '18px',
    cursor: 'pointer',
    marginTop: '20px',
    transition: 'background-color 0.3s ease',
  },
  error: {
    color: '#dc3545',
    marginBottom: '15px',
    whiteSpace: 'pre-wrap', // Para mostrar saltos de línea en el error
  },
  success: {
    color: '#28a745',
    marginBottom: '15px',
  },
  linkText: {
    marginTop: '20px',
    color: '#666',
  },
  link: {
    color: '#007bff',
    textDecoration: 'none',
  },
};

export default RegisterPage;
