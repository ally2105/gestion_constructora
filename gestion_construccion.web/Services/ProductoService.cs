using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gestion_construccion.web.Models;
using gestion_construccion.web.Repositories;

namespace gestion_construccion.web.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            return await _unitOfWork.Productos.GetAllAsync();
        }

        public async Task<Producto?> GetProductoByIdAsync(int id)
        {
            return await _unitOfWork.Productos.GetByIdAsync(id);
        }

        public async Task<Producto> AddProductoAsync(Producto producto)
        {
            await _unitOfWork.Productos.AddAsync(producto);
            await _unitOfWork.CompleteAsync();
            return producto;
        }

        public async Task<Producto?> UpdateProductoAsync(Producto producto)
        {
            _unitOfWork.Productos.Update(producto);
            await _unitOfWork.CompleteAsync();
            return producto;
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            var producto = await _unitOfWork.Productos.GetByIdAsync(id);
            if (producto == null) return false;

            _unitOfWork.Productos.Remove(producto);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<Producto>> SearchProductosAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllProductosAsync();
            }

            return await _unitOfWork.Productos.FindAsync(p => 
                p.Nombre.Contains(searchTerm) || 
                (p.Descripcion != null && p.Descripcion.Contains(searchTerm)));
        }
    }
}
