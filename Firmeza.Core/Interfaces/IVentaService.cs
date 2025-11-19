using Firmeza.Core.Models;
using Firmeza.Core.DTOs; // Nuevo using para VentaCreateDto
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    public interface IVentaService
    {
        Task<Venta> CrearVentaAsync(VentaCreateDto model); // Cambiado a VentaCreateDto
    }
}
