using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmeza.Core.Interfaces;
using System.Threading.Tasks;
using gestion_construccion.web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace gestion_construccion.web.Controllers
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
            var model = new DashboardViewModel
            {
                TotalProductos = await _unitOfWork.Productos.GetQuery().CountAsync(),
                TotalClientes = await _unitOfWork.Clientes.GetQuery().CountAsync(),
                TotalVentas = await _unitOfWork.Ventas.GetQuery().CountAsync(),
            };
            return View(model);
        }
    }
}
