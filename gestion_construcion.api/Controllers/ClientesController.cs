using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.Interfaces;
using Firmeza.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Añadido
using System; // Añadido para ApplicationException
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientesController> _logger; // Añadido

        public ClientesController(IClienteService clienteService, IMapper mapper, ILogger<ClientesController> logger) // Añadido logger
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _logger = logger; // Asignado
        }

        // GET: /api/Clientes
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAllClientes()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            var clientesDto = _mapper.Map<IEnumerable<ClienteDto>>(clientes);
            return Ok(clientesDto);
        }

        // GET: /api/Clientes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ClienteDto>> GetClienteById(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            var clienteDto = _mapper.Map<ClienteDto>(cliente);
            return Ok(clienteDto);
        }

        // POST: /api/Clientes
        [HttpPost]
        [AllowAnonymous] // Permitir que cualquiera se registre
        public async Task<ActionResult<ClienteDto>> CreateCliente([FromBody] ClienteCreateDto clienteCreateDto)
        {
            try
            {
                var usuario = _mapper.Map<Usuario>(clienteCreateDto);
                var cliente = _mapper.Map<Cliente>(clienteCreateDto);
                cliente.Usuario = usuario;

                var nuevoCliente = await _clienteService.AddClienteAsync(cliente, clienteCreateDto.Password);
                var clienteDto = _mapper.Map<ClienteDto>(nuevoCliente);

                return CreatedAtAction(nameof(GetClienteById), new { id = clienteDto.Id }, clienteDto);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Error de aplicación al crear cliente: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cliente.");
                return StatusCode(500, new { message = "Error interno del servidor al crear el cliente." });
            }
        }

        // PUT: /api/Clientes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteUpdateDto clienteUpdateDto)
        {
            try
            {
                var existingCliente = await _clienteService.GetClienteByIdAsync(id);
                if (existingCliente == null)
                {
                    return NotFound();
                }

                _mapper.Map(clienteUpdateDto, existingCliente.Usuario);
                _mapper.Map(clienteUpdateDto, existingCliente);

                await _clienteService.UpdateClienteAsync(id, existingCliente);

                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Error de aplicación al actualizar cliente: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar cliente.");
                return StatusCode(500, new { message = "Error interno del servidor al actualizar el cliente." });
            }
        }

        // DELETE: /api/Clientes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                var result = await _clienteService.DeleteClienteAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Error de aplicación al eliminar cliente: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar cliente.");
                return StatusCode(500, new { message = "Error interno del servidor al eliminar el cliente." });
            }
        }
    }
}
