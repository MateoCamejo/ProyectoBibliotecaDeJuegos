using GameLibrary.Domain;
using GameLibrary.Repositories.Interfaces;

namespace GameLibrary.Repositories.InMemory
{
    public class InMemoryCompraRepository : ICompraRepository
    {
        private readonly List<Compra> _compras = new();
        private readonly object _lock = new();
        private int _nextId = 1;

        public IEnumerable<Compra> GetAll()
        {
            lock (_lock) return _compras.ToList();
        }

        public Compra? GetById(int id)
        {
            lock (_lock) return _compras.FirstOrDefault(c => c.Id == id);
        }

        public Compra Add(Compra entity)
        {
            lock (_lock)
            {
                entity.Id = _nextId++;
                _compras.Add(entity);
                return entity;
            }
        }

        public bool Update(Compra entity)
        {
            lock (_lock)
            {
                var index = _compras.FindIndex(c => c.Id == entity.Id);
                if (index == -1) return false;
                _compras[index] = entity;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock) return _compras.RemoveAll(c => c.Id == id) > 0;
        }

        public IEnumerable<Compra> GetByUsuarioId(int usuarioId)
        {
            lock (_lock)
                return _compras.Where(c => c.UsuarioId == usuarioId).ToList();
        }
    }
}
