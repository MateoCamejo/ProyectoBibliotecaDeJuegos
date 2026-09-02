using GameLibrary.DTOs;

namespace GameLibrary.Services.Interfaces
{
    public interface ICategoriaService
    {
        IEnumerable<CategoriaResponseDto> GetAll();
        CategoriaResponseDto Create(CategoriaCreateDto dto);
    }
}
