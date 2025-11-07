using gestion_construccion.web.Models;
using gestion_construccion.web.Models.ViewModels;
using System.Threading.Tasks;

namespace gestion_construccion.web.Services
{
    public interface IVentaService
    {
        Task<Venta> CrearVentaAsync(VentaViewModel model);
    }
}
