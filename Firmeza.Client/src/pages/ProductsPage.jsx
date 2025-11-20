import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext'; // Importar useCart
import { useNavigate } from 'react-router-dom';

const ProductsPage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { logout } = useAuth();
  const { addToCart } = useCart(); // Usar el hook useCart
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await api.get('/api/Productos'); // Cambiado
        setProducts(response.data);
      } catch (err) {
        console.error('Error al obtener productos:', err.response?.data || err.message);
        setError('No se pudieron cargar los productos. Por favor, intenta de nuevo.');
        if (err.response && err.response.status === 401) {
          logout();
          navigate('/login');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [logout, navigate]);

  const handleAddToCart = (product) => {
    addToCart(product);
    alert(`${product.nombre} añadido al carrito!`);
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (loading) return <p>Cargando productos...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div style={styles.container}>
      <h2 style={styles.title}>Catálogo de Productos</h2>
      <button onClick={handleLogout} style={styles.logoutButton}>Cerrar Sesión</button>
      <div style={styles.productsGrid}>
        {products.length > 0 ? (
          products.map((product) => (
            <div key={product.id} style={styles.productCard}>
              <h3 style={styles.productName}>{product.nombre}</h3>
              <p style={styles.productDescription}>{product.descripcion}</p>
              <p style={styles.productPrice}>Precio: ${product.precio.toFixed(2)}</p>
              <p style={styles.productStock}>Stock: {product.stock}</p>
              <button onClick={() => handleAddToCart(product)} style={styles.addToCartButton}>
                Agregar al Carrito
              </button>
            </div>
          ))
        ) : (
          <p>No hay productos disponibles.</p>
        )}
      </div>
    </div>
  );
};

const styles = {
  container: {
    padding: '20px',
    maxWidth: '1200px',
    margin: '0 auto',
  },
  title: {
    textAlign: 'center',
    color: '#333',
    marginBottom: '30px',
  },
  logoutButton: {
    backgroundColor: '#dc3545',
    color: 'white',
    padding: '10px 15px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    marginBottom: '20px',
    float: 'right',
  },
  productsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
    gap: '20px',
    marginTop: '20px',
  },
  productCard: {
    backgroundColor: '#ffffff',
    padding: '20px',
    borderRadius: '8px',
    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.1)',
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'space-between',
  },
  productName: {
    color: '#007bff',
    marginBottom: '10px',
  },
  productDescription: {
    fontSize: '0.9em',
    color: '#666',
    marginBottom: '10px',
  },
  productPrice: {
    fontWeight: 'bold',
    color: '#28a745',
    fontSize: '1.1em',
  },
  productStock: {
    fontSize: '0.85em',
    color: '#555',
  },
  addToCartButton: {
    backgroundColor: '#007bff',
    color: 'white',
    padding: '10px 15px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    marginTop: '15px',
    alignSelf: 'flex-start',
  },
};

export default ProductsPage;
