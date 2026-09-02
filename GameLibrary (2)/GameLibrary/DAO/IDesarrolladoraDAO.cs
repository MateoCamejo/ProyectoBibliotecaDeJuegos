using GameLibrary.Domain;

namespace GameLibrary.DAO
{
    public interface IDesarrolladoraDAO
    {
        List<Desarrolladora> ListarTodos();
        Desarrolladora? BuscarPorId(int id);
        Desarrolladora Guardar(Desarrolladora desarrolladora);
    }
}
