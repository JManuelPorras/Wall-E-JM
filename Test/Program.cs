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

        Console.WriteLine("=== TOKENS ===");
        foreach (var token in tokens)
        {
            Console.WriteLine($"({token.Name}, {token.Type})");
        }

        Console.WriteLine("\n=== PARSER ===");
        Parser parser = new();
        var blockInst = (BlockInstruction)parser.Parse(tokens);

        Console.WriteLine("Instructions:");
        foreach (var instruction in blockInst.Lines)
        {
            Console.WriteLine($"- {instruction.GetType().Name}");
        }

        Console.WriteLine("\n=== CONTEXT ===");
        var context = blockInst.BuildContext([], []);
        Console.WriteLine("Labels:");
        foreach (var label in context.Labels)
        {
            Console.WriteLine($"- {label.Key} at position {label.Value}");
        }

        Console.WriteLine("\n=== ERRORS ===");
        var lexerproblems = lexer.lexerErrors;
        var parserproblems = parser.parserErrors;
        var semanticProblems = blockInst.CheckSemantic(context);

        if (lexerproblems != null && lexerproblems.Any())
        {
            Console.WriteLine("Lexer errors:");
            foreach (var item in lexerproblems)
                Console.WriteLine($"- {item.Message}");
        }

        if (parserproblems != null && parserproblems.Any())
        {
            Console.WriteLine("Parser errors:");
            foreach (var item in parserproblems)
                Console.WriteLine($"- {item.Message}");
        }

        if (semanticProblems != null && semanticProblems.Any())
        {
            Console.WriteLine("Semantic errors:");
            foreach (var item in semanticProblems!)
                Console.WriteLine($"- {item.Message}");
        }
        else
        {
            Console.WriteLine("No semantic errors!");
        }

        if (int.TryParse("-2", out int result))
        {
            Console.WriteLine($"{result} es un numero");
        }
        else Console.WriteLine($"{result} es un string");
    }
}