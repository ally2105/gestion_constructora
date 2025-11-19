using Firmeza.Core.Data;
using Firmeza.Core.Models;
using Firmeza.Core.Interfaces;

namespace Firmeza.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IRepository<Producto> Productos { get; private set; }
        public IRepository<Cliente> Clientes { get; private set; }
        public IRepository<Venta> Ventas { get; private set; }
        public IRepository<Usuario> Usuarios { get; private set; }
        public IRepository<DetalleVenta> DetallesVenta { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            
            Productos = new Repository<Producto>(_context);
            Clientes = new Repository<Cliente>(_context);
            Ventas = new Repository<Venta>(_context);
            Usuarios = new Repository<Usuario>(_context);
            DetallesVenta = new Repository<DetalleVenta>(_context);
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
