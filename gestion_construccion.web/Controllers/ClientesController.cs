using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using gestion_construccion.web.Models.ViewModels;
using gestion_construccion.web.Services;

namespace gestion_construccion.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string searchTerm)
        {
            var clientes = await _clienteService.SearchClientesAsync(searchTerm);
            return View(clientes);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _clienteService.AddClienteAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}");
                }
            }
            return View(model);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null || cliente.Usuario == null) return NotFound();

            // Mapear Cliente a ClienteEditViewModel.
            var viewModel = new ClienteEditViewModel
            {
                Email = cliente.Usuario.Email,
                Nombre = cliente.Usuario.Nombre,
                Identificacion = cliente.Usuario.Identificacion,
                FechaNacimiento = cliente.Usuario.FechaNacimiento,
                Direccion = cliente.Direccion
            };
            
            return View(viewModel);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // El servicio de actualización ahora recibirá el ClienteEditViewModel
                    var result = await _clienteService.UpdateClienteAsync(id, model);
                    if (result == null) return NotFound();
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}");
                }
            }
            return View(model);
        }

        // ... (resto de las acciones)
        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _clienteService.DeleteClienteAsync(id);
                if (!result) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return View(cliente);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}. Revise la consola del servidor para más detalles.");
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return View(cliente);
            }
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }
    }
}
