using System.Reflection;
using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class InstructionFunc : Function, IInstruction, ICheckSemantic, ILocation
{
    public InstructionFunc(string name, Location location, params string[] @params) : base(name, location, @params)
    {

    }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (!context.FunInst.ContainsKey(Name))
            yield return new SemanticErrors("Esta funcion no esta definida en el contexto actual", ErrorLocation);
        else if (!MatchParams(context.FunInst[Name].Item2, out IEnumerable<SemanticErrors> errors, context))
        {
            foreach (var item in errors)
                yield return item;
        }
    }

    public void Execute(Context context)
    {
        Action func = context.FunInst[Name].Item1;
        object?[] objects = new object[Params.Length];
        for (int i = 0; i < Params.Length; i++)
            objects[i] = ParamType(Params[i], context);
        func(objects);
    }
}