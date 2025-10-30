using gestion_construccion.Models;
using System.Linq.Expressions;

namespace gestion_construccion.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Producto> Productos { get; }
        IRepository<Cliente> Clientes { get; }
        IRepository<Venta> Ventas { get; }
        IRepository<Usuario> Usuarios { get; } // <-- AÑADIDO
        IRepository<DetalleVenta> DetallesVenta { get; } // <-- AÑADIDO

        Task<int> CompleteAsync();
    }
}
