namespace Core.Language;

public class Context
{
    public Context(Dictionary<string, Func> functions, Dictionary<string, Action> actions)
    {
        Variables = [];
        Functions = functions;
        Actions = actions;
    }

    public delegate object Func(params object[] @params);
    public delegate void Action(params object[] @params);

    public Dictionary<string, object> Variables { get; set; }
    public Dictionary<string, Func> Functions { get; set; }
    public Dictionary<string, Action> Actions { get; set; }

}