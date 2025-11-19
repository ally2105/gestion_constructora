using Firmeza.Core.Models;

namespace Firmeza.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Producto> Productos { get; }
        IRepository<Cliente> Clientes { get; }
        IRepository<Venta> Ventas { get; }
        IRepository<Usuario> Usuarios { get; }
        IRepository<DetalleVenta> DetallesVenta { get; }
        Task<int> CompleteAsync();
    }
}
