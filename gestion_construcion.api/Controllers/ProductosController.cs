using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
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

        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<ProductoDto>>> GetAllProductos([FromQuery] PagingParameters pagingParameters)
        {
            var (productos, totalRecords) = await _productoService.GetAllProductosAsync(pagingParameters.PageNumber, pagingParameters.PageSize);
            
            var productosDto = _mapper.Map<List<ProductoDto>>(productos);

            var pagedResponse = new PagedResponseDto<ProductoDto>(
                productosDto,
                pagingParameters.PageNumber,
                pagingParameters.PageSize,
                totalRecords
            );

            return Ok(pagedResponse);
        }

        // ... (resto de los métodos)
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProductoById(int id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            var productoDto = _mapper.Map<ProductoDto>(producto);
            return Ok(productoDto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProductoDto>> CreateProducto([FromBody] ProductoCreateDto productoCreateDto)
        {
            var producto = _mapper.Map<Producto>(productoCreateDto);
            var nuevoProducto = await _productoService.AddProductoAsync(producto);
            var productoDto = _mapper.Map<ProductoDto>(nuevoProducto);
            return CreatedAtAction(nameof(GetProductoById), new { id = productoDto.Id }, productoDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateProducto(int id, [FromBody] ProductoCreateDto productoUpdateDto)
        {
            var productoExistente = await _productoService.GetProductoByIdAsync(id);
            if (productoExistente == null)
            {
                return NotFound();
            }
            _mapper.Map(productoUpdateDto, productoExistente);
            await _productoService.UpdateProductoAsync(productoExistente);
            return NoContent();
        }

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
