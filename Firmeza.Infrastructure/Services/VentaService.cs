using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
using Firmeza.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Firmeza.Infrastructure.Services
{
    public class VentaService : IVentaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VentaService> _logger;

        public VentaService(IUnitOfWork unitOfWork, ILogger<VentaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Venta> CrearVentaAsync(VentaCreateDto model)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(model.ClienteId);
            if (cliente == null) throw new ApplicationException("El cliente seleccionado no existe.");

            var producto = await _unitOfWork.Productos.GetByIdAsync(model.ProductoId);
            if (producto == null) throw new ApplicationException("El producto seleccionado no existe.");

            if (producto.Stock < model.Cantidad)
            {
                throw new ApplicationException($"No hay suficiente stock para '{producto.Nombre}'. Stock: {producto.Stock}.");
            }

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

            producto.Stock -= model.Cantidad;
            _unitOfWork.Productos.Update(producto);

            await _unitOfWork.Ventas.AddAsync(venta);
            await _unitOfWork.DetallesVenta.AddAsync(detalleVenta);

            await _unitOfWork.CompleteAsync();

            // Cargar las propiedades de navegación necesarias para el controlador
            // Asegurarse de que cliente.Usuario no sea null antes de intentar cargarlo
            if (cliente.UsuarioId != 0) // Asumiendo que 0 es un ID no válido
            {
                cliente.Usuario = await _unitOfWork.Usuarios.GetQuery().FirstOrDefaultAsync(u => u.Id == cliente.UsuarioId);
            }
            
            venta.Cliente = cliente;
            venta.Detalles.Add(detalleVenta);
            detalleVenta.Producto = producto;

            return venta;
        }
    }
}
