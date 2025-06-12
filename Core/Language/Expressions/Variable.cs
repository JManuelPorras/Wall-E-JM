using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class Variable<T>(string name, Location location) : IExpression<T>, ICheckSemantic, ILocation
{
    public string Name { get; } = name;

    public Location ErrorLocation { get; private set; } = location;

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (context.Variables[Name] is not T)
        {
            var a = "La variable es de un tipo incorrecto";
            yield return new SemanticErrors(a, ErrorLocation);
        }
    }

    public T Execute(Context context) => (T)context.Variables[Name];
}
