using tpintegrador_psr2026.Api.DAO.Infraestructura;
using tpintegrador_psr2026.Api.DAO.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace tpintegrador_psr2026.Api.DAO.Extensions;

public static class ServiceCollectionExtensions
{
    // Uso: builder.Services.AddDAOsSqlServer(); en Program.cs
    public static IServiceCollection AddDAOsSqlServer(this IServiceCollection services)
    {
        services.AddSingleton<ConexionSql>();

        services.AddScoped<ICategoriaDAO, CategoriaDAOSql>();
        services.AddScoped<IDesarrolladoraDAO, DesarrolladoraDAOSql>();
        services.AddScoped<IJuegoDAO, JuegoDAOSql>();
        services.AddScoped<IUsuarioDAO, UsuarioDAOSql>();
        services.AddScoped<ICompraDAO, CompraDAOSql>();
        services.AddScoped<ICampanaDAO, CampanaDAOSql>();

        return services;
    }
}
