using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.DTOs
{
    public class ClienteCreateDto
    {
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

        public string? Direccion { get; set; }

        [Phone]
        public string? Telefono { get; set; }
    }
}
