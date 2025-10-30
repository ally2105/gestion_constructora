using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace gestion_construccion.Models
{
    // Añadimos propiedades personalizadas a nuestro usuario de Identity
    public class Usuario : IdentityUser<int>
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Identificacion { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
    }
}
