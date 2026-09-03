using tpintegrador_psr2026.Api.Domain;

namespace tpintegrador_psr2026.Api.DAO;

public interface IDesarrolladoraDAO
{
    List<Desarrolladora> ListarTodos();
    Desarrolladora? BuscarPorId(int id);
    Desarrolladora Guardar(Desarrolladora desarrolladora);
}
