using System.Runtime.Serialization;
using Core.Interface;
namespace Core.Errors;

public class SemanticErrors : Error, ILocation
{
    public SemanticErrors()
    {
    }

    public SemanticErrors(string? message, Location location) : base(message, location)
    {

    }
}
