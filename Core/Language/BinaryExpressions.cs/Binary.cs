using Core.Enum;
using Core.Errors;
using Core.Interface;

namespace Core.Language;

public abstract class BinaryExpression<T, K> : IExpression<T>, ICheckSemantic
{
    public BinaryExpression(IExpression<K> left, IExpression<K> right, TokenType type)
    {
        Left = left;
        Right = right;
        Type = type;
    }

    public IExpression<K> Left { get; }
    public IExpression<K> Right { get; }
    public TokenType Type { get; }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        var leftSon = Left.CheckSemantic(context)!;
        var rightSon = Right.CheckSemantic(context)!;
        foreach (var item in leftSon)
            yield return item;
        foreach (var item in rightSon)
            yield return item;
    }

    public abstract T Execute(Context context);
}
