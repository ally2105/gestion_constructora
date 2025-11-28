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
            mockDbSet.As<IQueryable<Producto>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Producto>(testProducts.AsQueryable().Provider));
            mockDbSet.As<IQueryable<Producto>>().Setup(m => m.Expression).Returns(testProducts.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Producto>>().Setup(m => m.ElementType).Returns(testProducts.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Producto>>().Setup(m => m.GetEnumerator()).Returns(() => testProducts.GetEnumerator());
            mockDbSet.As<IAsyncEnumerable<Producto>>().Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<Producto>(this.GetTestProducts().GetEnumerator()));

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
    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        public T Current => _enumerator.Current;
        public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());
        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);
    }

    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
        public object Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var result = Execute(expression);
            // Manejar la conversión de forma segura
            if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
            {
                var innerResultType = typeof(TResult).GetGenericArguments()[0];
                var taskFromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))?.MakeGenericMethod(innerResultType);
                return (TResult)(taskFromResultMethod?.Invoke(null, new[] { result }) ?? throw new InvalidOperationException("Failed to create Task.FromResult"));
            }
            return (TResult)result;
        }
    }
}
