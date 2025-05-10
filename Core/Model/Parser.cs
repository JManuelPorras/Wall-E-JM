using Core.Enum;
using Core.Language;

namespace Core.Model;

public class Parser
{
    private int tokenIndex;
    public IInstruction Parse(Token[] tokens)
    {
        tokenIndex = 0;
        return GetBlockInst(tokens);
    }

    private IInstruction GetBlockInst(Token[] tokens)
    {
        List<IInstruction> lines = [];

        do
        {
            //aqui se ve si es una linea de asignacion, funcion o etiqueta
            if (GetAssignInst(tokens, out IInstruction? line))
                lines.Add(line!);
            // else if (GetFuncInst(tokens, out line))
            //     lines.Add(line!); 
            // Añadir otras lineas   
        } while (tokenIndex < tokens.Length);

        return new BlockInstruction(lines);
    }

    private bool GetAssignInst(Token[] tokens, out IInstruction? inst)
    {
        int startIndex = tokenIndex;
        var name = tokens[startIndex].Name;

        if (!MatchToken(tokens, TokenType.Identificador))
            return ResetIndex(startIndex, out inst);
        if (!MatchToken(tokens, TokenType.Asignador))
            return ResetIndex(startIndex, out inst);

        if (GetArithmeticExp(tokens, out IExpression<int> num))
            return AssignInst(name, num, out inst);
        if (GetStringExp(tokens, out IExpression<string> str))
            return AssignInst(name, str, out inst);
        return ResetIndex(startIndex, out inst);
    }

    // private bool GetFuncInst(Token[] tokens, out IInstruction? inst)
    // {
    //     var startIndex = tokenIndex;
    //     var name=tokens[startIndex].Name;
    //     if(!MatchToken(tokens, TokenType.Identificador))
    //     return ResetIndex(startIndex,out inst);
    // }

    private bool AssignInst<T>(string name, IExpression<T> value, out IInstruction inst)
    {
        inst = new AssignInstruction<T>(name, value);
        return true;
    }

    private bool GetArithmeticExp(Token[] tokens, out IExpression<int> num)
    {
        throw new NotImplementedException();
    }

    private bool GetStringExp(Token[] tokens, out IExpression<string> str)
    {
        throw new NotImplementedException();
    }

    private bool ResetIndex(int startIndex, out IInstruction? inst)
    {
        tokenIndex = startIndex;
        inst = null;
        return false;
    }

    private bool MatchToken(Token[] tokens, TokenType type)
    {
        return tokenIndex < tokens.Length && tokens[tokenIndex++].Type == type;
    }
}

public class AssignInstruction<T> : IInstruction
{
    public string Name { get; set; }
    public IExpression<T> Value { get; set; }

    public AssignInstruction(string name, IExpression<T> value)
    {
        Name = name;
        Value = value;
    }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}