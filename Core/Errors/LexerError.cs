using Core.Interface;
namespace Core.Errors;

public class LexerError : Error, ILocation
{

    public LexerError()
    {

    }

    public LexerError(string? message, Location location) : base(message, location)
    {

    }
}

