using GameLibrary.Domain;
using GameLibrary.DTOs;

namespace GameLibrary.Services.Interfaces
{
    public interface ICampanaService
    {
        IEnumerable<CampanaResponseDto> GetActivas();
        CampanaResponseDto Create(CampanaCreateDto dto);

        // Usados internamente por otros Services (Videojuego, Compra) para
        // resolver el precio real de un juego en un momento determinado.
        Campana? ObtenerMejorPromocion(Juego juego, DateTime fecha);
        decimal CalcularPrecioFinal(Juego juego);
    }
}
