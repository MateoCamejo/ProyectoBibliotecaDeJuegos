using GameLibrary.Services.Implementations;
using GameLibrary.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GameLibrary.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Uso: builder.Services.AddAppServices(); en Program.cs
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IDesarrolladoraService, DesarrolladoraService>();
            services.AddScoped<ICampanaService, CampanaService>();
            services.AddScoped<IVideojuegoService, VideojuegoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ICompraService, CompraService>();

            return services;
        }
    }
}
