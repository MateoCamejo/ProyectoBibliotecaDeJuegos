namespace GameLibrary.Domain
{
    public class Biblioteca
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public List<ItemBiblioteca> Items { get; set; } = new();

        public bool Contiene(int juegoId) => Items.Any(i => i.JuegoId == juegoId);

        // Un videojuego retirado igual debe seguir en la biblioteca de quien
        // ya lo compró: como acá solo trabajamos con el JuegoId + fecha de
        // adquisición, esa regla se cumple sola (nunca lo borramos de acá).
        public void AgregarJuego(int juegoId)
        {
            if (Contiene(juegoId)) return;

            Items.Add(new ItemBiblioteca
            {
                JuegoId = juegoId,
                FechaAdquisicion = DateTime.UtcNow,
                HorasJugadas = 0,
                UltimaVezUsado = null
            });
        }
    }
}
