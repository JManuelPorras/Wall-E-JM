using Core.Errors;
using Core.Interface;

namespace Core.Language.Instructions;

public class GoToInst(string label, IExpression<bool> exp) : IInstruction, ICheckSemantic
{
    public string Label { get; } = label;
    public IExpression<bool> Exp { get; } = exp;

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        var a = Exp.CheckSemantic(context);
        if (a != null)
        {
            foreach (var item in a)
                yield return item;
        }
        if (!context.Labels.ContainsKey(Label))
            yield return new SemanticErrors("No existe la etiqueta en el contexto actual");
    }

    public void Execute(Context context)
    {
        throw new NotImplementedException();
    }
}