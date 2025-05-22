using System.Reflection;
using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class InstructionFunc : Function, IInstruction, ICheckSemantic
{
    public InstructionFunc(string name, params string[] @params) : base(name, @params)
    {

    }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (!context.FunInst.ContainsKey(Name))
            yield return new SemanticErrors("Esta funcion no esta definida en el contexto actual");
        else if (!MatchParams(context.FunInst[Name].Item2, out IEnumerable<SemanticErrors> errors, context))
        {
            foreach (var item in errors)
                yield return item;
        }
    }


    // TODO tengo que implementar esto todavia
    public void Execute(Context context) => throw new NotImplementedException();
}