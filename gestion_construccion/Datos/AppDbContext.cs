using gestion_construccion.Models;
using Microsoft.EntityFrameworkCore;

namespace gestion_construccion.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la herencia TPH (Table-Per-Hierarchy)
            modelBuilder.Entity<Persona>().ToTable("Personas");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
        }
    }
}
