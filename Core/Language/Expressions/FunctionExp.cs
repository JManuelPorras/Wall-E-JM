namespace Core.Language;

public class FunctionExp<T> : Function, IExpression<T>
{
    public FunctionExp(string name, params object[] @params) : base(name, @params)
    {
        Name = name;
        Params = @params;
    }

    public string Name { get; }
    public object[] Params { get; }

    //tengo que implementar esto todavia
    public T Execute(Context context) => throw new NotImplementedException();
}