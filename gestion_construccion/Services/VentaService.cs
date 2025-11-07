using gestion_construccion.Models;
using gestion_construccion.Models.ViewModels;
using gestion_construccion.Repositories;
using System;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    public class VentaService : IVentaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VentaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Venta> CrearVentaAsync(VentaViewModel model)
        {
            // 1. Validar que el cliente y el producto existen.
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(model.ClienteId);
            if (cliente == null)
            {
                throw new ApplicationException("El cliente seleccionado no existe.");
            }

            var producto = await _unitOfWork.Productos.GetByIdAsync(model.ProductoId);
            if (producto == null)
            {
                throw new ApplicationException("El producto seleccionado no existe.");
            }

            // 2. Validar que hay suficiente stock.
            if (producto.Stock < model.Cantidad)
            {
                throw new ApplicationException($"No hay suficiente stock para el producto '{producto.Nombre}'. Stock disponible: {producto.Stock}.");
            }

            // 3. Crear la entidad Venta.
            var venta = new Venta(model.ClienteId)
            {
                Fecha = DateTime.SpecifyKind(model.FechaVenta, DateTimeKind.Utc),
                Total = producto.Precio * model.Cantidad
            };

            // 4. Crear el DetalleVenta.
            var detalleVenta = new DetalleVenta
            {
                Venta = venta,
                ProductoId = producto.Id,
                Cantidad = model.Cantidad,
                PrecioUnitario = producto.Precio
            };

            // 5. Actualizar el stock del producto.
            producto.Stock -= model.Cantidad;
            _unitOfWork.Productos.Update(producto);

            // 6. Añadir la venta y el detalle al UnitOfWork.
            await _unitOfWork.Ventas.AddAsync(venta);
            await _unitOfWork.DetallesVenta.AddAsync(detalleVenta);

            // 7. Guardar todos los cambios en una sola transacción.
            await _unitOfWork.CompleteAsync();

            return venta;
        }
    }
}
