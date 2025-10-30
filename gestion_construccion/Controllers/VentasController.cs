using gestion_construccion.Models;
using gestion_construccion.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace gestion_construccion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VentasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VentasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            // Cargar ventas incluyendo Cliente y Usuario asociado
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
            // Necesitarás una lista de clientes para el Dropdown
            // Cargar clientes incluyendo su Usuario para mostrar el nombre
            var clientes = await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).ToListAsync();
            ViewBag.ClienteId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(clientes, "Id", "Usuario.Nombre");
            return View();
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClienteId,Fecha,Total")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _unitOfWork.Ventas.AddAsync(venta);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al crear la venta.");
                }
            }
            var clientes = await _unitOfWork.Clientes.GetQuery().Include(c => c.Usuario!).ToListAsync();
            ViewBag.ClienteId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(clientes, "Id", "Usuario.Nombre", venta.ClienteId);
            return View(venta);
        }

        // GET: Ventas/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id == null) return NotFound();
            
            // Cargar venta incluyendo Cliente, Usuario y Detalles con Producto
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
    }
}
