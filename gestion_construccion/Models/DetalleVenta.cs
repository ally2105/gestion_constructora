using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    // Mapea esta clase a la tabla "DetallesVenta".
    [Table("DetallesVenta")]
    public class DetalleVenta
    {
        // Clave primaria del detalle.
        [Key]
        public int Id { get; set; }

        // Clave foránea que apunta a la venta principal.
        [Required]
        public int VentaId { get; set; }

        // Propiedad de navegación a la Venta.
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = default!;

        // Clave foránea que apunta al producto vendido.
        [Required]
        public int ProductoId { get; set; }

        // Propiedad de navegación al Producto.
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; } = default!;

        // Cantidad de unidades vendidas de este producto.
        [Required]
        public int Cantidad { get; set; }

        // Precio del producto en el momento de la venta.
        // Se guarda aquí para mantener un registro histórico, ya que el precio en la tabla Productos puede cambiar.
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }
    }
}
