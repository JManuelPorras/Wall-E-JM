using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class BlockInstruction : IInstruction, ICheckSemantic
{
    private readonly List<IInstruction> lines;

    public BlockInstruction(List<IInstruction> lines)
    {
        this.lines = lines;
        Lines = lines;
    }

    public List<IInstruction> Lines { get; }

    private Context BuildContext(Dictionary<string, (Func, Type[], Type)> FuncExp, Dictionary<string, (Action, Type[])> FunInst)
    {
        var variables = new Dictionary<string, object>();
        var labels = new Dictionary<string, Label>();
        foreach (var item in lines)
        {
            if (item is Label label)
                labels.Add(label.Name!, label);
            if (item is Variable<Type> variable)
                //TODO revisar si el DefaultBinger devuelve el valor predeteriminado del tipo
                variables.Add(variable.Name, Type.DefaultBinder);

        }
        var context = new Context(FuncExp, FunInst, labels, variables);
        return context;
    }

    void IInstruction.Execute(Context context)
    {
        foreach (var item in Lines)
        {
            item.Execute(context);
        }
    }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        foreach (var item in Lines)
        {
            var a = item.CheckSemantic(context);
            foreach (var item1 in a!)
                yield return item1;
        }
    }
}
