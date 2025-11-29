using Firmeza.Core.Models;
using Firmeza.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    /// <summary>
    /// Interface for sale management service.
    /// </summary>
    public interface IVentaService
    {
        /// <summary>
        /// Creates a new sale from a DTO.
        /// </summary>
        /// <param name="model">Sale data to create.</param>
        /// <returns>The created sale.</returns>
        Task<Venta> CrearVentaAsync(VentaCreateDto model);

        /// <summary>
        /// Gets all registered sales.
        /// </summary>
        /// <returns>List of sales.</returns>
        Task<IEnumerable<Venta>> GetAllVentasAsync();

        /// <summary>
        /// Gets a sale by its ID.
        /// </summary>
        /// <param name="id">Sale ID.</param>
        /// <returns>The found sale or null.</returns>
        Task<Venta?> GetVentaByIdAsync(int id);

        /// <summary>
        /// Updates an existing sale.
        /// </summary>
        /// <param name="venta">Sale to update.</param>
        Task UpdateVentaAsync(Venta venta);
    }
}
