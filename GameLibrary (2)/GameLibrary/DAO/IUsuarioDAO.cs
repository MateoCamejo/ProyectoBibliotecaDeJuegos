using GameLibrary.Domain;

namespace GameLibrary.DAO
{
    public interface IUsuarioDAO
    {
        Usuario? BuscarPorId(int id);
        Usuario? BuscarPorEmail(string email);
        Usuario Guardar(Usuario usuario);

        // La biblioteca no se "actualiza" como un todo: cada juego
        // adquirido se agrega como una fila nueva en ItemsBiblioteca.
        void AgregarItemBiblioteca(int usuarioId, ItemBiblioteca item);
    }
}
