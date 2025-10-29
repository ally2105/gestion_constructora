using Microsoft.AspNetCore.Mvc;

namespace gestion_construccion.Controllers;

public class ClientesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}