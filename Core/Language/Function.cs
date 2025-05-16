namespace Core.Language;
public abstract class Function
{
    public Function(string name, params object[] @params)
    {
        Name = name;
        Params = @params;
    }

    public string Name { get; }
    public object[] Params { get; }

}