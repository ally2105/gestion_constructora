using Microsoft.AspNetCore.Mvc;

namespace gestion_construccion.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}