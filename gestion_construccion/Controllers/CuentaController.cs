using Microsoft.AspNetCore.Mvc;

namespace gestion_construccion.Controllers;

public class CuentaController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}