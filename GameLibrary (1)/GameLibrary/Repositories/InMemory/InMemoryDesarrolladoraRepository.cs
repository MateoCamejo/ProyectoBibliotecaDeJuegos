using GameLibrary.Domain;
using GameLibrary.Repositories.Interfaces;

namespace GameLibrary.Repositories.InMemory
{
    public class InMemoryDesarrolladoraRepository : IDesarrolladoraRepository
    {
        private readonly List<Desarrolladora> _desarrolladoras = new();
        private readonly object _lock = new();
        private int _nextId = 1;

        public IEnumerable<Desarrolladora> GetAll()
        {
            lock (_lock) return _desarrolladoras.ToList();
        }

        public Desarrolladora? GetById(int id)
        {
            lock (_lock) return _desarrolladoras.FirstOrDefault(d => d.Id == id);
        }

        public Desarrolladora Add(Desarrolladora entity)
        {
            lock (_lock)
            {
                entity.Id = _nextId++;
                _desarrolladoras.Add(entity);
                return entity;
            }
        }

        public bool Update(Desarrolladora entity)
        {
            lock (_lock)
            {
                var index = _desarrolladoras.FindIndex(d => d.Id == entity.Id);
                if (index == -1) return false;
                _desarrolladoras[index] = entity;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock) return _desarrolladoras.RemoveAll(d => d.Id == id) > 0;
        }

        public Desarrolladora? GetByNombre(string nombre)
        {
            lock (_lock)
                return _desarrolladoras.FirstOrDefault(d =>
                    string.Equals(d.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}
