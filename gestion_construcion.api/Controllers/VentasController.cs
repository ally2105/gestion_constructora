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

                _logger.LogWarning("----------------------------------------------------------------");
                _logger.LogWarning("DATOS DE ENVÍO DE CORREO:");
                _logger.LogWarning("REMITENTE (Config): {SenderEmail}", _emailService.GetType().Name); // Solo para referencia
                _logger.LogWarning("DESTINATARIO (Cliente): {ClienteEmail}", nuevaVenta.Cliente.Usuario.Email);
                _logger.LogWarning("NOMBRE CLIENTE: {ClienteNombre}", nuevaVenta.Cliente.Usuario.Nombre);
                _logger.LogWarning("----------------------------------------------------------------");

                _logger.LogInformation("Iniciando envío de correo para Venta ID: {VentaId} al email: {Email}", nuevaVenta.Id, nuevaVenta.Cliente.Usuario.Email);
                // Enviar correo
                var subject = $"Recibo de Compra - Venta #{nuevaVenta.Id}";
                var message = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2563eb;'>¡Gracias por tu compra, {nuevaVenta.Cliente.Usuario.Nombre}!</h2>
                            <p>Tu compra ha sido procesada exitosamente.</p>
                            <div style='background-color: #f3f4f6; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Número de Venta:</strong> #{nuevaVenta.Id}</p>
                                <p style='margin: 5px 0;'><strong>Fecha:</strong> {nuevaVenta.Fecha:dd/MM/yyyy HH:mm}</p>
                                <p style='margin: 5px 0;'><strong>Total:</strong> {nuevaVenta.Total:C}</p>
                            </div>
                            <p>Adjunto encontrarás el recibo de tu compra en formato PDF.</p>
                            <p style='color: #6b7280; font-size: 14px; margin-top: 30px;'>
                                Si tienes alguna pregunta, no dudes en contactarnos.
                            </p>
                            <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 20px 0;'>
                            <p style='color: #9ca3af; font-size: 12px;'>
                                Este es un correo automático, por favor no responder.
                            </p>
                        </div>
                    </body>
                    </html>";
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
