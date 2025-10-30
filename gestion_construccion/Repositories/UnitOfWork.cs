using gestion_construccion.Datos;
using gestion_construccion.Models;

namespace gestion_construccion.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepository<Producto> Productos { get; private set; }
        public IRepository<Cliente> Clientes { get; private set; }
        public IRepository<Venta> Ventas { get; private set; }
        public IRepository<Usuario> Usuarios { get; private set; } // <-- AÑADIDO
        public IRepository<DetalleVenta> DetallesVenta { get; private set; } // <-- AÑADIDO

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            
            // Inicializar los repositorios con una instancia del repositorio genérico
            Productos = new Repository<Producto>(_context);
            Clientes = new Repository<Cliente>(_context);
            Ventas = new Repository<Venta>(_context);
            Usuarios = new Repository<Usuario>(_context); // <-- AÑADIDO
            DetallesVenta = new Repository<DetalleVenta>(_context); // <-- AÑADIDO
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
