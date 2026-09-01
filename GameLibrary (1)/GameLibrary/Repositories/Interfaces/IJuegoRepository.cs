using GameLibrary.Domain;

namespace GameLibrary.Repositories.Interfaces
{
    public interface IJuegoRepository : IRepository<Juego>
    {
        IEnumerable<Juego> GetByCategoria(int categoriaId);
        IEnumerable<Juego> GetByDesarrolladora(int desarrolladoraId);
    }
}
