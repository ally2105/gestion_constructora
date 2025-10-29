using Microsoft.AspNetCore.Mvc;

namespace gestion_construccion.Controllers;

public class VentasController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}