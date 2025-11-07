using gestion_construccion.web.Models;
using gestion_construccion.web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers
{
    // Atributo que indica que esta clase es un controlador de API.
    [ApiController]
    // Define la ruta base para este controlador. En este caso, será "/api/Productos".
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        // Dependencia del servicio de productos.
        private readonly IProductoService _productoService;

        // El constructor recibe el servicio a través de inyección de dependencias.
        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // Acción para obtener todos los productos.
        // GET: /api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAllProductos()
        {
            try
            {
                // Llama al servicio para obtener los productos.
                var productos = await _productoService.GetAllProductosAsync();
                // Devuelve una respuesta HTTP 200 OK con la lista de productos en formato JSON.
                return Ok(productos);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, devuelve una respuesta HTTP 500 Internal Server Error con el mensaje.
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }

        // Acción para obtener un producto por su ID.
        // GET: /api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProductoById(int id)
        {
            try
            {
                var producto = await _productoService.GetProductoByIdAsync(id);
                if (producto == null)
                {
                    // Si no se encuentra el producto, devuelve un error 404 Not Found.
                    return NotFound();
                }
                // Devuelve una respuesta HTTP 200 OK con el producto encontrado.
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }
    }
}
