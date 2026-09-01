namespace GameLibrary.DTOs
{
    public class CampanaCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PorcentajeDescuento { get; set; }

        // Al menos uno de estos tres criterios debe indicarse.
        public int? CategoriaId { get; set; }
        public int? DesarrolladoraId { get; set; }
        public List<int>? JuegosAfectados { get; set; }
    }

    public class CampanaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public int? CategoriaId { get; set; }
        public int? DesarrolladoraId { get; set; }
        public List<int>? JuegosAfectados { get; set; }
        public bool Activa { get; set; }
    }
}
