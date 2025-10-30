using gestion_construccion.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace gestion_construccion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalProductos = (await _unitOfWork.Productos.GetAllAsync()).Count();
            var totalClientes = (await _unitOfWork.Clientes.GetAllAsync()).Count();
            var totalVentas = (await _unitOfWork.Ventas.GetAllAsync()).Count();

            ViewData["TotalProductos"] = totalProductos;
            ViewData["TotalClientes"] = totalClientes;
            ViewData["TotalVentas"] = totalVentas;

            return View();
        }
    }
}
