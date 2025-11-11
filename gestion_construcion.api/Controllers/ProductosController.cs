using AutoMapper;
using Firmeza.Api.DTOs;
using gestion_construccion.web.Models;
using gestion_construccion.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly IMapper _mapper;

        public ProductosController(IProductoService productoService, IMapper mapper)
        {
            _productoService = productoService;
            _mapper = mapper;
        }

        // GET: /api/Productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAllProductos()
        {
            var productos = await _productoService.GetAllProductosAsync();
            // Mapea la lista de entidades Producto a una lista de ProductoDto.
            var productosDto = _mapper.Map<IEnumerable<ProductoDto>>(productos);
            return Ok(productosDto);
        }

        // GET: /api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProductoById(int id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            // Mapea la entidad Producto a un ProductoDto.
            var productoDto = _mapper.Map<ProductoDto>(producto);
            return Ok(productoDto);
        }

        // POST: /api/Productos
        [HttpPost]
        [Authorize(Roles = "Administrador")] // Solo los administradores pueden crear productos.
        public async Task<ActionResult<ProductoDto>> CreateProducto([FromBody] ProductoCreateDto productoCreateDto)
        {
            // Mapea el DTO de creación a la entidad Producto.
            var producto = _mapper.Map<Producto>(productoCreateDto);
            
            var nuevoProducto = await _productoService.AddProductoAsync(producto);

            // Mapea el producto recién creado a un DTO para devolverlo en la respuesta.
            var productoDto = _mapper.Map<ProductoDto>(nuevoProducto);

            // Devuelve una respuesta 201 Created con la ubicación del nuevo recurso y el recurso mismo.
            return CreatedAtAction(nameof(GetProductoById), new { id = productoDto.Id }, productoDto);
        }

        // PUT: /api/Productos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateProducto(int id, [FromBody] ProductoCreateDto productoUpdateDto)
        {
            var productoExistente = await _productoService.GetProductoByIdAsync(id);
            if (productoExistente == null)
            {
                return NotFound();
            }

            // Mapea los datos del DTO sobre la entidad existente.
            _mapper.Map(productoUpdateDto, productoExistente);

            await _productoService.UpdateProductoAsync(productoExistente);

            // Devuelve una respuesta 204 No Content, que es el estándar para una actualización exitosa.
            return NoContent();
        }

        // DELETE: /api/Productos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var result = await _productoService.DeleteProductoAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
