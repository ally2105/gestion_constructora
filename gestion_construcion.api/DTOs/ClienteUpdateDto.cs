using System.ComponentModel.DataAnnotations;

namespace Firmeza.Api.DTOs
{
    public class ClienteUpdateDto
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

        public string? Direccion { get; set; }

        [Phone]
        public string? Telefono { get; set; }
    }
}
