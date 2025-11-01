using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    // La anotación [Table] especifica que esta clase se mapeará a una tabla llamada "Clientes" en la base de datos.
    [Table("Clientes")]
    public class Cliente
    {
        // La anotación [Key] marca esta propiedad como la clave primaria de la tabla.
        [Key]
        public int Id { get; set; }

        // --- Vínculo con el sistema de Identity ---

        // Propiedad que almacenará la clave foránea al usuario de Identity.
        [Required] // Es obligatorio que un cliente esté asociado a un usuario.
        public int UsuarioId { get; set; }

        // Propiedad de navegación a la entidad Usuario.
        // La anotación [ForeignKey] le dice a EF Core que UsuarioId es la clave foránea para esta relación.
        // Permite acceder al objeto Usuario completo (ej: miCliente.Usuario.Nombre).
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = default!;

        // --- Propiedades específicas del Cliente ---

        // Dirección del cliente. Es opcional (puede ser nulo).
        [StringLength(250)]
        public string? Direccion { get; set; }

        // Fecha en que el cliente fue registrado.
        [Required]
        [Display(Name = "Fecha de Registro")] // Texto que se mostrará en las etiquetas de los formularios.
        public DateTime FechaRegistro { get; set; }

        // Constructor principal para crear un nuevo Cliente.
        // Recibe el ID del usuario asociado y la dirección.
        public Cliente(int usuarioId, string? direccion)
        {
            UsuarioId = usuarioId;
            Direccion = direccion;
            FechaRegistro = DateTime.UtcNow; // Asigna automáticamente la fecha y hora actual en formato UTC.
        }

        // Constructor vacío. Es requerido por Entity Framework Core para poder crear instancias de la clase al leer desde la base de datos.
        public Cliente() { }
    }
}
