namespace tpintegrador_psr2026.Api.Domain;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Biblioteca Biblioteca { get; set; } = new();
}
