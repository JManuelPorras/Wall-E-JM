using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class Variable<T>(string name) : IExpression<T>, ICheckSemantic
{
    public string Name { get; } = name;

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (context.Variables[Name] is not T)
            yield return new SemanticErrors("La variable es de un tipo incorrecto");
    }

    public T Execute(Context context) => context.Variables[Name] is T value ? value : throw new InvalidCastException();
}
