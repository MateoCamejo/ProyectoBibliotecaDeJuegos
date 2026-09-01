using GameLibrary.Domain;
using GameLibrary.Repositories.Interfaces;

namespace GameLibrary.Repositories.InMemory
{
    public class InMemoryCampanaRepository : ICampanaRepository
    {
        private readonly List<Campana> _campanas = new();
        private readonly object _lock = new();
        private int _nextId = 1;

        public IEnumerable<Campana> GetAll()
        {
            lock (_lock) return _campanas.ToList();
        }

        public Campana? GetById(int id)
        {
            lock (_lock) return _campanas.FirstOrDefault(c => c.Id == id);
        }

        public Campana Add(Campana entity)
        {
            lock (_lock)
            {
                entity.Id = _nextId++;
                _campanas.Add(entity);
                return entity;
            }
        }

        public bool Update(Campana entity)
        {
            lock (_lock)
            {
                var index = _campanas.FindIndex(c => c.Id == entity.Id);
                if (index == -1) return false;
                _campanas[index] = entity;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock) return _campanas.RemoveAll(c => c.Id == id) > 0;
        }

        public IEnumerable<Campana> GetActivas(DateTime fecha)
        {
            lock (_lock)
                return _campanas.Where(c => c.EstaActiva(fecha)).ToList();
        }
    }
}
