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
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = default!;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; }
        // Constructor para crear una nueva Venta. 
        // Se asigna el cliente, la fecha actual y se inicializa el total y los detalles.
        public Venta(int clienteId)
        {
            ClienteId = clienteId;
            Fecha = DateTime.UtcNow;
            Total = 0; 
            Detalles = new HashSet<DetalleVenta>();
        }

        // Constructor vacío requerido por Entity Framework.
        public Venta()
        {
            Detalles = new HashSet<DetalleVenta>();
        }
    }
}
