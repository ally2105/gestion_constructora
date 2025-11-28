using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Core.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = default!;

        [StringLength(250)]
        public string? Direccion { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        public Cliente(int usuarioId, string? direccion)
        {
            UsuarioId = usuarioId;
            Direccion = direccion;
            FechaRegistro = DateTime.UtcNow;
        }

        public Cliente() { }
    }
}
