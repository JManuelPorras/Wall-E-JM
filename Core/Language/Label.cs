using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class Label : IInstruction, ICheckSemantic
{
    public Label(string name)
    {
        Name = name;
    }

    public string? Name { get; }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (context.Labels.ContainsKey(Name!))
            yield return new SemanticErrors("Existe otra etiqueta con el mismo nombre");
    }

    void IInstruction.Execute(Context context)
    {

    }
}