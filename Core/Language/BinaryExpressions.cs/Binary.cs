using Core.Enum;

namespace Core.Language;

public abstract class BinaryExpression<T, K> : IExpression<T>
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

    public abstract T Execute(Context context);
}

