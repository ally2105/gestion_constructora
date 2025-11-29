using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace gestion_construccion.web.Models.ViewModels
{
    // ViewModel to handle the creation of a new sale.
    public class VentaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required]
        [Display(Name = "Fecha de la Venta")]
        [DataType(DataType.Date)]
        public DateTime FechaVenta { get; set; } = DateTime.Today;

        // For this version, we will handle a single product per sale.
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }
}
