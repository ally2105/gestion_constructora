using System.ComponentModel.DataAnnotations;

namespace gestion_construccion.Models.ViewModels
{
    public class DetalleVentaViewModel
    {
        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        // El precio se obtendrá del producto seleccionado, no se envía desde el formulario.
        public decimal PrecioUnitario { get; set; }
        public string? NombreProducto { get; set; }
    }
}
