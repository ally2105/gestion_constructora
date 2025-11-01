using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    // Mapea esta clase a la tabla "Ventas".
    [Table("Ventas")]
    public class Venta
    {
        // Clave primaria de la venta.
        [Key]
        public int Id { get; set; }

        // Clave foránea que apunta al cliente que realizó la venta.
        [Required]
        public int ClienteId { get; set; }

        // Propiedad de navegación al objeto Cliente completo.
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = default!;

        // Fecha de la venta.
        [Required]
        public DateTime Fecha { get; set; }

        // Monto total de la venta.
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        // Propiedad de navegación a una colección de detalles de la venta.
        // Representa la relación "uno a muchos": una venta tiene muchos detalles.
        public ICollection<DetalleVenta> Detalles { get; set; }

        // Constructor para crear una nueva Venta.
        public Venta(int clienteId)
        {
            ClienteId = clienteId;
            Fecha = DateTime.UtcNow;
            Total = 0; 
            // Se inicializa la colección de detalles para evitar errores de referencia nula.
            Detalles = new HashSet<DetalleVenta>();
        }

        // Constructor vacío requerido por Entity Framework.
        public Venta()
        {
            Detalles = new HashSet<DetalleVenta>();
        }
    }
}
