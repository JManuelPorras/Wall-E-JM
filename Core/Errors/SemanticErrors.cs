using System.Runtime.Serialization;

namespace Core.Errors;

public class SemanticErrors : Exception
{
    public SemanticErrors()
    {
    }

    public SemanticErrors(string? message) : base(message)
    {

    }
}
