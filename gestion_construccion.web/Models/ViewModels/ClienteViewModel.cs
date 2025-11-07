using System.ComponentModel.DataAnnotations;
using gestion_construccion.web.Validation; // <-- AÑADIDO

namespace gestion_construccion.web.Models.ViewModels
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
        [MinimumAge(18)] // <-- ATRIBUTO AÑADIDO
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Propiedades del Cliente
        public string? Direccion { get; set; }

        // Propiedad de Teléfono añadida
        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
