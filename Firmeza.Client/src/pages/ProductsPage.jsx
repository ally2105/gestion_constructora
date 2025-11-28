import React, { useEffect, useState, useCallback } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import ProductCard from '../components/ProductCard';
import Pagination from '../components/Pagination';

const ProductsPage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const { logout } = useAuth();
  const navigate = useNavigate();

  const fetchProducts = useCallback(async (page) => {
    setLoading(true);
    try {
      const response = await api.get('/api/Productos', {
        params: { pageNumber: page, pageSize: 6 }
      });
      setProducts(response.data.data);
      setTotalPages(response.data.totalPages);
      setCurrentPage(response.data.pageNumber);
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
  }, [logout, navigate]);

  useEffect(() => {
    fetchProducts(currentPage);
  }, [fetchProducts, currentPage]);

  const handlePageChange = (page) => {
    if (page > 0 && page <= totalPages) {
      setCurrentPage(page);
    }
  };

  if (error) return <p style={{ textAlign: 'center', color: 'red' }}>{error}</p>;

  return (
    <div className="main-container">
      <h1 className="page-title">Catálogo de Productos</h1>
      {loading ? (
        <p style={{ textAlign: 'center' }}>Cargando productos...</p>
      ) : (
        <>
          <div className="products-grid">
            {products.length > 0 ? (
              products.map((product) => (
                <ProductCard key={product.id} product={product} />
              ))
            ) : (
              <p style={{ textAlign: 'center' }}>No hay productos disponibles.</p>
            )}
          </div>
          <Pagination 
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
};

export default ProductsPage;
