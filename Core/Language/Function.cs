namespace Core.Language;

using Core.Errors;

public abstract class Function
{
    public Function(string name, params string[] @params)
    {
        Name = name;
        Params = @params;
    }

    public string Name { get; }
    public string[] Params { get; }

    protected bool MatchParams(Type[] types, out IEnumerable<SemanticErrors> errors, Context context)
    {
        List<SemanticErrors> semanticErrors = [];
        errors = semanticErrors;
        if (Params.Length != types.Length)
        {
            var a = new SemanticErrors("La cantidad de parametros es incorrecta");
            semanticErrors.Add(a);
            return false;
        }

        for (int i = 0; i < Params.Length; i++)
        {
            if (ParamType(Params[i], context) != types[i])
            {
                var b = new SemanticErrors("Se esperaba un parametro de otro tipo");
                semanticErrors.Add(b);
            }
        }
        return semanticErrors.Count == 0;
    }

    public static Type? ParamType(string v, Context context)
    {
        if (int.TryParse(v, out int result))
            return result.GetType();
        else if (bool.TryParse(v, out bool result1))
            return result1.GetType();
        else if (context.Colores.Contains(v))
            return v.GetType();
        else if (context.Variables.TryGetValue(v, out object? value))
            return value.GetType();
        //TODO acordarme de cuando haga el recorrido por las variables guardar el valor por default
        else
            return null;
    }

}