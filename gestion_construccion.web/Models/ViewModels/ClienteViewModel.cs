using System.ComponentModel.DataAnnotations;
using Firmeza.Core.Validation;

namespace gestion_construccion.web.Models.ViewModels
{
    public class ClienteViewModel
    {
        public int Id { get; set; } // ID del cliente, necesario para la edición

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Identificacion { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [MinimumAge(18)]
        public DateTime FechaNacimiento { get; set; }

        // La contraseña solo es requerida para la creación, no para la edición.
        // Para manejar esto, podemos hacerla opcional y validarla en el controlador si es necesario.
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Direccion { get; set; }

        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
