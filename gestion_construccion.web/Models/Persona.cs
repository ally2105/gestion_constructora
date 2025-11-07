using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.web.Models
{
    [Table("Personas")]
    public abstract class Persona
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(50)]
        public string Identificacion { get; set; } = default!;

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
    }
}
