using tpintegrador_psr2026.Api.DAO;
using tpintegrador_psr2026.Api.Domain;

namespace tpintegrador_psr2026.Tests.Fakes;

// Implementaciones mínimas en memoria de los DAO, solo para poder testear
// la lógica de los Services sin necesitar una base de datos real. Los
// métodos que no se usan en los tests actuales lanzan NotImplementedException
// a propósito: si un test nuevo los necesita, se van completando.

public class FakeCampanaDAO : ICampanaDAO
{
    private readonly List<Campana> _campanas;

    public FakeCampanaDAO(List<Campana> campanas)
    {
        _campanas = campanas;
    }

    public List<Campana> ListarActivas(DateTime fecha) =>
        _campanas.Where(c => c.EstaActiva(fecha)).ToList();

    public Campana Guardar(Campana campana) =>
        throw new NotImplementedException("No se usa en estos tests.");
}

public class FakeCategoriaDAO : ICategoriaDAO
{
    public List<Categoria> ListarTodos() => new();

    public Categoria? BuscarPorId(int id) => new Categoria { Id = id, Nombre = "Categoría de prueba" };

    public Categoria Guardar(Categoria categoria) =>
        throw new NotImplementedException("No se usa en estos tests.");
}

public class FakeDesarrolladoraDAO : IDesarrolladoraDAO
{
    public List<Desarrolladora> ListarTodos() => new();

    public Desarrolladora? BuscarPorId(int id) => new Desarrolladora { Id = id, Nombre = "Desarrolladora de prueba" };

    public Desarrolladora Guardar(Desarrolladora desarrolladora) =>
        throw new NotImplementedException("No se usa en estos tests.");
}
