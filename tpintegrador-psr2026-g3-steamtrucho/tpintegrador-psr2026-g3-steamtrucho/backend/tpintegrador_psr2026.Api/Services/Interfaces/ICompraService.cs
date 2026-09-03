using tpintegrador_psr2026.Api.DTOs;

namespace tpintegrador_psr2026.Api.Services.Interfaces;

public interface ICompraService
{
    CompraResponseDto Create(CompraCreateDto dto);
    CompraResponseDto GetById(int id);
    CompraResponseDto Confirmar(int id);
    CompraResponseDto Cancelar(int id);
}
