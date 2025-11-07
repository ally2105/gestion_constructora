using gestion_construccion.Datos;
using gestion_construccion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace gestion_construccion.Services
{
    // ViewModel interno para almacenar los datos leídos del Excel.
    internal class ExcelRowData
    {
        public int Row { get; set; }
        public string? ClienteEmail { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime FechaVenta { get; set; }
    }

    public class ImportService : IImportService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IServiceProvider _serviceProvider;

        public ImportService(IDbContextFactory<AppDbContext> contextFactory, IServiceProvider serviceProvider)
        {
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ImportResultViewModel> ImportarDatosDesdeExcelAsync(Stream stream)
        {
            var result = new ImportResultViewModel();
            var excelData = new List<ExcelRowData>();

            // --- FASE 1: Leer todo el Excel y cargarlo en memoria ---
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) { result.Errores.Add("Hoja de Excel no encontrada."); return result; }

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var email = worksheet.Cells[row, 1].GetValue<string>()?.Trim();
                        if (string.IsNullOrEmpty(email)) continue; // Ignorar filas sin email

                        excelData.Add(new ExcelRowData
                        {
                            Row = row,
                            ClienteEmail = email,
                            ClienteNombre = worksheet.Cells[row, 2].GetValue<string>()?.Trim(),
                            ProductoNombre = worksheet.Cells[row, 3].GetValue<string>()?.Trim(),
                            Cantidad = worksheet.Cells[row, 4].GetValue<int>(),
                            PrecioUnitario = worksheet.Cells[row, 5].GetValue<decimal>(),
                            FechaVenta = worksheet.Cells[row, 6].GetValue<DateTime>()
                        });
                    }
                }
                result.FilasProcesadas = excelData.Count;
            }
            catch (Exception ex)
            {
                result.Errores.Add($"Error al leer el archivo Excel: {ex.Message}");
                return result;
            }

            // --- FASE 2: "Get or Create" de Clientes y Productos en Lote ---
            var clienteEmails = excelData.Select(d => d.ClienteEmail!).Distinct().ToList();
            var productoNombres = excelData.Select(d => d.ProductoNombre!).Distinct().ToList();

            var clientesDict = await GetOrCreateClientesEnLoteAsync(clienteEmails, excelData, result);
            var productosDict = await GetOrCreateProductosEnLoteAsync(productoNombres, excelData, result);

            // --- FASE 3: Insertar Ventas en una única transacción ---
            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                // Deshabilitar el seguimiento de cambios para este contexto de inserción de ventas.
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                foreach (var rowData in excelData)
                {
                    // Validar que el cliente y producto para esta fila existen en nuestros diccionarios.
                    if (!clientesDict.TryGetValue(rowData.ClienteEmail!, out var clienteId) || 
                        !productosDict.TryGetValue(rowData.ProductoNombre!, out var productoId))
                    {
                        result.Errores.Add($"Fila {rowData.Row}: No se pudo procesar la venta porque el cliente o el producto no se pudieron crear o encontrar.");
                        continue;
                    }

                    var venta = new Venta(clienteId) 
                    { 
                        Fecha = DateTime.SpecifyKind(rowData.FechaVenta, DateTimeKind.Utc), 
                        Total = rowData.Cantidad * rowData.PrecioUnitario 
                    };

                    var detalleVenta = new DetalleVenta 
                    { 
                        Venta = venta, 
                        ProductoId = productoId, 
                        Cantidad = rowData.Cantidad, 
                        PrecioUnitario = rowData.PrecioUnitario 
                    };

                    context.Ventas.Add(venta);
                    context.DetallesVenta.Add(detalleVenta);
                    result.VentasCreadas++;
                }

                try
                {
                    if (result.VentasCreadas > 0) {
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    result.Errores.Add($"Error final al guardar las ventas: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            return result;
        }

        private async Task<Dictionary<string, int>> GetOrCreateClientesEnLoteAsync(List<string> emails, List<ExcelRowData> excelData, ImportResultViewModel result)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            // Deshabilitar el seguimiento de cambios para este contexto.
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var emailsNormalizados = emails.Select(e => e.ToUpper()).ToList();

            var usuariosExistentes = await context.Users
                .Where(u => emailsNormalizados.Contains(u.NormalizedEmail!))
                .Select(u => new { u.Id, u.NormalizedEmail })
                .ToListAsync();

            var clientesExistentes = await context.Clientes
                .Where(c => usuariosExistentes.Select(u => u.Id).Contains(c.UsuarioId))
                .ToDictionaryAsync(c => c.UsuarioId, c => c.Id);

            var clientesDict = new Dictionary<string, int>();

            foreach (var user in usuariosExistentes)
            {
                if (clientesExistentes.TryGetValue(user.Id, out var clienteId))
                {
                    clientesDict[user.NormalizedEmail!] = clienteId;
                }
                else
                {
                    // El usuario existe, pero el cliente no. Se crea el cliente.
                    var nuevoCliente = new Cliente(user.Id, null);
                    context.Clientes.Add(nuevoCliente);
                    result.ClientesNuevos++;
                }
            }

            var emailsNuevos = emails.Where(e => !usuariosExistentes.Any(u => u.NormalizedEmail == e.ToUpper())).ToList();
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

            foreach (var email in emailsNuevos)
            {
                var nombre = excelData.First(d => d.ClienteEmail == email).ClienteNombre;
                var newUser = new Usuario { UserName = email, Email = email, Nombre = nombre ?? "" };
                var identityResult = await userManager.CreateAsync(newUser, "PasswordPorDefecto123!");
                if (!identityResult.Succeeded) { result.Errores.Add($"Error al crear usuario {email}"); continue; }
                await userManager.AddToRoleAsync(newUser, "Cliente");

                var nuevoCliente = new Cliente(newUser.Id, null);
                context.Clientes.Add(nuevoCliente);
                result.ClientesNuevos++;
            }

            await context.SaveChangesAsync();

            // Rellenar el diccionario con los clientes recién creados
            var todosLosClientes = await context.Clientes.Include(c => c.Usuario).ToListAsync();
            foreach (var cliente in todosLosClientes)
            {
                if (cliente.Usuario != null && !string.IsNullOrEmpty(cliente.Usuario.NormalizedEmail))
                {
                    clientesDict[cliente.Usuario.NormalizedEmail] = cliente.Id;
                }
            }

            return clientesDict;
        }

        private async Task<Dictionary<string, int>> GetOrCreateProductosEnLoteAsync(List<string> nombres, List<ExcelRowData> excelData, ImportResultViewModel result)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            // Deshabilitar el seguimiento de cambios para este contexto.
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var productosExistentes = await context.Productos.Where(p => nombres.Contains(p.Nombre)).ToListAsync();
            var productosDict = productosExistentes.ToDictionary(p => p.Nombre, p => p.Id);

            var nombresNuevos = nombres.Where(n => !productosExistentes.Any(p => p.Nombre == n)).ToList();
            foreach (var nombre in nombresNuevos)
            {
                var precio = excelData.First(d => d.ProductoNombre == nombre).PrecioUnitario;
                var nuevoProducto = new Producto { Nombre = nombre, Precio = precio, Stock = 1000 };
                context.Productos.Add(nuevoProducto);
                result.ProductosNuevos++;
            }

            await context.SaveChangesAsync();

            // Rellenar el diccionario con los productos recién creados
            var todosLosProductos = await context.Productos.Where(p => nombres.Contains(p.Nombre)).ToListAsync();
            return todosLosProductos.ToDictionary(p => p.Nombre, p => p.Id);
        }
    }
}
