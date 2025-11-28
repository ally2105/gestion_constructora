using Firmeza.Core.Models;

namespace Firmeza.Core.Interfaces
{
    /// <summary>
    /// Unit of Work pattern to group repository operations and persist changes atomically.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Producto> Productos { get; }
        IRepository<Cliente> Clientes { get; }
        IRepository<Venta> Ventas { get; }
        IRepository<Usuario> Usuarios { get; }
        IRepository<DetalleVenta> DetallesVenta { get; }

        /// <summary>
        /// Persists pending changes in the unit of work.
        /// </summary>
        /// <returns>Number of affected records.</returns>
        Task<int> CompleteAsync();
    }
}
