namespace GameLibrary.DTOs
{
    public class JuegoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public int DesarrolladoraId { get; set; }
        public List<int> CategoriaIds { get; set; } = new();

        // Opcional. Si no se envía, el juego se crea como "Proximamente".
        // Valores válidos: Disponible, Proximamente, Retirado.
        public string? Estado { get; set; }
    }

    public class JuegoUpdateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public int DesarrolladoraId { get; set; }
        public List<int> CategoriaIds { get; set; } = new();

        // Requerido en el update. Valores válidos: Disponible, Proximamente, Retirado.
        public string Estado { get; set; } = string.Empty;
    }

    public class JuegoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public int DesarrolladoraId { get; set; }
        public string DesarrolladoraNombre { get; set; } = string.Empty;
        public List<CategoriaResponseDto> Categorias { get; set; } = new();
        public string Estado { get; set; } = string.Empty;
    }

    public class PrecioActualResponseDto
    {
        public int JuegoId { get; set; }
        public decimal PrecioOriginal { get; set; }
        public decimal PrecioFinal { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public string? CampanaAplicada { get; set; }
    }
}
