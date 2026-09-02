using GameLibrary.Domain;

namespace GameLibrary.DAO
{
    public interface ICategoriaDAO
    {
        List<Categoria> ListarTodos();
        Categoria? BuscarPorId(int id);
        Categoria Guardar(Categoria categoria);
    }
}
