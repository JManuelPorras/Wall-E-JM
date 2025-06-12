using Core.Interface;
namespace Core.Errors;

public class Error : Exception, ILocation
{
    public Location ErrorLocation { get; protected set; }

    public Error()
    {

    }

    public Error(string? message, Location location) : base(message)
    {
        ErrorLocation = location;
    }

    public override string ToString()
    {
        return $"{Message}. Fila:{ErrorLocation.Row}, Columna:{ErrorLocation.StartCol}.";
    }
}

public record struct Location(int Row, int StartCol, int EndCol);
