using Firmeza.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    public interface IProductoService
    {
        Task<(IEnumerable<Producto> Productos, int TotalRecords)> GetAllProductosAsync(int pageNumber, int pageSize);
        Task<Producto?> GetProductoByIdAsync(int id);
        Task<Producto> AddProductoAsync(Producto producto);
        Task<Producto?> UpdateProductoAsync(Producto producto);
        Task<bool> DeleteProductoAsync(int id);
        Task<IEnumerable<Producto>> SearchProductosAsync(string searchTerm);
    }
}
