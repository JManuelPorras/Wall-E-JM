using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class Variable<T>(string name, Location location) : IExpression<T>, ICheckSemantic, ILocation
{
    public string Name { get; } = name;

    public Location ErrorLocation { get; private set; } = location; public IEnumerable<SemanticErrors>? CheckSemantic(Context context)
    {
        // Verificar que la variable existe en el contexto
        if (!context.Variables.ContainsKey(Name))
        {
            var a = "La variable no ha sido declarada en el contexto actual";
            yield return new SemanticErrors(a, ErrorLocation);
            yield break; // No continuar con más validaciones si no existe
        }

        // Verificar que el tipo de la variable coincide con T
        var variableValue = context.Variables[Name];
        if (variableValue != null && !typeof(T).IsAssignableFrom(variableValue.GetType()))
        {
            var a = "La variable es de un tipo incorrecto";
            yield return new SemanticErrors(a, ErrorLocation);
        }
    }

    public T Execute(Context context)
    {
        if (!context.Variables.ContainsKey(Name))
            throw new InvalidOperationException($"La variable '{Name}' no ha sido declarada");

        return (T)context.Variables[Name];
    }
}
