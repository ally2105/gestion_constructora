using Firmeza.Core.Data;
using Firmeza.Core.Models;
using Firmeza.Infrastructure.Services;
using Firmeza.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Firmeza.Tests.Services
{
    public class ProductoServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetAllProductosAsync_ReturnsAllProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Productos.AddRange(GetTestProducts());
            context.SaveChanges();

            var unitOfWork = new UnitOfWork(context);
            var service = new ProductoService(unitOfWork);

            // Act
            var (productos, totalRecords) = await service.GetAllProductosAsync(1, 10);

            // Assert
            Assert.NotNull(productos);
            Assert.Equal(3, productos.Count());
            Assert.Equal(3, totalRecords);
        }

        [Fact]
        public async Task AddProductoAsync_AddsProductSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ProductoService(unitOfWork);
            var newProduct = new Producto { Nombre = "Nuevo Producto", Precio = 10.00M, Stock = 5 };

            // Act
            var result = await service.AddProductoAsync(newProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, context.Productos.Count());
        }

        private List<Producto> GetTestProducts()
        {
            return new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Producto 1", Precio = 10.00M, Stock = 10 },
                new Producto { Id = 2, Nombre = "Producto 2", Precio = 20.00M, Stock = 20 },
                new Producto { Id = 3, Nombre = "Producto 3", Precio = 30.00M, Stock = 30 }
            };
        }
    }
}
