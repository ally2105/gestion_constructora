using gestion_construccion.web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gestion_construccion.web.Datos
{
    // AppDbContext es el corazón de Entity Framework Core en nuestra aplicación.
    // Hereda de IdentityDbContext, lo que le da toda la funcionalidad para manejar usuarios y roles de Identity,
    // además de nuestras propias entidades.
    // Los tipos genéricos <Usuario, Rol, int, ...> especifican que usaremos nuestras clases personalizadas y claves de tipo entero.
    public class AppDbContext : IdentityDbContext<Usuario, Rol, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        // El constructor recibe las opciones de configuración (como la cadena de conexión) y las pasa a la clase base.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet<T> representa una tabla en la base de datos.
        // Cada DbSet aquí se convertirá en una tabla para el modelo correspondiente.
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        // OnModelCreating es un método que se llama cuando EF Core está construyendo el modelo de la base de datos.
        // Se usa para configuraciones avanzadas.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Es crucial llamar a base.OnModelCreating() para que la configuración de Identity se aplique correctamente.
            base.OnModelCreating(modelBuilder);

            // Aquí personalizamos los nombres de las tablas de Identity.
            // Esto es opcional, pero ayuda a mantener la base de datos organizada.
            modelBuilder.Entity<Usuario>().ToTable("Seguridad_Usuarios");
            modelBuilder.Entity<Rol>().ToTable("Seguridad_Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("Seguridad_UsuariosRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("Seguridad_UsuariosClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("Seguridad_UsuariosLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("Seguridad_RolesClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("Seguridad_UsuariosTokens");
        }
    }
}
