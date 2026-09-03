using tpintegrador_psr2026.Api.DAO.Extensions;
using tpintegrador_psr2026.Api.Middleware;
using tpintegrador_psr2026.Api.Services.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Steam Trucho API",
        Version = "v1",
        Description = "API REST para la biblioteca y tienda digital de videojuegos (TP Integrador PSR 2026 - Grupo 3)."
    });
});

// Cliente -> Controller -> Service -> DAO -> Datos
builder.Services.AddDAOsSqlServer();
builder.Services.AddAppServices();

var app = builder.Build();

// Manejo centralizado de excepciones: traduce NotFoundException/
// BusinessRuleException lanzadas por los Services en respuestas HTTP.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Steam Trucho API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    proyecto = "tpintegrador_psr2026",
    estado = "API funcionando"
}));

app.Run();

public partial class Program;
