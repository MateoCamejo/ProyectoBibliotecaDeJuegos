namespace GameLibrary.DTOs
{
    public class CategoriaCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class CategoriaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
