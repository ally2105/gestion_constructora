using gestion_construccion.Models;
using gestion_construccion.Models.ViewModels;
using gestion_construccion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // <-- AÑADIDO

namespace gestion_construccion.Controllers
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
                    // Crear el objeto Cliente a partir del ViewModel
                    var cliente = new Cliente(0, model.Direccion) // El UsuarioId se asignará en el servicio
                    {
                        FechaRegistro = DateTime.UtcNow // Se puede inicializar aquí o en el servicio
                    };
                    // Crear un objeto Usuario temporal para pasar los datos al servicio
                    cliente.Usuario = new Usuario // Inicializar Usuario aquí
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        Nombre = model.Nombre,
                        Identificacion = model.Identificacion,
                        FechaNacimiento = model.FechaNacimiento
                    };

                    await _clienteService.AddClienteAsync(cliente, model.Password);
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el cliente.");
                }
            }
            return View(model);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();

            // Mapear Cliente a ClienteViewModel para la edición
            var viewModel = new ClienteViewModel
            {
                Email = cliente.Usuario.Email,
                Nombre = cliente.Usuario.Nombre,
                Identificacion = cliente.Usuario.Identificacion,
                FechaNacimiento = cliente.Usuario.FechaNacimiento,
                Direccion = cliente.Direccion
                // La contraseña no se carga para edición por seguridad
            };
            ViewData["ClienteId"] = cliente.Id; // Pasar el ID del cliente para el POST
            return View(viewModel);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            // Recuperar el cliente original para obtener el UsuarioId
            var existingCliente = await _clienteService.GetClienteByIdAsync(id);
            if (existingCliente == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar el objeto Cliente a partir del ViewModel
                    existingCliente.Direccion = model.Direccion;
                    // Actualizar el objeto Usuario asociado
                    existingCliente.Usuario.Email = model.Email;
                    existingCliente.Usuario.UserName = model.Email;
                    existingCliente.Usuario.Nombre = model.Nombre;
                    existingCliente.Usuario.Identificacion = model.Identificacion;
                    existingCliente.Usuario.FechaNacimiento = model.FechaNacimiento;

                    await _clienteService.UpdateClienteAsync(existingCliente);
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _clienteService.GetClienteByIdAsync(id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el cliente.");
                }
            }
            ViewData["ClienteId"] = id; // Mantener el ID en caso de error de validación
            return View(model);
        }

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
                return View(cliente); // Volver a la vista de eliminación con el error
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al eliminar el cliente.");
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return View(cliente); // Volver a la vista de eliminación con el error
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
