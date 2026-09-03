using tpintegrador_psr2026.Api.DAO;
using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.DTOs;
using tpintegrador_psr2026.Api.Services.Exceptions;
using tpintegrador_psr2026.Api.Services.Interfaces;

namespace tpintegrador_psr2026.Api.Services.Implementations;

public class DesarrolladoraService : IDesarrolladoraService
{
    private readonly IDesarrolladoraDAO _desarrolladoraDAO;

    public DesarrolladoraService(IDesarrolladoraDAO desarrolladoraDAO)
    {
        _desarrolladoraDAO = desarrolladoraDAO;
    }

    public IEnumerable<DesarrolladoraResponseDto> GetAll() =>
        _desarrolladoraDAO.ListarTodos().Select(ToDto);

    public DesarrolladoraResponseDto Create(DesarrolladoraCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new BusinessRuleException("El nombre de la desarrolladora es obligatorio.");

        var desarrolladora = new Desarrolladora { Nombre = dto.Nombre.Trim() };
        _desarrolladoraDAO.Guardar(desarrolladora);
        return ToDto(desarrolladora);
    }

    private static DesarrolladoraResponseDto ToDto(Desarrolladora d) =>
        new() { Id = d.Id, Nombre = d.Nombre };
}
