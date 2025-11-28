using System.ComponentModel.DataAnnotations;
using Firmeza.Core.Validation;

namespace gestion_construccion.web.Models.ViewModels
{
    public class ClienteViewModel
    {
        public int Id { get; set; } // Client ID, required for editing

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

        // Password is only required for creation, not for editing.
        // To handle this, we make it optional and validate it in the controller if necessary.
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Direccion { get; set; }

        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }
    }
}
