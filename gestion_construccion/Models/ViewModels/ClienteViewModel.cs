using System.ComponentModel.DataAnnotations;

namespace gestion_construccion.Models.ViewModels
{
    public class ClienteViewModel
    {
        // Propiedades del Usuario
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Identificacion { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Propiedades del Cliente
        public string? Direccion { get; set; }
    }
}
