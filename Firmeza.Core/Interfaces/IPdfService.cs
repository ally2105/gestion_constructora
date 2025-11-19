using Firmeza.Core.Models; // Nuevo using para Venta
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    public interface IPdfService
    {
        Task<string> GenerarReciboVentaAsync(Venta venta);
    }
}
