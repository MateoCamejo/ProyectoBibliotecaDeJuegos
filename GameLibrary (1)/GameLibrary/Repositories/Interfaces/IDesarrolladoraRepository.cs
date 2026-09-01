using GameLibrary.Domain;

namespace GameLibrary.Repositories.Interfaces
{
    public interface IDesarrolladoraRepository : IRepository<Desarrolladora>
    {
        Desarrolladora? GetByNombre(string nombre);
    }
}
