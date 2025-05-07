using Core.Model;

namespace Test;

public class Program
{
    private static void Main(string[] args)
    {
        var lines = Lexer.ReadAllLines("Test.pw");
        var tokens = Lexer.Tokenizer(lines);
        var resp = string.Join(',', tokens.Select(x => (x.Name, x.Type)));
        System.Console.WriteLine(resp);
    }
}