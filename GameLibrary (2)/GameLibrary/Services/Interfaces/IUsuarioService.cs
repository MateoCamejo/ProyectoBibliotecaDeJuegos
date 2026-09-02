using GameLibrary.DTOs;

namespace GameLibrary.Services.Interfaces
{
    public interface IUsuarioService
    {
        UsuarioResponseDto Create(UsuarioCreateDto dto);
        BibliotecaResponseDto GetBiblioteca(int usuarioId);
        IEnumerable<CompraResponseDto> GetCompras(int usuarioId);
    }
}
