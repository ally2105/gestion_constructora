using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace gestion_construccion.Models.ViewModels
{
    // ViewModel para manejar la creación de una nueva venta.
    public class VentaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required]
        [Display(Name = "Fecha de la Venta")]
        [DataType(DataType.Date)]
        public DateTime FechaVenta { get; set; } = DateTime.Today;

        // Para esta versión simplificada, manejaremos un solo producto por venta.
        // En una versión avanzada, esto sería una lista de 'DetalleVentaViewModel'.
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }
}
