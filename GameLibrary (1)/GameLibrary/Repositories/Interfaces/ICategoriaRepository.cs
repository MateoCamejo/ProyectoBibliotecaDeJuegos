using GameLibrary.Domain;

namespace GameLibrary.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Categoria? GetByNombre(string nombre);
    }
}
