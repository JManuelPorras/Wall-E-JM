using Core.Language;
using Core.Model;
using Action = Core.Language.Action;

namespace Test;

public class Program
{
    private static void Main(string[] args)
    {

        var lexer = new Lexer();
        var lines = Lexer.ReadAllLines("Test.pw");
        var tokens = lexer.Tokenizer(lines);
        var resp = string.Join(',', tokens.Select(x => (x.Name, x.Type)));
        Console.WriteLine(resp);
        Parser parser = new();
        var blockInst = (BlockInstruction)parser.Parse(tokens);
        var resp1 = string.Join(',', blockInst.Lines);
        Console.WriteLine(resp1);
        var parserproblems = parser.parserErrors;
        var lexerproblems = lexer.lexerErrors;
        var context = blockInst.BuildContext([], []);
        var semanticProblems = blockInst.CheckSemantic(context);

        if (lexerproblems != null)
            foreach (var item in lexerproblems)
                Console.WriteLine(item.Message);

        if (parserproblems != null)
            foreach (var item in parserproblems)
                Console.WriteLine(item.Message);

        if (semanticProblems != null)
            foreach (var item in semanticProblems!)
                Console.WriteLine(item.Message);

    }

}