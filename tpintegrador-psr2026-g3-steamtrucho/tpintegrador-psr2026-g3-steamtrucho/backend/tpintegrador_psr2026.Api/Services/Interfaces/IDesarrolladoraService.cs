using tpintegrador_psr2026.Api.DTOs;

namespace tpintegrador_psr2026.Api.Services.Interfaces;

public interface IDesarrolladoraService
{
    IEnumerable<DesarrolladoraResponseDto> GetAll();
    DesarrolladoraResponseDto Create(DesarrolladoraCreateDto dto);
}
