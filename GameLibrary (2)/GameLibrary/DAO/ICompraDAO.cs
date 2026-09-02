using GameLibrary.Domain;
using GameLibrary.Domain.Enums;

namespace GameLibrary.DAO
{
    public interface ICompraDAO
    {
        Compra? BuscarPorId(int id);
        List<Compra> ListarPorUsuario(int usuarioId);
        Compra Guardar(Compra compra);
        bool ActualizarEstado(int id, EstadoCompra estado);
    }
}
