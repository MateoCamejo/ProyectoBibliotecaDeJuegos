using tpintegrador_psr2026.Api.DTOs;

namespace tpintegrador_psr2026.Api.Services.Interfaces;

public interface IUsuarioService
{
    UsuarioResponseDto Create(UsuarioCreateDto dto);
    BibliotecaResponseDto GetBiblioteca(int usuarioId);
    IEnumerable<CompraResponseDto> GetCompras(int usuarioId);
}
