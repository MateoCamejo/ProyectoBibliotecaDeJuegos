using GameLibrary.Domain;

namespace GameLibrary.Repositories.Interfaces
{
    public interface ICompraRepository : IRepository<Compra>
    {
        IEnumerable<Compra> GetByUsuarioId(int usuarioId);
    }
}
