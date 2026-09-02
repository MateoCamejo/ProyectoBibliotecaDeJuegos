namespace GameLibrary.Services.Exceptions
{
    // Se lanza cuando se busca una entidad por Id y no existe.
    // El middleware la traduce a un 404 Not Found.
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
