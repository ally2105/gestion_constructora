using gestion_construccion.web.Models;
using gestion_construccion.web.Models.ViewModels;
using gestion_construccion.web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace gestion_construccion.web.Services
{
    public class VentaService : IVentaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPdfService _pdfService;
        private readonly ILogger<VentaService> _logger;

        public VentaService(IUnitOfWork unitOfWork, IPdfService pdfService, ILogger<VentaService> logger)
        {
            _unitOfWork = unitOfWork;
            _pdfService = pdfService;
            _logger = logger;
        }

        public async Task<Venta> CrearVentaAsync(VentaViewModel model)
        {
            // La transacción se maneja implícitamente por la única llamada a CompleteAsync al final.
            // Si algo falla, no se guardará nada.

            // 1. Validaciones
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(model.ClienteId);
            if (cliente == null) throw new ApplicationException("El cliente seleccionado no existe.");

            var producto = await _unitOfWork.Productos.GetByIdAsync(model.ProductoId);
            if (producto == null) throw new ApplicationException("El producto seleccionado no existe.");

            if (producto.Stock < model.Cantidad)
            {
                throw new ApplicationException($"No hay suficiente stock para '{producto.Nombre}'. Stock: {producto.Stock}.");
            }

            // 2. Crear las entidades
            var venta = new Venta(model.ClienteId)
            {
                Fecha = DateTime.SpecifyKind(model.FechaVenta, DateTimeKind.Utc),
                Total = producto.Precio * model.Cantidad
            };

            var detalleVenta = new DetalleVenta
            {
                Venta = venta,
                ProductoId = producto.Id,
                Cantidad = model.Cantidad,
                PrecioUnitario = producto.Precio
            };

            // 3. Actualizar el stock
            producto.Stock -= model.Cantidad;
            _unitOfWork.Productos.Update(producto);

            // 4. Añadir las nuevas entidades al UnitOfWork
            await _unitOfWork.Ventas.AddAsync(venta);
            await _unitOfWork.DetallesVenta.AddAsync(detalleVenta);

            // 5. Guardar todos los cambios (excepto la ruta del PDF)
            await _unitOfWork.CompleteAsync();

            // --- Generación del PDF ---
            try
            {
                // Cargar las propiedades de navegación necesarias para el PDF.
                venta.Cliente = cliente;
                cliente.Usuario = await _unitOfWork.Usuarios.GetQuery().FirstAsync(u => u.Id == cliente.UsuarioId);
                venta.Detalles.Add(detalleVenta);
                detalleVenta.Producto = producto;

                var pdfPath = await _pdfService.GenerarReciboVentaAsync(venta);
                venta.ReciboPdfPath = pdfPath;

                // Actualizamos la venta con la ruta del PDF y guardamos de nuevo.
                _unitOfWork.Ventas.Update(venta);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                // Si la generación del PDF falla, lo registramos pero no hacemos que la venta falle.
                _logger.LogError(ex, "Ocurrió un error al generar o guardar el PDF para la venta ID {VentaId}.", venta.Id);
                // La venta ya se creó, pero sin el enlace al PDF.
            }

            return venta;
        }
    }
}
