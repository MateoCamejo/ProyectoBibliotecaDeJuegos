using tpintegrador_psr2026.Api.Domain;

namespace tpintegrador_psr2026.Api.DAO;

public interface ICategoriaDAO
{
    List<Categoria> ListarTodos();
    Categoria? BuscarPorId(int id);
    Categoria Guardar(Categoria categoria);
}
