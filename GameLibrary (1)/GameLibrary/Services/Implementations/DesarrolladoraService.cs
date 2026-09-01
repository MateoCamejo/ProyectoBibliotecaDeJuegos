using GameLibrary.Domain;
using GameLibrary.DTOs;
using GameLibrary.Repositories.Interfaces;
using GameLibrary.Services.Exceptions;
using GameLibrary.Services.Interfaces;

namespace GameLibrary.Services.Implementations
{
    public class DesarrolladoraService : IDesarrolladoraService
    {
        private readonly IDesarrolladoraRepository _desarrolladoraRepo;

        public DesarrolladoraService(IDesarrolladoraRepository desarrolladoraRepo)
        {
            _desarrolladoraRepo = desarrolladoraRepo;
        }

        public IEnumerable<DesarrolladoraResponseDto> GetAll() =>
            _desarrolladoraRepo.GetAll().Select(ToDto);

        public DesarrolladoraResponseDto Create(DesarrolladoraCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new BusinessRuleException("El nombre de la desarrolladora es obligatorio.");

            var desarrolladora = new Desarrolladora { Nombre = dto.Nombre.Trim() };
            _desarrolladoraRepo.Add(desarrolladora);
            return ToDto(desarrolladora);
        }

        private static DesarrolladoraResponseDto ToDto(Desarrolladora d) =>
            new() { Id = d.Id, Nombre = d.Nombre };
    }
}
