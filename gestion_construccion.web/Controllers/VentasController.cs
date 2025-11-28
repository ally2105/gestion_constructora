using Firmeza.Core.DTOs;
using Firmeza.Core.Interfaces;
using gestion_construccion.web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using System;
using System.IO;
using System.Threading.Tasks;

namespace gestion_construccion.web.Controllers
{
    /// <summary>
    /// Controller for sale management.
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class VentasController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IClienteService _clienteService;
        private readonly IProductoService _productoService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public VentasController(IVentaService ventaService, IClienteService clienteService, IProductoService productoService, IWebHostEnvironment webHostEnvironment, IPdfService pdfService, IEmailService emailService)
        {
            _ventaService = ventaService;
            _clienteService = clienteService;
            _productoService = productoService;
            _webHostEnvironment = webHostEnvironment;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var ventas = await _ventaService.GetAllVentasAsync();
            return View(ventas);
        }

        // GET: Ventas/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new VentaViewModel());
        }

        // POST: Ventas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VentaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ventaCreateDto = new VentaCreateDto
                    {
                        ClienteId = model.ClienteId,
                        FechaVenta = model.FechaVenta,
                        ProductoId = model.ProductoId,
                        Cantidad = model.Cantidad
                    };
                    var venta = await _ventaService.CrearVentaAsync(ventaCreateDto);

                    // Generar PDF del recibo
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string pdfAbsolutePath = await _pdfService.GenerarReciboVentaAsync(venta, wwwRootPath);
                    
                    // Guardar ruta relativa en la base de datos
                    string relativePath = Path.GetRelativePath(wwwRootPath, pdfAbsolutePath);
                    relativePath = "/" + relativePath.Replace("\\", "/");
                    
                    venta.ReciboPdfPath = relativePath;
                    await _ventaService.UpdateVentaAsync(venta);

                    // Enviar email al cliente con el PDF adjunto
                    try
                    {
                        string? clienteEmail = venta.Cliente?.Usuario?.Email;
                        string clienteNombre = venta.Cliente?.Usuario?.Nombre ?? "Cliente";
                        
                        if (!string.IsNullOrEmpty(clienteEmail))
                        {
                            string subject = $"Recibo de Compra - Venta #{venta.Id}";
                            string message = $@"
                                <html>
                                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                        <h2 style='color: #2563eb;'>¡Gracias por tu compra, {clienteNombre}!</h2>
                                        <p>Tu compra ha sido procesada exitosamente.</p>
                                        <div style='background-color: #f3f4f6; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                            <p style='margin: 5px 0;'><strong>Número de Venta:</strong> #{venta.Id}</p>
                                            <p style='margin: 5px 0;'><strong>Fecha:</strong> {venta.Fecha:dd/MM/yyyy HH:mm}</p>
                                            <p style='margin: 5px 0;'><strong>Total:</strong> {venta.Total:C}</p>
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

                            await _emailService.SendEmailAsync(clienteEmail, subject, message, pdfAbsolutePath);
                        }
                    }
                    catch (Exception emailEx)
                    {
                        // Log el error pero no fallar la venta
                        Console.WriteLine($"Error al enviar email: {emailEx.Message}");
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear la venta.");
                }
            }
            
            await PopulateDropdowns();
            return View(model);
        }

        // GET: Ventas/Receipt/5
        public async Task<IActionResult> Receipt(int? id)
        {
            if (id == null) return NotFound();
            
            var venta = await _ventaService.GetVentaByIdAsync(id.Value);

            if (venta == null) return NotFound();

            return View(venta);
        }

        // GET: Ventas/DownloadReceipt/5
        public async Task<IActionResult> DownloadReceipt(int id)
        {
            var venta = await _ventaService.GetVentaByIdAsync(id);
            if (venta == null)
            {
                return NotFound();
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = "";

            if (!string.IsNullOrEmpty(venta.ReciboPdfPath))
            {
                filePath = Path.Combine(wwwRootPath, venta.ReciboPdfPath.TrimStart('/'));
            }

            if (string.IsNullOrEmpty(venta.ReciboPdfPath) || !System.IO.File.Exists(filePath))
            {
                string newPdfPath = await _pdfService.GenerarReciboVentaAsync(venta, wwwRootPath);
                
                string relativePath = "/" + Path.GetRelativePath(wwwRootPath, newPdfPath).Replace("\\", "/");
                
                venta.ReciboPdfPath = relativePath;
                await _ventaService.UpdateVentaAsync(venta);
                
                filePath = newPdfPath;
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            string fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }

        private async Task PopulateDropdowns()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            ViewBag.ClienteId = new SelectList(clientes, "Id", "Usuario.Nombre");

            var productos = await _productoService.GetProductosConStockAsync();
            ViewBag.ProductoId = new SelectList(productos, "Id", "Nombre");
        }
    }
}
