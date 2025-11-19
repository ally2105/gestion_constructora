using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;
using Firmeza.Core.Models.ViewModels;

namespace Firmeza.Infrastructure.Services
{
    public class ImportService : IImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Usuario> _userManager;

        public ImportService(IUnitOfWork unitOfWork, UserManager<Usuario> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ImportResultViewModel> ImportarDatosDesdeExcelAsync(Stream stream)
        {
            var result = new ImportResultViewModel();
            var clientesCache = new Dictionary<string, Cliente>();
            var productosCache = new Dictionary<string, Producto>();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Errores.Add("El archivo Excel está vacío o no tiene hojas de cálculo.");
                    return result;
                }

                var rowCount = worksheet.Dimension.Rows;
                result.FilasProcesadas = rowCount - 1;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var clienteEmail = worksheet.Cells[row, 1].GetValue<string>()?.Trim();
                        var clienteNombre = worksheet.Cells[row, 2].GetValue<string>()?.Trim() ?? "SinNombre";
                        var productoNombre = worksheet.Cells[row, 3].GetValue<string>()?.Trim();
                        var cantidad = worksheet.Cells[row, 4].GetValue<int>();
                        var precioUnitario = worksheet.Cells[row, 5].GetValue<decimal>();
                        var fechaVentaRaw = worksheet.Cells[row, 6].GetValue<DateTime>();

                        if (string.IsNullOrEmpty(clienteEmail) || string.IsNullOrEmpty(productoNombre) || cantidad <= 0 || precioUnitario <= 0)
                        {
                            result.Errores.Add($"Fila {row}: Faltan datos obligatorios (Email, Producto, Cantidad o Precio).");
                            continue;
                        }

                        if (!clientesCache.TryGetValue(clienteEmail, out var cliente))
                        {
                            var user = await _userManager.FindByEmailAsync(clienteEmail);
                            if (user == null)
                            {
                                user = new Usuario { UserName = clienteEmail, Email = clienteEmail, Nombre = clienteNombre };
                                var identityResult = await _userManager.CreateAsync(user, "PasswordPorDefecto123!");
                                if (!identityResult.Succeeded)
                                    throw new Exception($"Error al crear usuario {clienteEmail}: {string.Join(",", identityResult.Errors.Select(e => e.Description))}");
                                await _userManager.AddToRoleAsync(user, "Cliente");
                            }

                            cliente = (await _unitOfWork.Clientes.FindAsync(c => c.UsuarioId == user.Id)).FirstOrDefault();
                            if (cliente == null)
                            {
                                cliente = new Cliente(user.Id, null);
                                await _unitOfWork.Clientes.AddAsync(cliente);
                                await _unitOfWork.CompleteAsync();
                                result.ClientesNuevos++;
                            }

                            clientesCache[clienteEmail] = cliente;
                        }

                        if (!productosCache.TryGetValue(productoNombre, out var producto))
                        {
                            producto = (await _unitOfWork.Productos.FindAsync(p => p.Nombre == productoNombre)).FirstOrDefault();
                            if (producto == null)
                            {
                                producto = new Producto
                                {
                                    Nombre = productoNombre,
                                    Precio = precioUnitario,
                                    Stock = 1000
                                };
                                await _unitOfWork.Productos.AddAsync(producto);
                                await _unitOfWork.CompleteAsync();
                                result.ProductosNuevos++;
                            }
                            productosCache[productoNombre] = producto;
                        }

                        var fechaVenta = fechaVentaRaw.Kind == DateTimeKind.Utc
                            ? fechaVentaRaw
                            : DateTime.SpecifyKind(fechaVentaRaw, DateTimeKind.Utc);

                        var venta = new Venta(cliente.Id)
                        {
                            Fecha = fechaVenta,
                            Total = cantidad * precioUnitario
                        };
                        await _unitOfWork.Ventas.AddAsync(venta);

                        var detalleVenta = new DetalleVenta
                        {
                            Venta = venta,
                            Producto = producto,
                            Cantidad = cantidad,
                            PrecioUnitario = precioUnitario
                        };

                        result.VentasCreadas++;
                    }
                    catch (Exception ex)
                    {
                        result.Errores.Add($"Fila {row}: {ex.Message}");
                    }
                }

                try
                {
                    await _unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    result.Errores.Add($"Error final guardando ventas: {inner}");
                }
            }

            return result;
        }
    }
}
