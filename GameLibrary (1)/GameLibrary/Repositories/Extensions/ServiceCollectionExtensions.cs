using GameLibrary.Repositories.InMemory;
using GameLibrary.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GameLibrary.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Uso: builder.Services.AddInMemoryRepositories(); en Program.cs
        //
        // Se registran como Singleton porque son listas en memoria: si se
        // registraran como Scoped, cada request tendría su propia lista
        // "vacía" y se perdería todo entre pedidos. Cuando migren a EF Core,
        // esto pasa a ser AddScoped (para respetar el ciclo de vida del
        // DbContext) y no hay que tocar nada más de la app.
        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IJuegoRepository, InMemoryJuegoRepository>();
            services.AddSingleton<IUsuarioRepository, InMemoryUsuarioRepository>();
            services.AddSingleton<ICompraRepository, InMemoryCompraRepository>();
            services.AddSingleton<ICategoriaRepository, InMemoryCategoriaRepository>();
            services.AddSingleton<IDesarrolladoraRepository, InMemoryDesarrolladoraRepository>();
            services.AddSingleton<ICampanaRepository, InMemoryCampanaRepository>();

            return services;
        }
    }
}
