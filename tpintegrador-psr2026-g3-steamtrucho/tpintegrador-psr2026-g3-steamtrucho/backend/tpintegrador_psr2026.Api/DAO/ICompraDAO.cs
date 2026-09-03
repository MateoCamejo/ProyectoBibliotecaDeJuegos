using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.Domain.Enums;

namespace tpintegrador_psr2026.Api.DAO;

public interface ICompraDAO
{
    Compra? BuscarPorId(int id);
    List<Compra> ListarPorUsuario(int usuarioId);
    Compra Guardar(Compra compra);
    bool ActualizarEstado(int id, EstadoCompra estado);
}
