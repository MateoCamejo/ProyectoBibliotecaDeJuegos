namespace GameLibrary.DTOs
{
    public class CompraCreateDto
    {
        public int UsuarioId { get; set; }
        public List<int> JuegoIds { get; set; } = new();
    }

    public class DetalleCompraResponseDto
    {
        public int JuegoId { get; set; }
        public string NombreJuego { get; set; } = string.Empty;
        public decimal PrecioFinal { get; set; }
    }

    public class CompraResponseDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal ImporteFinal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<DetalleCompraResponseDto> Detalles { get; set; } = new();
    }
}
