using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class FunctionExp<T> : Function, IExpression<T>, ICheckSemantic
{
    public FunctionExp(string name, params string[] @params) : base(name, @params)
    {

    }

    public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        if (!context.FunInst.ContainsKey(Name))
            yield return new SemanticErrors("Esta funcion no esta definida en el contexto actual");
        else if (!MatchParams(context.FunExp[Name].Item2, out IEnumerable<SemanticErrors> errors, context))
        {
            foreach (var item in errors)
                yield return item;
        }
        if (!MatchResult(context.FunExp[Name].Item3, out SemanticErrors resultError, context))
            yield return resultError;
    }

    private bool MatchResult(object item3, out SemanticErrors resultError, Context context)
    {
        if (item3.GetType() is not T)
        {
            resultError = new SemanticErrors("El tipo que devuelve la funcion no es el esperado");
            return false;
        }
        resultError = default!;
        return true;
    }

    // TODO tengo que implementar esto todavia
    public T Execute(Context context)
    {
        return context.FunExp[Name].Item1(Params);
    }
}