using GameLibrary.DTOs;

namespace GameLibrary.Services.Interfaces
{
    public interface ICompraService
    {
        CompraResponseDto Create(CompraCreateDto dto);
        CompraResponseDto GetById(int id);
        CompraResponseDto Confirmar(int id);
        CompraResponseDto Cancelar(int id);
    }
}
