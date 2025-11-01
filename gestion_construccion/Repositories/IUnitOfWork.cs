using gestion_construccion.Models;

namespace gestion_construccion.Repositories
{
    // La interfaz IUnitOfWork define un "contrato" para la Unidad de Trabajo.
    // Hereda de IDisposable para asegurar que la conexión a la base de datos se cierre correctamente.
    public interface IUnitOfWork : IDisposable
    {
        // Propiedad para acceder al repositorio de Productos.
        IRepository<Producto> Productos { get; }
        
        // Propiedad para acceder al repositorio de Clientes.
        IRepository<Cliente> Clientes { get; }
        
        // Propiedad para acceder al repositorio de Ventas.
        IRepository<Venta> Ventas { get; }

        // Propiedad para acceder al repositorio de Usuarios.
        IRepository<Usuario> Usuarios { get; }

        // Propiedad para acceder al repositorio de Detalles de Venta.
        IRepository<DetalleVenta> DetallesVenta { get; }

        // Método asíncrono que guardará todos los cambios realizados en esta unidad de trabajo en la base de datos.
        // Devuelve el número de filas afectadas.
        Task<int> CompleteAsync();
    }
}
