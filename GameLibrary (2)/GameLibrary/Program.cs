using GameLibrary.DAO.Extensions;
using GameLibrary.Middleware;
using GameLibrary.Services.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Plataforma de Videojuegos API",
        Version = "v1",
        Description = "API REST para la biblioteca y tienda digital de videojuegos (Entregas 2 y 3)."
    });
});

// Cliente -> Controller -> Service -> DAO -> Dominio
builder.Services.AddDAOsSqlServer();
builder.Services.AddAppServices();

var app = builder.Build();

// Manejo centralizado de excepciones: transforma NotFoundException/
// BusinessRuleException lanzadas por los Services en respuestas HTTP.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plataforma de Videojuegos API v1");
    c.RoutePrefix = string.Empty; // Swagger UI queda en la raíz: http://localhost:PORT/
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
