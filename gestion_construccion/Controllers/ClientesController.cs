using gestion_construccion.Models.ViewModels;
using gestion_construccion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace gestion_construccion.Controllers
{
    // Atributo que protege todo el controlador. Solo los usuarios con el rol "Administrador" pueden acceder a estas acciones.
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        // Dependencia de la capa de servicio. El controlador no sabe cómo se hacen las cosas, solo a quién pedírselas.
        private readonly IClienteService _clienteService;

        // El constructor recibe el servicio a través de inyección de dependencias.
        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // Acción para mostrar la lista de clientes (y resultados de búsqueda).
        // GET: /Clientes o /Clientes?searchTerm=texto
        public async Task<IActionResult> Index(string searchTerm)
        {
            // Delega la lógica de búsqueda al servicio.
            var clientes = await _clienteService.SearchClientesAsync(searchTerm);
            // Devuelve la vista "Index.cshtml" pasándole la lista de clientes para que la muestre.
            return View(clientes);
        }

        // Acción para mostrar el formulario de creación de un nuevo cliente.
        // GET: /Clientes/Create
        public IActionResult Create()
        {
            // Simplemente devuelve la vista "Create.cshtml".
            return View();
        }

        // Acción que procesa los datos del formulario de creación cuando se envía.
        // POST: /Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Previene ataques de tipo Cross-Site Request Forgery (CSRF).
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            // Verifica si los datos recibidos en el 'model' cumplen con las validaciones (ej: [Required], [EmailAddress]).
            if (ModelState.IsValid)
            {
                try
                {
                    // Delega toda la lógica de creación del cliente y su usuario al servicio.
                    await _clienteService.AddClienteAsync(model);
                    // Si todo va bien, redirige al usuario a la lista de clientes.
                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex) // Captura errores de negocio específicos lanzados por el servicio.
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex) // Captura cualquier otro error inesperado.
                {
                    Debug.WriteLine(ex.ToString()); // Imprime el error completo en la consola de depuración.
                    ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}. Revise la consola del servidor para más detalles.");
                }
            }
            // Si el ModelState no es válido, o si ocurrió un error, se vuelve a mostrar el formulario con los datos y los mensajes de error.
            return View(model);
        }

        // Acción para mostrar el formulario de edición de un cliente existente.
        // GET: /Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound(); // Si no se proporciona un ID, devuelve un error 404.
            
            // Pide al servicio que busque el cliente por su ID.
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null || cliente.Usuario == null) return NotFound(); // Si no se encuentra, devuelve un error 404.

            // Mapea los datos del objeto 'Cliente' a un 'ClienteViewModel' para rellenar el formulario.
            // Esto desacopla la vista del modelo de la base de datos.
            var viewModel = new ClienteViewModel
            {
                Email = cliente.Usuario.Email,
                Nombre = cliente.Usuario.Nombre,
                Identificacion = cliente.Usuario.Identificacion,
                FechaNacimiento = cliente.Usuario.FechaNacimiento,
                Direccion = cliente.Direccion
                // La contraseña no se carga en el formulario de edición por seguridad.
            };
            
            // Devuelve la vista "Edit.cshtml" con los datos del cliente.
            return View(viewModel);
        }

        // Acción que procesa los datos del formulario de edición cuando se envía.
        // POST: /Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Delega la lógica de actualización al servicio.
                    var result = await _clienteService.UpdateClienteAsync(id, model);
                    if (result == null) return NotFound(); // Si el servicio no encontró el cliente, devuelve 404.
                    
                    // Si la actualización es exitosa, redirige a la lista de clientes.
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
            // Si hay errores, se vuelve a mostrar el formulario de edición.
            return View(model);
        }

        // Acción para mostrar la página de confirmación de eliminación.
        // GET: /Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            // Devuelve la vista "Delete.cshtml" con los datos del cliente a eliminar.
            return View(cliente);
        }

        // Acción que efectúa la eliminación después de la confirmación.
        // POST: /Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Delega la lógica de eliminación al servicio.
                var result = await _clienteService.DeleteClienteAsync(id);
                if (!result) return NotFound(); // Si el servicio no encontró el cliente, devuelve 404.
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return View(cliente); // Vuelve a la vista de eliminación con el error.
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}. Revise la consola del servidor para más detalles.");
                var cliente = await _clienteService.GetClienteByIdAsync(id);
                return View(cliente);
            }
        }

        // Acción para mostrar los detalles de un cliente.
        // GET: /Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cliente = await _clienteService.GetClienteByIdAsync(id.Value);
            if (cliente == null) return NotFound();
            // Devuelve la vista "Details.cshtml" con los datos del cliente.
            return View(cliente);
        }
    }
}
