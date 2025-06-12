using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class AssignInstruction<T> : IInstruction, ICheckSemantic
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

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        var a = Value.CheckSemantic(context)!;
        if (a != null)
        {
            foreach (var item in a)
                yield return item;
        }
        if (!context.Variables.ContainsKey(Name))
            context.Variables.Add(Name, default(T)!);
        else context.Variables[Name] = default(T)!;
    }
}