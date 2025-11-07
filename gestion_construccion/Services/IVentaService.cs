using gestion_construccion.Models;
using gestion_construccion.Models.ViewModels;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    public interface IVentaService
    {
        Task<Venta> CrearVentaAsync(VentaViewModel model);
    }
}
