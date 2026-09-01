namespace GameLibrary.Domain
{
    // Clase de asociación entre Usuario/Biblioteca y Juego: guarda el uso
    // particular que ese usuario le da al juego sin tocar el Juego "maestro".
    public class ItemBiblioteca
    {
        public int Id { get; set; }
        public int JuegoId { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public double HorasJugadas { get; set; }
        public DateTime? UltimaVezUsado { get; set; }
    }
}
