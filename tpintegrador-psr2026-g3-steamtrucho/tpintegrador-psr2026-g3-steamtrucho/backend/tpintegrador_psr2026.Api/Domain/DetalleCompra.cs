namespace tpintegrador_psr2026.Api.Domain;

// Conserva el precio histórico pagado, aunque después cambie el precio
// del Juego o termine la campaña que dio el descuento.
public class DetalleCompra
{
    public int Id { get; set; }
    public int JuegoId { get; set; }
    public decimal PrecioFinal { get; set; }
}
