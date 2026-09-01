using GameLibrary.Domain;
using GameLibrary.Repositories.Interfaces;

namespace GameLibrary.Repositories.InMemory
{
    // Implementación provisoria mientras no se define la persistencia real.
    // Al respetar IJuegoRepository, el día de mañana se reemplaza por una
    // EfJuegoRepository sin tocar Services ni Controllers.
    public class InMemoryJuegoRepository : IJuegoRepository
    {
        private readonly List<Juego> _juegos = new();
        private readonly object _lock = new();
        private int _nextId = 1;

        public IEnumerable<Juego> GetAll()
        {
            lock (_lock) return _juegos.ToList();
        }

        public Juego? GetById(int id)
        {
            lock (_lock) return _juegos.FirstOrDefault(j => j.Id == id);
        }

        public Juego Add(Juego entity)
        {
            lock (_lock)
            {
                entity.Id = _nextId++;
                _juegos.Add(entity);
                return entity;
            }
        }

        public bool Update(Juego entity)
        {
            lock (_lock)
            {
                var index = _juegos.FindIndex(j => j.Id == entity.Id);
                if (index == -1) return false;
                _juegos[index] = entity;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock) return _juegos.RemoveAll(j => j.Id == id) > 0;
        }

        public IEnumerable<Juego> GetByCategoria(int categoriaId)
        {
            lock (_lock)
                return _juegos.Where(j => j.PerteneceACategoria(categoriaId)).ToList();
        }

        public IEnumerable<Juego> GetByDesarrolladora(int desarrolladoraId)
        {
            lock (_lock)
                return _juegos.Where(j => j.DesarrolladoraId == desarrolladoraId).ToList();
        }
    }
}
