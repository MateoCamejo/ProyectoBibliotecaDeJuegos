using System.Net;
using System.Text.Json;
using tpintegrador_psr2026.Api.Services.Exceptions;

namespace tpintegrador_psr2026.Api.Middleware;

// Traduce las excepciones que tiran los Services en respuestas HTTP con
// el código de estado correcto, para que los Controllers no necesiten
// try/catch (solo llaman al Service y devuelven el resultado).
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteError(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteError(context, HttpStatusCode.InternalServerError,
                "Ocurrió un error inesperado: " + ex.Message);
        }
    }

    private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var payload = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(payload);
    }
}
