using System.Linq.Expressions;
using Core.Errors;
using Core.Interface;

namespace Core.Language.Instructions;

public class GoToInst(string label, IExpression<bool> exp, Location location) : IInstruction, ICheckSemantic, ILocation
{
    public string Label { get; } = label;
    public IExpression<bool> Exp { get; } = exp;
    public Location ErrorLocation { get; private set; } = location;

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        var a = Exp.CheckSemantic(context);
        if (a != null)
        {
            foreach (var item in a)
                yield return item;
        }
        if (!context.Labels.ContainsKey(Label))
            yield return new SemanticErrors("No existe la etiqueta en el contexto actual", ErrorLocation);
    }

    public void Execute(Context context)
    {
        if (Exp.Execute(context))
        {
            context.IsGoTo = true;
            context.CurrentLabel = Label;
        }
    }
}