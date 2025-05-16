namespace Core.Language;

public class AssignInstruction<T> : IInstruction
{
    public string Name { get; set; }
    public IExpression<T> Value { get; set; }

    public AssignInstruction(string name, IExpression<T> value)
    {
        Name = name;
        Value = value;
    }

    void IInstruction.Execute(Context context)
    {
        context.Variables[Name] = Value.Execute(context)!;
    }
}