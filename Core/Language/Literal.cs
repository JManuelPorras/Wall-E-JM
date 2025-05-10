namespace Core.Language;

public class Literal<T> : IExpression<T>
{
    public Literal(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public T Execute() => Value;
}