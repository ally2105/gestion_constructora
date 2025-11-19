using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Core.Models
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = default!;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; }

        [StringLength(500)]
        public string? ReciboPdfPath { get; set; }

        public Venta(int clienteId)
        {
            ClienteId = clienteId;
            Fecha = DateTime.UtcNow;
            Total = 0;
            Detalles = new HashSet<DetalleVenta>();
        }

        public Venta()
        {
            Detalles = new HashSet<DetalleVenta>();
        }
    }
}
