using GameLibrary.Domain;

namespace GameLibrary.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario? GetByEmail(string email);
        Biblioteca? GetBiblioteca(int usuarioId);
    }
}
