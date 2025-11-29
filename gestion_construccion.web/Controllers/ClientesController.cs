using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Diagnostics;
using gestion_construccion.web.Models.ViewModels;
using Firmeza.Core.Interfaces;
using Firmeza.Core.Models;

namespace gestion_construccion.web.Controllers
{
    /// <summary>
    /// Controller for client management (CRUD).
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Displays the list of clients with search option.
        /// </summary>
        /// <param name="searchTerm">Search term (name or identification).</param>
        /// <returns>View with the list of clients.</returns>
        public async Task<IActionResult> Index(string searchTerm)
        {
            var clientes = await _clienteService.SearchClientesAsync(searchTerm);
            return View(clientes);
        }

        /// <summary>
        /// Displays the form to create a new client.
        /// </summary>
        /// <returns>Creation view.</returns>
        public IActionResult Create()
        {
            return View(new ClienteViewModel());
        }

    /// <summary>
    /// Processes the creation of a new client.
    /// </summary>
    /// <param name="model">Client data.</param>
    /// <returns>Redirect to Index if successful, or view with errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClienteViewModel model)
        {
            // Explicit validation for password on creation
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = new Usuario
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        Nombre = model.Nombre,
                        Identificacion = model.Identificacion,
                        FechaNacimiento = model.FechaNacimiento,
                        PhoneNumber = model.Telefono
                    };

                    var newCliente = new Cliente
                    {
                        Usuario = usuario,
                        Direccion = model.Direccion,
                        FechaRegistro = DateTime.UtcNow
                    };

                    await _clienteService.AddClienteAsync(newCliente, model.Password!);
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Displays the form to edit an existing client.
        /// </summary>
        /// <param name="id">Client ID.</param>
        /// <returns>Edit view.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null || cliente.Usuario == null) return NotFound();

            var viewModel = new ClienteViewModel
            {
                Id = cliente.Id,
                Email = cliente.Usuario.Email!,
                Nombre = cliente.Usuario.Nombre,
                Identificacion = cliente.Usuario.Identificacion,
                FechaNacimiento = cliente.Usuario.FechaNacimiento,
                Direccion = cliente.Direccion,
                Telefono = cliente.Usuario.PhoneNumber
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            if (id != model.Id) return NotFound();

            // As the password is optional in edit, we remove it from ModelState
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCliente = await _clienteService.GetClienteByIdAsync(id);
                    if (existingCliente == null || existingCliente.Usuario == null) return NotFound();

                    existingCliente.Usuario.Email = model.Email;
                    existingCliente.Usuario.UserName = model.Email;
                    existingCliente.Usuario.Nombre = model.Nombre;
                    existingCliente.Usuario.Identificacion = model.Identificacion;
                    existingCliente.Usuario.FechaNacimiento = model.FechaNacimiento;
                    existingCliente.Usuario.PhoneNumber = model.Telefono;
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
                    ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Displays the confirmation to delete a client.
        /// </summary>
        /// <param name="id">Client ID.</param>
        /// <returns>Delete view.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

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
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}.");
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return View(cliente);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            return View(cliente);
        }
    }
}
