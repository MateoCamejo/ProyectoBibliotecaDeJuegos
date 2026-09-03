using tpintegrador_psr2026.Api.DTOs;

namespace tpintegrador_psr2026.Api.Services.Interfaces;

public interface IVideojuegoService
{
    IEnumerable<JuegoResponseDto> GetAll(string? nombre = null);
    JuegoResponseDto GetById(int id);
    IEnumerable<JuegoResponseDto> GetByCategoria(int categoriaId);
    IEnumerable<JuegoResponseDto> GetByDesarrolladora(int desarrolladoraId);
    PrecioActualResponseDto GetPrecioActual(int id);
    JuegoResponseDto Create(JuegoCreateDto dto);
    JuegoResponseDto Update(int id, JuegoUpdateDto dto);
}
