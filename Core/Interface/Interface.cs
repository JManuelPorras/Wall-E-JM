using Core.Errors;
using Core.Language;

namespace Core.Interface;

public interface IInstruction : ICheckSemantic
{
    void Execute(Context context);
}

public interface IExpression<T> : ICheckSemantic
{
    T Execute(Context context);
}

public interface ICheckSemantic
{
    IEnumerable<SemanticErrors>? CheckSemantic(Context context);
}

public interface ILocation
{
    public Location ErrorLocation { get; }
}
