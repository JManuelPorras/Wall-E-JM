using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class Literal<T>(T value) : IExpression<T>, ICheckSemantic
{
    public T Value { get; } = value;

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        return default;
    }

    public T Execute(Context context) => Value;
}
