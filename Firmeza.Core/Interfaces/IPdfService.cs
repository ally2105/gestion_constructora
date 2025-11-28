using Firmeza.Core.Models;
using System.Threading.Tasks;

namespace Firmeza.Core.Interfaces
{
    public interface IPdfService
    {
        Task<string> GenerarReciboVentaAsync(Venta venta, string basePath);
    }
}
