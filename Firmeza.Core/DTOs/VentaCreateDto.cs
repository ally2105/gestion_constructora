using System;
using System.ComponentModel.DataAnnotations;

namespace Firmeza.Core.DTOs
{
    public class VentaCreateDto
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int ClienteId { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }
}
