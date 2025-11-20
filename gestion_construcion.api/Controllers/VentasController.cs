using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.DTOs;
using Firmeza.Core.Interfaces;
using Firmeza.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Añadido
using System; // Añadido
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmeza.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<VentasController> _logger; // Añadido

        public VentasController(IVentaService ventaService, IUnitOfWork unitOfWork, IMapper mapper, IPdfService pdfService, IEmailService emailService, IWebHostEnvironment webHostEnvironment, ILogger<VentasController> logger) // Añadido logger
        {
            _ventaService = ventaService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pdfService = pdfService;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger; // Asignado
        }

        // GET: /api/Ventas
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetAllVentas()
        {
            var ventas = await _unitOfWork.Ventas.GetAllAsync();
            var ventasDto = _mapper.Map<IEnumerable<VentaDto>>(ventas);
            return Ok(ventasDto);
        }

        // GET: /api/Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetVentaById(int id)
        {
            var venta = await _unitOfWork.Ventas.GetByIdAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            var ventaDto = _mapper.Map<VentaDto>(venta);
            return Ok(ventaDto);
        }

        // POST: /api/Ventas
        [HttpPost]
        public async Task<ActionResult<VentaDto>> CreateVenta([FromBody] VentaCreateDto ventaCreateDto)
        {
            Venta nuevaVenta;
            try
            {
                nuevaVenta = await _ventaService.CrearVentaAsync(ventaCreateDto);
                _logger.LogInformation("Venta creada con éxito. ID: {VentaId}", nuevaVenta.Id);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Error de aplicación al crear venta: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear venta.");
                return StatusCode(500, new { message = "Error interno del servidor al crear la venta." });
            }

            try
            {
                _logger.LogInformation("Iniciando generación de PDF para Venta ID: {VentaId}", nuevaVenta.Id);
                // Generar PDF
                var pdfPath = await _pdfService.GenerarReciboVentaAsync(nuevaVenta, _webHostEnvironment.ContentRootPath);
                _logger.LogInformation("PDF generado con éxito. Ruta: {PdfPath}", pdfPath);

                _logger.LogInformation("Iniciando envío de correo para Venta ID: {VentaId} al email: {Email}", nuevaVenta.Id, nuevaVenta.Cliente.Usuario.Email);
                // Enviar correo
                var subject = $"Confirmación de tu compra - Venta #{nuevaVenta.Id}";
                var message = $"<h1>¡Gracias por tu compra, {nuevaVenta.Cliente.Usuario.Nombre}!</h1><p>Adjuntamos el recibo de tu compra.</p>";
                await _emailService.SendEmailAsync(nuevaVenta.Cliente.Usuario.Email!, subject, message, pdfPath);
                _logger.LogInformation("Correo de confirmación de compra enviado con éxito para Venta ID: {VentaId}", nuevaVenta.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF o enviar correo para Venta ID: {VentaId}. La venta se creó pero el comprobante no se envió.", nuevaVenta.Id);
                // No devolvemos un error 500 aquí, ya que la venta se creó correctamente.
                // Podríamos devolver un 200 con un mensaje de advertencia.
                // Por ahora, solo logueamos el error.
            }

            var ventaDto = _mapper.Map<VentaDto>(nuevaVenta);
            return CreatedAtAction(nameof(GetVentaById), new { id = ventaDto.Id }, ventaDto);
        }
    }
}
