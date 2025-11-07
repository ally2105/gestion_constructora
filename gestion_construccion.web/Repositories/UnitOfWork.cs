using gestion_construccion.web.Datos;
using gestion_construccion.web.Models;

namespace gestion_construccion.web.Repositories
{
    // La clase UnitOfWork es la implementación concreta de la interfaz IUnitOfWork.
    public class UnitOfWork : IUnitOfWork
    {
        // Campo privado para mantener la referencia al contexto de la base de datos.
        private readonly AppDbContext _context;

        // Propiedades públicas para exponer los diferentes repositorios.
        public IRepository<Producto> Productos { get; private set; }
        public IRepository<Cliente> Clientes { get; private set; }
        public IRepository<Venta> Ventas { get; private set; }
        public IRepository<Usuario> Usuarios { get; private set; }
        public IRepository<DetalleVenta> DetallesVenta { get; private set; }

        // El constructor recibe el AppDbContext a través de inyección de dependencias.
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            
            // Se inicializan todas las propiedades de los repositorios, pasándoles el contexto de la base de datos.
            // Se utiliza la implementación genérica Repository<T> para cada uno.
            Productos = new Repository<Producto>(_context);
            Clientes = new Repository<Cliente>(_context);
            Ventas = new Repository<Venta>(_context);
            Usuarios = new Repository<Usuario>(_context);
            DetallesVenta = new Repository<DetalleVenta>(_context);
        }

        // Implementación del método para guardar todos los cambios.
        // Simplemente llama al método SaveChangesAsync() de Entity Framework Core.
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Implementación del método Dispose para liberar recursos.
        // Llama al método Dispose() del contexto de la base de datos para cerrar la conexión.
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
