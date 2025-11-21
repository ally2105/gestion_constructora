using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;

namespace Firmeza.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<Producto> Productos, int TotalRecords)> GetAllProductosAsync(int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Productos.GetQuery();
            var totalRecords = await query.CountAsync();
            var productos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (productos, totalRecords);
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
                var (productos, _) = await GetAllProductosAsync(1, 50); // Devuelve la primera página en búsqueda vacía
                return productos;
            }

            return await _unitOfWork.Productos.FindAsync(p => 
                p.Nombre.Contains(searchTerm) || 
                (p.Descripcion != null && p.Descripcion.Contains(searchTerm)));
        }
    }
}
