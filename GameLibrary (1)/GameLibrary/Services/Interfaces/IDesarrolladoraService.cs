using GameLibrary.DTOs;

namespace GameLibrary.Services.Interfaces
{
    public interface IDesarrolladoraService
    {
        IEnumerable<DesarrolladoraResponseDto> GetAll();
        DesarrolladoraResponseDto Create(DesarrolladoraCreateDto dto);
    }
}
