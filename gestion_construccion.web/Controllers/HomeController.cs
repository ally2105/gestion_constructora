using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using gestion_construccion.web.Models;
using Microsoft.AspNetCore.Authorization;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Firmeza.Core.Models;
using System.Threading.Tasks;
using gestion_construccion.web.Models.ViewModels;
using Microsoft.EntityFrameworkCore; // Necesario para .CountAsync()

namespace gestion_construccion.web.Controllers;

[Authorize(Roles = "Administrador")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<Usuario> _userManager;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<Usuario> userManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TotalProductos = await _unitOfWork.Productos.GetQuery().CountAsync(), // Corregido
            TotalClientes = await _unitOfWork.Clientes.GetQuery().CountAsync(),   // Corregido
            TotalVentas = await _unitOfWork.Ventas.GetQuery().CountAsync(),       // Corregido
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
