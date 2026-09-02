using GameLibrary.Domain.Enums;

namespace GameLibrary.Domain
{
    public class Juego
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public int DesarrolladoraId { get; set; }
        public List<Categoria> Categorias { get; set; } = new();
        public EstadoJuego Estado { get; set; } = EstadoJuego.Proximamente;

        // Invariante básica del propio Juego. Las reglas que combinan
        // Juego + Usuario + Compra (p. ej. "ya lo posee") viven en los Services.
        public bool PuedeComprarse() => Estado == EstadoJuego.Disponible;

        public bool PerteneceACategoria(int categoriaId) =>
            Categorias.Any(c => c.Id == categoriaId);
    }
}
