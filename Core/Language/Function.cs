namespace Core.Language;

using Core.Interface;
using Core.Errors;

public abstract class Function : ILocation
{
    public Function(string name, Location errorLocation, params string[] @params)
    {
        ErrorLocation = errorLocation;
        Name = name;
        Params = @params;
    }

    public Location ErrorLocation { get; private set; }
    public string Name { get; }
    public string[] Params { get; }

    protected bool MatchParams(Type[] types, out IEnumerable<SemanticErrors> errors, Context context)
    {
        List<SemanticErrors> semanticErrors = [];
        errors = semanticErrors;
        if (Params.Length != types.Length)
        {
            var b = "La cantidad de parametros es incorrecta";
            var a = new SemanticErrors(b, ErrorLocation);
            semanticErrors.Add(a);
            return false;
        }

        for (int i = 0; i < Params.Length; i++)
        {
            if (ParamType(Params[i], context)?.GetType() != types[i])
            {
                var b = new SemanticErrors("Se esperaba un parametro de otro tipo", ErrorLocation);
                semanticErrors.Add(b);
            }
        }
        return semanticErrors.Count == 0;
    }

    public static object? ParamType(string v, Context context)
    {
        if (int.TryParse(v, out int result))
            return result;
        else if (bool.TryParse(v, out bool result1))
            return result1;
        else if (context.Colors.Contains(v))
            return v;
        else if (context.Variables.TryGetValue(v, out object? value))
            return value;
        else
            return null;
    }
}