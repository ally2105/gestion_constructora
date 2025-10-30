using gestion_construccion.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllProductosAsync();
        Task<Producto?> GetProductoByIdAsync(int id);
        Task<Producto> AddProductoAsync(Producto producto);
        Task<Producto?> UpdateProductoAsync(Producto producto);
        Task<bool> DeleteProductoAsync(int id);
        Task<IEnumerable<Producto>> SearchProductosAsync(string searchTerm);
    }
}
