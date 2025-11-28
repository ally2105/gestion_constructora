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
                _logger.LogInformation("Sale created successfully. ID: {VentaId}", nuevaVenta.Id);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Application error creating sale: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating sale.");
                return StatusCode(500, new { message = "Internal server error creating sale." });
            }

            try
            {
                _logger.LogInformation("Starting PDF generation for Sale ID: {VentaId}", nuevaVenta.Id);
                // Generate PDF
                var pdfPath = await _pdfService.GenerarReciboVentaAsync(nuevaVenta, _webHostEnvironment.ContentRootPath);
                _logger.LogInformation("PDF generated successfully. Path: {PdfPath}", pdfPath);

                _logger.LogWarning("----------------------------------------------------------------");
                _logger.LogWarning("EMAIL SENDING DATA:");
                _logger.LogWarning("SENDER (Config): {SenderEmail}", _emailService.GetType().Name); // Reference only
                _logger.LogWarning("RECIPIENT (Client): {ClienteEmail}", nuevaVenta.Cliente.Usuario.Email);
                _logger.LogWarning("CLIENT NAME: {ClienteNombre}", nuevaVenta.Cliente.Usuario.Nombre);
                _logger.LogWarning("----------------------------------------------------------------");

                _logger.LogInformation("Starting email sending for Sale ID: {VentaId} to email: {Email}", nuevaVenta.Id, nuevaVenta.Cliente.Usuario.Email);
                // Send email
                var subject = $"Purchase Receipt - Sale #{nuevaVenta.Id}";
                var message = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2563eb;'>Thank you for your purchase, {nuevaVenta.Cliente.Usuario.Nombre}!</h2>
                            <p>Your purchase has been processed successfully.</p>
                            <div style='background-color: #f3f4f6; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Sale Number:</strong> #{nuevaVenta.Id}</p>
                                <p style='margin: 5px 0;'><strong>Date:</strong> {nuevaVenta.Fecha:dd/MM/yyyy HH:mm}</p>
                                <p style='margin: 5px 0;'><strong>Total:</strong> {nuevaVenta.Total:C}</p>
                            </div>
                            <p>Attached you will find your purchase receipt in PDF format.</p>
                            <p style='color: #6b7280; font-size: 14px; margin-top: 30px;'>
                                If you have any questions, please do not hesitate to contact us.
                            </p>
                            <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 20px 0;'>
                            <p style='color: #9ca3af; font-size: 12px;'>
                                This is an automated email, please do not reply.
                            </p>
                        </div>
                    </body>
                    </html>";
                await _emailService.SendEmailAsync(nuevaVenta.Cliente.Usuario.Email!, subject, message, pdfPath);
                _logger.LogInformation("Purchase confirmation email sent successfully for Sale ID: {VentaId}", nuevaVenta.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF or sending email for Sale ID: {VentaId}. The sale was created but the receipt was not sent.", nuevaVenta.Id);
                // We don't return a 500 error here, as the sale was created successfully.
                // We could return a 200 with a warning message.
                // For now, we just log the error.
            }

            var ventaDto = _mapper.Map<VentaDto>(nuevaVenta);
            return CreatedAtAction(nameof(GetVentaById), new { id = ventaDto.Id }, ventaDto);
        }
    }
}
