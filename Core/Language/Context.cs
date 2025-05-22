namespace Core.Language;

public delegate object Func(params string[] @params);
public delegate void Action(params string[] @params);

public class Context
{
    public Context(Dictionary<string, (Func, Type[], Type)> functions, Dictionary<string, (Action, Type[])> actions,
     Dictionary<string, Label>? labels, Dictionary<string, object>? variables)
    {
        Variables = variables!;
        Labels = labels!;
        FunExp = functions;
        FunInst = actions;
    }


    public Dictionary<string, object> Variables { get; set; }
    public Dictionary<string, Label> Labels { get; set; }
    public Dictionary<string, (Func, Type[], Type)> FunExp { get; set; }
    public Dictionary<string, (Action, Type[])> FunInst { get; set; }
    public readonly HashSet<string> Colores = ["Red", "Blue", "Green", "Yelow", "Orange", "Purple", "Black", "White", "Transparent"];

}

public static class Portals
{
    public static (Func, Type[]) Portal(this Func<int, string, int> call)
    {
        var @params = call.Method.GetParameters();
        var types = new Type[@params.Length];
        for (int i = 0; i < @params.Length; i++)
        {
            types[i] = @params[i].ParameterType;
        }

        // otra forma de convertir de ParamsInfo a Type
        // types = @params.Select(x => x.ParameterType).ToArray();

        object PortalFunc(params object[] @params)
        {
            return call((int)@params[0], (string)@params[1]);
        }
        return (PortalFunc, types);
    }

    public static (Func, Type[]) Portal(this Func<int, string, string> call) =>
    (
        (@params) => call((int)@params[0], (string)@params[1]),
        call.Method.GetParameters().Select(x => x.ParameterType).ToArray()
    );

    public static Func Portal<T1, T2, T3>(this Func<T1, T2, T3> call) => (@params) =>
    {
        if (@params[0] is not T1 param1)
            throw new Exception();
        if (@params[1] is not T2 param2)
            throw new Exception();
        return call(param1, param2)!;
    };
}