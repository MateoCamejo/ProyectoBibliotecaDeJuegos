using GameLibrary.DAO;
using GameLibrary.Domain;
using GameLibrary.DTOs;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaDAO _categoriaDAO;

        public CategoriaService(ICategoriaDAO categoriaDAO)
        {
            _categoriaDAO = categoriaDAO;
        }

        public IEnumerable<CategoriaResponseDto> GetAll() =>
            _categoriaDAO.ListarTodos().Select(ToDto);

        public CategoriaResponseDto Create(CategoriaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessRuleException("El nombre de la categoría es obligatorio.");

            var categoria = new Categoria { Nombre = dto.Nombre.Trim() };
            _categoriaDAO.Guardar(categoria);
            return ToDto(categoria);
        }

        private static CategoriaResponseDto ToDto(Categoria c) =>
            new() { Id = c.Id, Nombre = c.Nombre };
    }
}
