using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Core.Models
{
    [Table("DetallesVenta")]
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VentaId { get; set; }

        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = default!;

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; } = default!;

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }
    }
}
