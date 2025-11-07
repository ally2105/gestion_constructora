using System.Collections.Generic;
using System.Threading.Tasks;
using gestion_construccion.web.Models;

namespace gestion_construccion.web.Services
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
