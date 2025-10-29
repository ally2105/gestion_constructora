using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    [Table("Clientes")]
    public class Cliente : Persona
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(150)]
        public string Email { get; set; } = default!;

        [Phone(ErrorMessage = "El formato del número de teléfono no es válido")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(250)]
        public string? Direccion { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
