using GameLibrary.Domain.Enums;

namespace GameLibrary.Domain
{
    public class Compra
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal ImporteFinal { get; set; }
        public List<DetalleCompra> Detalles { get; set; } = new();
        public EstadoCompra Estado { get; set; } = EstadoCompra.Pendiente;

        public void Confirmar() => Estado = EstadoCompra.Confirmada;
        public void Cancelar() => Estado = EstadoCompra.Cancelada;
    }
}
