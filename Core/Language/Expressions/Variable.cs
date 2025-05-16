namespace Core.Language;

public class Variable<T> : IExpression<T>
{
    public Variable(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public T Execute(Context context) => context.Variables[Name] is T value ? value : throw new InvalidCastException();
}
