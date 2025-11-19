using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Firmeza.Core.Models
{
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
