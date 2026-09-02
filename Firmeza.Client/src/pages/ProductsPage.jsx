import React, { useEffect, useState, useCallback, useMemo } from 'react';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import ProductCard from '../components/ProductCard';
import Pagination from '../components/Pagination';
import HeroBanner from '../components/HeroBanner';
import Footer from '../components/Footer';
import '../styles/ProductsPage.css';

const ProductsPage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [searchTerm, setSearchTerm] = useState('');
  const [viewMode, setViewMode] = useState('grid'); // 'grid' | 'list'
  const [sortBy, setSortBy] = useState('default'); // 'default' | 'price-asc' | 'price-desc' | 'name'
  const [stockFilter, setStockFilter] = useState('all'); // 'all' | 'in-stock'
  const { logout } = useAuth();
  const navigate = useNavigate();

  const fetchProducts = useCallback(async (page) => {
    setLoading(true);
    try {
      const response = await api.get('/api/Productos', {
        params: { pageNumber: page, pageSize: 12 }
      });
      setProducts(response.data.data || response.data || []);
      setTotalPages(response.data.totalPages || 1);
      setCurrentPage(response.data.pageNumber || page);
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
      document.getElementById('products-section')?.scrollIntoView({ behavior: 'smooth' });
    }
  };

  const processedProducts = useMemo(() => {
    let result = products.filter(product => {
      const matchesSearch =
        product.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
        product.descripcion.toLowerCase().includes(searchTerm.toLowerCase());
      const matchesStock = stockFilter === 'all' || (stockFilter === 'in-stock' && product.stock > 0);
      return matchesSearch && matchesStock;
    });

    if (sortBy === 'price-asc') {
      result.sort((a, b) => a.precio - b.precio);
    } else if (sortBy === 'price-desc') {
      result.sort((a, b) => b.precio - a.precio);
    } else if (sortBy === 'name') {
      result.sort((a, b) => a.nombre.localeCompare(b.nombre));
    }

    return result;
  }, [products, searchTerm, stockFilter, sortBy]);

  return (
    <div className="products-page-wrapper">
      {/* Hero Banner */}
      <HeroBanner />

      <div className="products-container" id="products-section">
        {/* Header Controls */}
        <div className="products-header">
          <div className="header-text">
            <h2 className="products-title">Catálogo de Productos</h2>
            <p className="products-subtitle">
              Explora nuestra amplia gama de materiales de construcción con garantía y envío directo
            </p>
          </div>

          <div className="products-controls">
            {/* Search Box */}
            <div className="search-box">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <circle cx="11" cy="11" r="8"></circle>
                <path d="m21 21-4.35-4.35"></path>
              </svg>
              <input
                type="text"
                placeholder="Buscar por nombre o descripción..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="search-input"
                id="search-products-input"
              />
              {searchTerm && (
                <button className="clear-search" onClick={() => setSearchTerm('')} title="Limpiar búsqueda">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <line x1="18" y1="6" x2="6" y2="18"></line>
                    <line x1="6" y1="6" x2="18" y2="18"></line>
                  </svg>
                </button>
              )}
            </div>

            {/* Sort Dropdown */}
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
              className="filter-select"
              id="sort-products-select"
            >
              <option value="default">Ordenar por: Relevancia</option>
              <option value="price-asc">Precio: Menor a Mayor</option>
              <option value="price-desc">Precio: Mayor a Menor</option>
              <option value="name">Nombre: A-Z</option>
            </select>

            {/* Stock Filter Dropdown */}
            <select
              value={stockFilter}
              onChange={(e) => setStockFilter(e.target.value)}
              className="filter-select"
              id="stock-filter-select"
            >
              <option value="all">Todos los productos</option>
              <option value="in-stock">Solo disponible</option>
            </select>

            {/* View Mode Toggle */}
            <div className="view-toggle">
              <button
                className={`view-btn ${viewMode === 'grid' ? 'active' : ''}`}
                onClick={() => setViewMode('grid')}
                title="Vista en cuadrícula"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <rect x="3" y="3" width="7" height="7"></rect>
                  <rect x="14" y="3" width="7" height="7"></rect>
                  <rect x="14" y="14" width="7" height="7"></rect>
                  <rect x="3" y="14" width="7" height="7"></rect>
                </svg>
              </button>
              <button
                className={`view-btn ${viewMode === 'list' ? 'active' : ''}`}
                onClick={() => setViewMode('list')}
                title="Vista en lista"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <line x1="8" y1="6" x2="21" y2="6"></line>
                  <line x1="8" y1="12" x2="21" y2="12"></line>
                  <line x1="8" y1="18" x2="21" y2="18"></line>
                  <line x1="3" y1="6" x2="3.01" y2="6"></line>
                  <line x1="3" y1="12" x2="3.01" y2="12"></line>
                  <line x1="3" y1="18" x2="3.01" y2="18"></line>
                </svg>
              </button>
            </div>
          </div>
        </div>

        {/* Error View */}
        {error && (
          <div className="error-message">
            <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="12" cy="12" r="10"></circle>
              <line x1="12" y1="8" x2="12" y2="12"></line>
              <line x1="12" y1="16" x2="12.01" y2="16"></line>
            </svg>
            <p>{error}</p>
            <button className="btn btn-primary" onClick={() => fetchProducts(currentPage)}>Reintentar</button>
          </div>
        )}

        {/* Loading Skeleton */}
        {loading ? (
          <div className="products-grid grid">
            {[1, 2, 3, 4, 5, 6].map(n => (
              <div key={n} className="product-skeleton">
                <div className="skeleton-image"></div>
                <div className="skeleton-content">
                  <div className="skeleton-title"></div>
                  <div className="skeleton-text"></div>
                  <div className="skeleton-footer"></div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <>
            {processedProducts.length > 0 ? (
              <>
                <div className={`products-grid ${viewMode}`}>
                  {processedProducts.map((product) => (
                    <ProductCard key={product.id} product={product} viewMode={viewMode} />
                  ))}
                </div>

                {!searchTerm && totalPages > 1 && (
                  <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={handlePageChange}
                  />
                )}
              </>
            ) : (
              <div className="empty-state">
                <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.35-4.35"></path>
                </svg>
                <h3>No se encontraron productos</h3>
                <p>
                  {searchTerm
                    ? `No hay coincidencias para "${searchTerm}"`
                    : 'No hay productos disponibles en este momento'}
                </p>
                {(searchTerm || stockFilter !== 'all') && (
                  <button
                    className="btn btn-primary"
                    onClick={() => {
                      setSearchTerm('');
                      setStockFilter('all');
                      setSortBy('default');
                    }}
                  >
                    Restablecer Filtros
                  </button>
                )}
              </div>
            )}
          </>
        )}
      </div>

      {/* Global Footer */}
      <Footer />
    </div>
  );
};

export default ProductsPage;
