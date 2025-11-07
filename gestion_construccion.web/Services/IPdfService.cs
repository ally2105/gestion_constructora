using gestion_construccion.web.Models;
using System.Threading.Tasks;

namespace gestion_construccion.web.Services
{
    public interface IPdfService
    {
        Task<string> GenerarReciboVentaAsync(Venta venta);
    }
}
