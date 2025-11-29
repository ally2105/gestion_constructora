using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using gestion_construccion.web.Models.ViewModels;

namespace gestion_construccion.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    /// <summary>
    /// Controller for managing products (CRUD) from the administration panel.
    /// </summary>
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Lists products, optionally filtered by a search term.
        /// </summary>
        /// <param name="searchTerm">Term to filter products by name or description.</param>
        /// <returns>View with the list of products.</returns>
        public async Task<IActionResult> Index(string searchTerm)
        {
            var productos = await _productoService.SearchProductosAsync(searchTerm);
            return View(productos);
        }

        /// <summary>
        /// Displays the form to create a new product.
        /// </summary>
        /// <returns>Creation view.</returns>
        public IActionResult Create()
        {
            return View(new ProductoViewModel());
        }

        /// <summary>
        /// Processes the creation of a new product.
        /// </summary>
        /// <param name="model">Product data.</param>
        /// <returns>Redirect to Index if successful, or view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var producto = new Producto
                    {
                        Nombre = model.Nombre,
                        Descripcion = model.Descripcion,
                        Precio = model.Precio,
                        Stock = model.Stock
                    };
                    await _productoService.AddProductoAsync(producto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el producto.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var producto = await _productoService.GetProductoByIdAsync(id.Value);
            if (producto == null) return NotFound();

            var viewModel = new ProductoViewModel
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var producto = await _productoService.GetProductoByIdAsync(id);
                    if (producto == null) return NotFound();

                    producto.Nombre = model.Nombre;
                    producto.Descripcion = model.Descripcion;
                    producto.Precio = model.Precio;
                    producto.Stock = model.Stock;

                    await _productoService.UpdateProductoAsync(producto);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _productoService.GetProductoByIdAsync(model.Id) == null)
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
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el producto.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var producto = await _productoService.GetProductoByIdAsync(id.Value);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productoService.DeleteProductoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var producto = await _productoService.GetProductoByIdAsync(id.Value);
            if (producto == null) return NotFound();
            return View(producto);
        }
    }
}
