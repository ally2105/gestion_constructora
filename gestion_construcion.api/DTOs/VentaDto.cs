using System;
using System.Collections.Generic;

namespace Firmeza.Api.DTOs
{
    public class VentaDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
    }
}
