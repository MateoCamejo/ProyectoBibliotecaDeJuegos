namespace tpintegrador_psr2026.Api.Services.Exceptions;

// Se lanza cuando se viola una regla de negocio (comprar un juego ya
// poseído, un juego no disponible, datos inválidos, etc.).
// El middleware la traduce a un 400 Bad Request.
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
