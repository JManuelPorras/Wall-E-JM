using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class BlockInstruction : IInstruction, ICheckSemantic
{

    public BlockInstruction(List<IInstruction> lines)
    {
        Lines = lines;
    }

    public List<IInstruction> Lines { get; }

    public Context BuildContext(Dictionary<string, (Func, Type[], Type)> FuncExp, Dictionary<string, (Action, Type[])> FunInst)
    {
        var labels = new Dictionary<string, int>();
        if (Lines != null)
            for (int i = 0; i < Lines.Count; i++)
            {
                IInstruction? item = Lines[i];
                if (item is Label label && !labels.ContainsKey(label.Name!))
                {
                    labels.Add(label.Name!, i);
                }
            }
        var context = new Context(FuncExp, FunInst, labels);
        return context;
    }

    public void Execute(Context context)
    {
        for (int i = 0; i < Lines.Count; i++)
        {
            IInstruction? item = Lines[i];
            item.Execute(context);
            if (context.IsGoTo)
            {
                i = context.Labels[context.CurrentLabel!];
                context.IsGoTo = false;
            }
        }
    }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (Lines != null)
            foreach (var item in Lines)
            {
                var a = item.CheckSemantic(context);
                if (a != null)
                    foreach (var item1 in a!)
                        yield return item1;
            }
    }
}
