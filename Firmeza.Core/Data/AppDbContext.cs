using Firmeza.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Core.Data
{
    public class AppDbContext : IdentityDbContext<Usuario, Rol, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
