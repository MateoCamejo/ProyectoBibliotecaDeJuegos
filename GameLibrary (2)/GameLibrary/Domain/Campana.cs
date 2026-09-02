namespace GameLibrary.Domain
{
    public class Campana
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PorcentajeDescuento { get; set; }

        // Criterios opcionales de alcance: pueden combinarse entre sí.
        public int? CategoriaId { get; set; }
        public int? DesarrolladoraId { get; set; }
        public List<int>? JuegosAfectados { get; set; }

        public bool EstaActiva(DateTime fecha) =>
            fecha >= FechaInicio && fecha <= FechaFin;

        // Determina si el juego cae dentro de alguno de los criterios de
        // la campaña. La elección de "cuál campaña usar" cuando hay varias
        // aplicables (no acumulables) queda para el Service, no para acá.
        public bool Alcanza(Juego juego)
        {
            if (JuegosAfectados != null && JuegosAfectados.Contains(juego.Id))
                return true;

            if (CategoriaId.HasValue && juego.PerteneceACategoria(CategoriaId.Value))
                return true;

            if (DesarrolladoraId.HasValue && juego.DesarrolladoraId == DesarrolladoraId.Value)
                return true;

            return false;
        }
    }
}
