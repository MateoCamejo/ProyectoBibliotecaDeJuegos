using GameLibrary.Domain;
using GameLibrary.Repositories.Interfaces;

namespace GameLibrary.Repositories.InMemory
{
    public class InMemoryCategoriaRepository : ICategoriaRepository
    {
        private readonly List<Categoria> _categorias = new();
        private readonly object _lock = new();
        private int _nextId = 1;

        public IEnumerable<Categoria> GetAll()
        {
            lock (_lock) return _categorias.ToList();
        }

        public Categoria? GetById(int id)
        {
            lock (_lock) return _categorias.FirstOrDefault(c => c.Id == id);
        }

        public Categoria Add(Categoria entity)
        {
            lock (_lock)
            {
                entity.Id = _nextId++;
                _categorias.Add(entity);
                return entity;
            }
        }

        public bool Update(Categoria entity)
        {
            lock (_lock)
            {
                var index = _categorias.FindIndex(c => c.Id == entity.Id);
                if (index == -1) return false;
                _categorias[index] = entity;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock) return _categorias.RemoveAll(c => c.Id == id) > 0;
        }

        public Categoria? GetByNombre(string nombre)
        {
            lock (_lock)
                return _categorias.FirstOrDefault(c =>
                    string.Equals(c.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
        }
    }
}
