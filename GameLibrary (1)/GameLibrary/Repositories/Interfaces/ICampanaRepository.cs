using GameLibrary.Domain;

namespace GameLibrary.Repositories.Interfaces
{
    public interface ICampanaRepository : IRepository<Campana>
    {
        IEnumerable<Campana> GetActivas(DateTime fecha);
    }
}
