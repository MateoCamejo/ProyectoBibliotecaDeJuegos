namespace GameLibrary.Repositories.Interfaces
{
    // Contrato genérico de CRUD. Al depender de esto (y no de la
    // implementación concreta) los Services no se enteran si por debajo
    // hay una List<T> en memoria o un DbContext de EF Core.
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        T Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
