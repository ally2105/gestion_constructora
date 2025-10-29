using Microsoft.AspNetCore.Mvc;

namespace gestion_construccion.Controllers;

public class ProductosController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}