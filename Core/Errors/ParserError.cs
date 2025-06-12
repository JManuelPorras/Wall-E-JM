using Core.Interface;
namespace Core.Errors;

public class ParserError : Error, ILocation
{
    public ParserError()
    {
    }

    public ParserError(string? message, Location location) : base(message, location)
    {

    }
}
