using GameLibrary.Domain;

namespace GameLibrary.DAO
{
    public interface IJuegoDAO
    {
        List<Juego> ListarTodos();
        List<Juego> ListarPorNombre(string nombre);
        Juego? BuscarPorId(int id);
        List<Juego> ListarPorCategoria(int categoriaId);
        List<Juego> ListarPorDesarrolladora(int desarrolladoraId);
        Juego Guardar(Juego juego);
        bool Actualizar(Juego juego);
    }
}
