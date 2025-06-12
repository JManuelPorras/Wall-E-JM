using Core.Errors;
using Core.Language;

using Core.Model;
using Action = Core.Language.Action;

namespace ViewModel;

public class CoreManager
{
    public BlockInstruction blockInstruction;
    public IEnumerable<Error>? errors;
    public Context context;
    public CoreManager(string[] lines, Dictionary<string, (Func, Type[], Type)> FuncExp, Dictionary<string, (Action, Type[])> FunInst)
    {

        var lexer = new Lexer();
        var tokens = lexer.Tokenizer(lines);

        Parser parser = new();
        var blockInst = (BlockInstruction)parser.Parse(tokens);

        var parserproblems = parser.parserErrors;
        var lexerproblems = lexer.lexerErrors;
        context = blockInst.BuildContext(FuncExp, FunInst);
        
        var semanticProblems = blockInst.CheckSemantic(context);
        errors = ConcatErrors(lexerproblems, parserproblems, semanticProblems);

        blockInstruction = blockInst;
    }

    private static IEnumerable<Error>? ConcatErrors(IEnumerable<LexerError> lexerproblems, IEnumerable<ParserError> parserproblems, IEnumerable<SemanticErrors>? semanticProblems)
    {
        if (lexerproblems != null)
            foreach (var item in lexerproblems)
                yield return item;

        if (parserproblems != null)
            foreach (var item in parserproblems)
                yield return item;

        if (semanticProblems != null)
            foreach (var item in semanticProblems)
                yield return item;
    }
}