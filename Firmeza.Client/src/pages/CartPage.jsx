import React from 'react';
import { useCart } from '../context/CartContext';
import { useNavigate } from 'react-router-dom';

const CartPage = () => {
  const { cartItems, updateQuantity, removeFromCart, calculateTotals, clearCart } = useCart();
  const navigate = useNavigate();
  const { subtotal, tax, total } = calculateTotals();

  const handleCheckout = () => {
    // Aquí iría la lógica para procesar la compra
    alert('Procesando compra...');
    clearCart();
    navigate('/products');
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.title}>Tu Carrito de Compras</h2>
      {cartItems.length === 0 ? (
        <p style={styles.emptyCart}>El carrito está vacío. <span style={styles.link} onClick={() => navigate('/products')}>¡Añade algunos productos!</span></p>
      ) : (
        <div>
          <div style={styles.cartItemsGrid}>
            {cartItems.map((item) => (
              <div key={item.id} style={styles.cartItemCard}>
                <h3 style={styles.itemName}>{item.nombre}</h3>
                <p style={styles.itemPrice}>Precio Unitario: ${item.precio.toFixed(2)}</p>
                <div style={styles.quantityControl}>
                  <button onClick={() => updateQuantity(item.id, item.quantity - 1)} style={styles.quantityButton}>-</button>
                  <span style={styles.itemQuantity}>{item.quantity}</span>
                  <button onClick={() => updateQuantity(item.id, item.quantity + 1)} style={styles.quantityButton}>+</button>
                </div>
                <p style={styles.itemSubtotal}>Subtotal: ${(item.precio * item.quantity).toFixed(2)}</p>
                <button onClick={() => removeFromCart(item.id)} style={styles.removeButton}>Eliminar</button>
              </div>
            ))}
          </div>
          <div style={styles.totalsSummary}>
            <p>Subtotal: ${subtotal.toFixed(2)}</p>
            <p>IVA (19%): ${tax.toFixed(2)}</p>
            <h3>Total: ${total.toFixed(2)}</h3>
            <button onClick={handleCheckout} style={styles.checkoutButton}>Proceder al Pago</button>
            <button onClick={clearCart} style={styles.clearCartButton}>Vaciar Carrito</button>
          </div>
        </div>
      )}
      <button onClick={() => navigate('/products')} style={styles.backToProductsButton}>Volver a Productos</button>
    </div>
  );
};

const styles = {
  container: {
    padding: '20px',
    maxWidth: '900px',
    margin: '0 auto',
  },
  title: {
    textAlign: 'center',
    color: '#333',
    marginBottom: '30px',
  },
  emptyCart: {
    textAlign: 'center',
    fontSize: '1.2em',
    color: '#666',
  },
  link: {
    color: '#007bff',
    cursor: 'pointer',
    textDecoration: 'underline',
  },
  cartItemsGrid: {
    display: 'grid',
    gridTemplateColumns: '1fr',
    gap: '15px',
    marginBottom: '30px',
  },
  cartItemCard: {
    backgroundColor: '#ffffff',
    padding: '15px',
    borderRadius: '8px',
    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.05)',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    flexWrap: 'wrap',
  },
  itemName: {
    margin: '0',
    color: '#007bff',
    flexBasis: '100%',
    marginBottom: '5px',
  },
  itemPrice: {
    margin: '0',
    color: '#555',
    flexBasis: '100%',
    marginBottom: '10px',
  },
  quantityControl: {
    display: 'flex',
    alignItems: 'center',
  },
  quantityButton: {
    backgroundColor: '#007bff',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    width: '30px',
    height: '30px',
    fontSize: '1.1em',
    cursor: 'pointer',
    margin: '0 5px',
  },
  itemQuantity: {
    fontSize: '1.1em',
    fontWeight: 'bold',
  },
  itemSubtotal: {
    fontWeight: 'bold',
    color: '#28a745',
    fontSize: '1.1em',
    marginLeft: 'auto',
  },
  removeButton: {
    backgroundColor: '#dc3545',
    color: 'white',
    padding: '8px 12px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    marginLeft: '15px',
  },
  totalsSummary: {
    backgroundColor: '#e9ecef',
    padding: '20px',
    borderRadius: '8px',
    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.05)',
    textAlign: 'right',
    marginBottom: '20px',
  },
  checkoutButton: {
    backgroundColor: '#28a745',
    color: 'white',
    padding: '12px 20px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    fontSize: '1.1em',
    marginTop: '15px',
    marginRight: '10px',
  },
  clearCartButton: {
    backgroundColor: '#6c757d',
    color: 'white',
    padding: '12px 20px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    fontSize: '1.1em',
    marginTop: '15px',
  },
  backToProductsButton: {
    backgroundColor: '#6c757d',
    color: 'white',
    padding: '10px 15px',
    borderRadius: '5px',
    border: 'none',
    cursor: 'pointer',
    marginTop: '20px',
  }
};

export default CartPage;
