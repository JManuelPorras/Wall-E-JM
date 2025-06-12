using Core.Errors;
using Core.Interface;

namespace Core.Language;

public delegate object Func(params object?[] @params);
public delegate void Action(params object?[] @params);

public class Context(Dictionary<string, (Func, Type[], Type)> functions, Dictionary<string, (Action, Type[])> actions,
 Dictionary<string, int>? labels)
{
    public string? CurrentLabel { get; set; }
    public bool IsGoTo { get; set; }
    public Dictionary<string, object> Variables { get; set; } = [];
    public Dictionary<string, int> Labels { get; set; } = labels!;
    public Dictionary<string, (Func, Type[], Type)> FunExp { get; set; } = functions;
    public Dictionary<string, (Action, Type[])> FunInst { get; set; } = actions;
    public readonly HashSet<string> Colors = ["Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Black", "White", "Transparent"];

    // public Dictionary<string, ContextErrors> CheckSemantic()
    // {
    //     var a = CheckDictionary(Variables);
    //     var b = CheckDictionary(Labels);
    //     var c = CheckDictionary(FunExp);
    //     var d = CheckDictionary(FunInst);
    //     var e = a.Concat(b);
    //     var f = e.Concat(c);
    //     var g = f.Concat(d);
    //     return (Dictionary<string, ContextErrors>)g;
    // }

    // private static Dictionary<string, ContextErrors> CheckDictionary<K>(Dictionary<string, K> dictionary)
    // {
    //     Dictionary<string, ContextErrors> errors = [];
    //     var CheckedErrors = new List<string>();
    //     foreach (var item in dictionary)
    //     {
    //         var (Key, Value) = (item.Key, item.Value);
    //         dictionary.Remove(item.Key);
    //         if (dictionary.ContainsKey(Key) && !CheckedErrors.Contains(Key))
    //         //BUG
    //         {
    //             CheckedErrors.Add(Key);
    //             errors.Add(Key, new ContextErrors($"Hay mas de un elemento con el nombre {Key} en el contexto actual"));
    //         }
    //         dictionary.Add(Key, Value);
    //     }
    //     return errors;
    // }
}
