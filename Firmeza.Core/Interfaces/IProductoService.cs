using Firmeza.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    /// <summary>
    /// Service for product-related operations.
    /// </summary>
    public interface IProductoService
    {
        /// <summary>
        /// Gets paginated products.
        /// </summary>
        Task<(IEnumerable<Producto> Productos, int TotalRecords)> GetAllProductosAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a product by its id.
        /// </summary>
        Task<Producto?> GetProductoByIdAsync(int id);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        Task<Producto> AddProductoAsync(Producto producto);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        Task<Producto?> UpdateProductoAsync(Producto producto);

        /// <summary>
        /// Deletes a product by its id.
        /// </summary>
        Task<bool> DeleteProductoAsync(int id);

        /// <summary>
        /// Searches products by name or description.
        /// </summary>
        Task<IEnumerable<Producto>> SearchProductosAsync(string searchTerm);
        
        /// <summary>
        /// Gets all products with stock greater than 0.
        /// </summary>
        /// <returns>List of available products.</returns>
        Task<IEnumerable<Producto>> GetProductosConStockAsync();
    }
}
