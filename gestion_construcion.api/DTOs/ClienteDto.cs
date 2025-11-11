namespace Firmeza.Api.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
