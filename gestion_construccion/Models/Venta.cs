using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PersonaId { get; set; }

        [ForeignKey("PersonaId")]
        public Cliente Cliente { get; set; } = default!;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; }

        public Venta()
        {
            Detalles = new HashSet<DetalleVenta>();
        }
    }
}
