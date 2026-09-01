namespace GameLibrary.DTOs
{
    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ItemBibliotecaResponseDto
    {
        public int JuegoId { get; set; }
        public string NombreJuego { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public double HorasJugadas { get; set; }
        public DateTime? UltimaVezUsado { get; set; }
    }

    public class BibliotecaResponseDto
    {
        public int UsuarioId { get; set; }
        public List<ItemBibliotecaResponseDto> Items { get; set; } = new();
    }
}
