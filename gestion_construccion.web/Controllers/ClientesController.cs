using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Diagnostics;
using gestion_construccion.web.Models.ViewModels; // Se mantiene para los ViewModels
using Firmeza.Core.Interfaces; // Nuevo using para la interfaz IClienteService
using Firmeza.Core.Models; // Nuevo using para las entidades Cliente y Usuario

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
                    // Mapear ClienteViewModel a Cliente y Usuario
                    var usuario = new Usuario
                    {
                        Email = model.Email,
                        UserName = model.Email, // UserName suele ser el mismo que Email para Identity
                        Nombre = model.Nombre,
                        Identificacion = model.Identificacion,
                        FechaNacimiento = model.FechaNacimiento,
                        PhoneNumber = model.Telefono // Asignar el teléfono
                    };

                    var newCliente = new Cliente
                    {
                        Usuario = usuario,
                        Direccion = model.Direccion,
                        FechaRegistro = DateTime.UtcNow // Se puede establecer aquí o en el servicio
                    };

                    // Pasar la contraseña al servicio
                    await _clienteService.AddClienteAsync(newCliente, model.Password);
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
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

            // Mapear Cliente a ClienteEditViewModel para la vista
            var viewModel = new ClienteEditViewModel
            {
                Email = cliente.Usuario.Email,
                Nombre = cliente.Usuario.Nombre,
                Identificacion = cliente.Usuario.Identificacion,
                FechaNacimiento = cliente.Usuario.FechaNacimiento,
                Direccion = cliente.Direccion,
                Telefono = cliente.Usuario.PhoneNumber // Asignar el teléfono
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
                    // Mapear ClienteEditViewModel a Cliente y Usuario
                    var existingCliente = await _clienteService.GetClienteByIdAsync(id);
                    if (existingCliente == null || existingCliente.Usuario == null) return NotFound();

                    // Actualizar propiedades del Usuario
                    existingCliente.Usuario.Email = model.Email;
                    existingCliente.Usuario.UserName = model.Email;
                    existingCliente.Usuario.Nombre = model.Nombre;
                    existingCliente.Usuario.Identificacion = model.Identificacion;
                    existingCliente.Usuario.FechaNacimiento = model.FechaNacimiento;
                    existingCliente.Usuario.PhoneNumber = model.Telefono;

                    // Actualizar propiedades del Cliente
                    existingCliente.Direccion = model.Direccion;

                    var result = await _clienteService.UpdateClienteAsync(id, existingCliente);
                    if (result == null) return NotFound();
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}. Revise la consola del servidor para más detalles.");
                }
            }
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
