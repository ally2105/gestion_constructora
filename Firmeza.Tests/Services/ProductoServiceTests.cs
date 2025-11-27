namespace Firmeza.Tests.Services
{
    public class ProductoServiceTests
    {
        [Fact]
        public async Task GetAllProductosAsync_ReturnsAllProducts()
        {
            // Arrange
            var testProducts = GetTestProducts();
            var mockProductRepository = new Mock<IRepository<Producto>>();

            // Configurar el mock para simular un IQueryable que soporta operaciones asíncronas
            var mockDbSet = new Mock<DbSet<Producto>>();
            mockDbSet.As<IAsyncEnumerable<Producto>>()
                     .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                     .Returns(new TestAsyncEnumerable<Producto>(testProducts.GetEnumerator())); // Corregido aquí

            mockDbSet.As<IQueryable<Producto>>()
                     .Setup(x => x.Provider)
                     .Returns(new TestAsyncQueryProvider<Producto>(testProducts.AsQueryable().Provider));

            mockDbSet.As<IQueryable<Producto>>().Setup(x => x.Expression).Returns(testProducts.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Producto>>().Setup(x => x.ElementType).Returns(testProducts.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Producto>>().Setup(x => x.GetEnumerator()).Returns(testProducts.GetEnumerator());

            mockProductRepository.Setup(repo => repo.GetQuery()).Returns(mockDbSet.Object);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Productos).Returns(mockProductRepository.Object);

            var service = new ProductoService(mockUnitOfWork.Object);

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
            var mockProductRepository = new Mock<IRepository<Producto>>();
            mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Producto>()))
                                 .Returns(Task.CompletedTask);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Productos).Returns(mockProductRepository.Object);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.FromResult(1));

            var service = new ProductoService(mockUnitOfWork.Object);
            var newProduct = new Producto { Nombre = "Nuevo Producto", Precio = 10.00M, Stock = 5 };

            // Act
            var result = await service.AddProductoAsync(newProduct);

            // Assert
            Assert.NotNull(result);
            mockProductRepository.Verify(repo => repo.AddAsync(newProduct), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
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

    // Clases auxiliares para simular IAsyncEnumerable y IAsyncQueryProvider
    // Necesarias para que los métodos asíncronos de EF Core funcionen con IQueryable en memoria
    public class TestAsyncEnumerable<T> : IAsyncEnumerable<T>, IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerable(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumeratorInner<T>(_enumerator);
        }

        public IEnumerator<T> GetEnumerator() => _enumerator;
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class TestAsyncEnumeratorInner<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumeratorInner(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public T Current => _enumerator.Current;

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_enumerator.MoveNext());
        }
    }

    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return _inner.CreateQuery(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return _inner.CreateQuery<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var enumerationResult = Execute(expression);

            return (TResult)(typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { enumerationResult }) ?? throw new InvalidOperationException("Failed to create Task.FromResult"));
        }
    }
}
