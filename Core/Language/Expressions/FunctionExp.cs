using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class FunctionExp<T> : Function, IExpression<T>, ICheckSemantic, ILocation
{
    public FunctionExp(string name, Location location, params string[] @params) : base(name, location, @params)
    {

    }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (!context.FunExp.ContainsKey(Name))
            yield return new SemanticErrors("Esta funcion no esta definida en el contexto actual", ErrorLocation);
        else
        {
            if (!MatchParams(context.FunExp[Name].Item2, out IEnumerable<SemanticErrors> errors, context))
                foreach (var item in errors)
                    yield return item;
            if (!MatchResult(context.FunExp[Name].Item3, out SemanticErrors resultError, context))
                yield return resultError;
        }
    }

    private bool MatchResult(object item3, out SemanticErrors resultError, Context context)
    {
        if (!Equals(item3, typeof(T)))
        {
            resultError = new SemanticErrors("El tipo que devuelve la funcion no es el esperado", ErrorLocation);
            return false;
        }
        resultError = default!;
        return true;
    }

    public T Execute(Context context)
    {
        Func func = context.FunExp[Name].Item1;
        object?[] objects = new object[Params.Length];
        for (int i = 0; i < Params.Length; i++)
            objects[i] = ParamType(Params[i], context);
        return (T)func(objects);
    }
}