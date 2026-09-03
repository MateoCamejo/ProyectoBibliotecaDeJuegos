using tpintegrador_psr2026.Api.DTOs;

namespace tpintegrador_psr2026.Api.Services.Interfaces;

public interface ICategoriaService
{
    IEnumerable<CategoriaResponseDto> GetAll();
    CategoriaResponseDto Create(CategoriaCreateDto dto);
}
