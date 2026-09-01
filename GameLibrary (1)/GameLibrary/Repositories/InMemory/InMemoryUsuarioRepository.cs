using GameLibrary.Domain;
using GameLibrary.Repositories.Interfaces;

namespace GameLibrary.Repositories.InMemory
{
    public class InMemoryUsuarioRepository : IUsuarioRepository
    {
        private readonly List<Usuario> _usuarios = new();
        private readonly object _lock = new();
        private int _nextId = 1;
        private int _nextBibliotecaId = 1;

        public IEnumerable<Usuario> GetAll()
        {
            lock (_lock) return _usuarios.ToList();
        }

        public Usuario? GetById(int id)
        {
            lock (_lock) return _usuarios.FirstOrDefault(u => u.Id == id);
        }

        public Usuario Add(Usuario entity)
        {
            lock (_lock)
            {
                entity.Id = _nextId++;
                entity.Biblioteca ??= new Biblioteca();
                entity.Biblioteca.Id = _nextBibliotecaId++;
                entity.Biblioteca.UsuarioId = entity.Id;
                _usuarios.Add(entity);
                return entity;
            }
        }

        public bool Update(Usuario entity)
        {
            lock (_lock)
            {
                var index = _usuarios.FindIndex(u => u.Id == entity.Id);
                if (index == -1) return false;
                _usuarios[index] = entity;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock) return _usuarios.RemoveAll(u => u.Id == id) > 0;
        }

        public Usuario? GetByEmail(string email)
        {
            lock (_lock)
                return _usuarios.FirstOrDefault(u =>
                    string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public Biblioteca? GetBiblioteca(int usuarioId)
        {
            lock (_lock)
                return _usuarios.FirstOrDefault(u => u.Id == usuarioId)?.Biblioteca;
        }
    }
}
