namespace tpintegrador_psr2026.Api.DTOs;

public class DesarrolladoraCreateDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class DesarrolladoraResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
