using gestion_construccion.web.Models.ViewModels;
using gestion_construccion.web.Repositories;
using gestion_construccion.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace gestion_construccion.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VentasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; // Mantenemos UnitOfWork para vistas de solo lectura.
        private readonly IVentaService _ventaService;

        public VentasController(IUnitOfWork unitOfWork, IVentaService ventaService)
        {
            _unitOfWork = unitOfWork;
            _ventaService = ventaService;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = await _unitOfWork.Ventas
                                    .GetQuery()
                                    .Include(v => v.Cliente!)
                                    .ThenInclude(c => c.Usuario!)
                                    .ToListAsync();
            return View(ventas);
        }

        // GET: Ventas/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VentaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ventaService.CrearVentaAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear la venta.");
                }
            }
            // Si hay un error, volver a poblar los dropdowns y mostrar el formulario.
            await PopulateDropdowns();
            return View(model);
        }

        // GET: Ventas/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id == null) return NotFound();
            
            var venta = await _unitOfWork.Ventas
                                    .GetQuery()
                                    .Include(v => v.Cliente!)
                                    .ThenInclude(c => c.Usuario!)
                                    .Include(v => v.Detalles!)
                                    .ThenInclude(d => d.Producto!)
                                    .FirstOrDefaultAsync(v => v.Id == id.Value);

            if (venta == null) return NotFound();

            return View(venta);
        }

        // Método auxiliar para poblar los DropDownLists de Clientes y Productos.
        private async Task PopulateDropdowns()
        {
            var clientes = await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).ToListAsync();
            ViewBag.ClienteId = new SelectList(clientes, "Id", "Usuario.Nombre");

            var productos = await _unitOfWork.Productos.GetQuery().Where(p => p.Stock > 0).ToListAsync();
            ViewBag.ProductoId = new SelectList(productos, "Id", "Nombre");
        }
    }
}
