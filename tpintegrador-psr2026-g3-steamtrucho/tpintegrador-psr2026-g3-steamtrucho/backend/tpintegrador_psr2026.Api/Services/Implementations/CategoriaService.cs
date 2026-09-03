using tpintegrador_psr2026.Api.DAO;
using tpintegrador_psr2026.Api.Domain;
using tpintegrador_psr2026.Api.DTOs;
using tpintegrador_psr2026.Api.Services.Exceptions;
using tpintegrador_psr2026.Api.Services.Interfaces;

namespace tpintegrador_psr2026.Api.Services.Implementations;

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
