using GameLibrary.Domain;
using GameLibrary.DTOs;
using GameLibrary.Repositories.Interfaces;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepo;

        public CategoriaService(ICategoriaRepository categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        public IEnumerable<CategoriaResponseDto> GetAll() =>
            _categoriaRepo.GetAll().Select(ToDto);

        public CategoriaResponseDto Create(CategoriaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessRuleException("El nombre de la categoría es obligatorio.");

            var categoria = new Categoria { Nombre = dto.Nombre.Trim() };
            _categoriaRepo.Add(categoria);
            return ToDto(categoria);
        }

        private static CategoriaResponseDto ToDto(Categoria c) =>
            new() { Id = c.Id, Nombre = c.Nombre };
    }
}
