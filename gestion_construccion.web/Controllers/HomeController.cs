using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using gestion_construccion.web.Models;
using Microsoft.AspNetCore.Authorization;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Firmeza.Core.Models;
using System.Threading.Tasks;
using gestion_construccion.web.Models.ViewModels;
using Microsoft.EntityFrameworkCore; // Required for .CountAsync()

namespace gestion_construccion.web.Controllers;

[Authorize(Roles = "Administrador")]
/// <summary>
/// Main controller for the administration dashboard.
/// Provides summary views (dashboard) and static pages like Privacy and Error.
/// </summary>
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

    /// <summary>
    /// Displays the dashboard with aggregated metrics (total products, clients and sales).
    /// </summary>
    /// <returns>Dashboard view with <see cref="DashboardViewModel"/>.</returns>
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TotalProductos = await _unitOfWork.Productos.GetQuery().CountAsync(), // Fixed
            TotalClientes = await _unitOfWork.Clientes.GetQuery().CountAsync(),   // Fixed
            TotalVentas = await _unitOfWork.Ventas.GetQuery().CountAsync(),       // Fixed
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
