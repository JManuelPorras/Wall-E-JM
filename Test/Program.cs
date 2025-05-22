using Core.Language;
using Core.Model;
using Action = Core.Language.Action;

namespace Test;

public class Program
{
    private static void Main(string[] args)
    {
        var lines = Lexer.ReadAllLines("Test.pw");
        var tokens = Lexer.Tokenizer(lines);
        var resp = string.Join(',', tokens.Select(x => (x.Name, x.Type)));
        System.Console.WriteLine(resp);

        var context = new Context(
            new Dictionary<string, Func>
            {
                {"IsValid", IsValid},
            },
            new Dictionary<string, Action>
            {

            });
    }

    private static object IsValid(params object[] objects)
    {
        return true;
    }
}