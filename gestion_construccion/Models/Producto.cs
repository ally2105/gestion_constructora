using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestion_construccion.Models
{
    // Mapea esta clase a la tabla "Productos".
    [Table("Productos")]
    public class Producto
    {
        // Clave primaria.
        [Key]
        // La base de datos generará automáticamente el valor para esta propiedad al insertar un nuevo registro (autoincremental).
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Nombre del producto. Es obligatorio.
        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres")]
        public string Nombre { get; set; } = default!;

        // Descripción opcional del producto.
        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string? Descripcion { get; set; }

        // Precio del producto. Es obligatorio.
        [Required(ErrorMessage = "El precio es obligatorio")]
        // Especifica el tipo de dato exacto en la base de datos (ej: decimal(18,2) en SQL Server).
        [Column(TypeName = "decimal(18, 2)")]
        // Valida que el precio esté en un rango válido (mayor que cero).
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }

        // Cantidad en stock. Es obligatorio.
        [Required(ErrorMessage = "El stock es obligatorio")]
        // Valida que el stock no sea un número negativo.
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
    }
}
