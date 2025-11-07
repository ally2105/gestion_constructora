using Microsoft.AspNetCore.Mvc;

namespace gestion_construccion.web.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}