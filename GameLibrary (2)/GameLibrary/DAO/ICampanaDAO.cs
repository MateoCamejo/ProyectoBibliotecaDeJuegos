using GameLibrary.Domain;

namespace GameLibrary.DAO
{
    public interface ICampanaDAO
    {
        List<Campana> ListarActivas(DateTime fecha);
        Campana Guardar(Campana campana);
    }
}
