using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        // --- Vínculo con el sistema de Identity ---
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = default!;

        // --- Propiedades específicas del Cliente ---

        [StringLength(250)]
        public string? Direccion { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        // Constructor para crear un nuevo Cliente a partir de un Usuario
        public Cliente(int usuarioId, string? direccion)
        {
            UsuarioId = usuarioId;
            Direccion = direccion;
            FechaRegistro = DateTime.UtcNow;
        }

        // Constructor vacío para Entity Framework
        public Cliente() { }
    }
}
