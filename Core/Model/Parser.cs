using Core.Enum;
using Core.Interface;
using Core.Language;
using Core.Language.Instructions;
using System.Data;
using System.Text;

namespace Core.Model;

public class Parser
{
    private readonly Dictionary<int, TokenType[]> PreceedOrderInt = new()
     {
        {1,[TokenType.Resta, TokenType.Suma]},
        {2,[TokenType.Multiplicacion,TokenType.Modulo, TokenType.Division]},
        {3,[TokenType.Potencia]},
        {4,[TokenType.Numero, TokenType.Identificador, TokenType.ParentesisAbierto]}
     };
    private readonly Dictionary<int, TokenType[]> PreceedOrderBool = new()
     {
        {1,[TokenType.Conjuncion]},
        {2,[TokenType.Disyuncion]},
        {3,[TokenType.Igualdad, TokenType.MayorOIgual, TokenType.MayorQue,
        TokenType.MenorOIgual, TokenType.MenorQue]},
     };

    private int tokenIndex;

    public IInstruction Parse(Token[] tokens)
    {
        tokenIndex = 0;
        return GetBlockInst(tokens);
    }

    private IInstruction GetBlockInst(Token[] tokens)
    {
        List<IInstruction> lines = [];
        IEnumerable<IInstruction> a = lines;

        do
        {
            if (tokens[tokenIndex].Type == TokenType.EndOfLine ||
            tokens[tokenIndex].Type == TokenType.EndOfFile) tokenIndex += 1;
            //aqui se ve si es una linea de asignacion, funcion, etiqueta o GoTo
            if (GetAssignInst(tokens, out IInstruction? line))
                lines.Add(line!);
            else if (GetFuncInst(tokens, out IInstruction? funcInst))
                lines.Add(funcInst!);
            else if (GetGoToLine(tokens, out GoToInst goTo))
                lines.Add(goTo);
            else if (GetLabel(tokens, out Label label))
                lines.Add(label);
            else throw new InvalidExpressionException("Esta instruccion no es valida");

            // Añadir otras lineas   
        } while (tokenIndex < tokens.Length);

        return new BlockInstruction(lines);
    }

    private bool GetLabel(Token[] tokens, out Label label)
    {
        int startIndex = tokenIndex;
        if (!MatchToken(tokens, TokenType.Etiqueta, out string name))
            return ResetIndex(startIndex, out label!);
        label = new Label(name);
        return true;
    }

    private bool GetGoToLine(Token[] tokens, out GoToInst goTo)
    {
        int startIndex = tokenIndex;
        if (!MatchGoTo(tokens, out goTo))
            return ResetIndex(startIndex, out goTo!);
        return true;
    }

    private bool MatchGoTo(Token[] tokens, out GoToInst goTo)
    {
        if (!MatchToken(tokens, TokenType.Jump) ||
         !MatchToken(tokens, TokenType.CorcheteAbierto) ||
         !MatchToken(tokens, TokenType.Identificador, out string name) ||
         !MatchToken(tokens, TokenType.CorcheteCerrado) ||
         !MatchToken(tokens, TokenType.ParentesisAbierto) ||
         !GetBooleanExp(tokens, out IExpression<bool> boolean) ||
         !MatchToken(tokens, TokenType.ParentesisCerrado)
         )

        {
            goTo = default!;
            return false;
        }

        goTo = new GoToInst(name, boolean);
        return true;
    }

    private bool GetAssignInst(Token[] tokens, out IInstruction? inst)
    {
        int startIndex = tokenIndex;
        var name = tokens[startIndex].Name;

        if (!MatchToken(tokens, TokenType.Identificador))
            return ResetIndex(startIndex, out inst);
        if (!MatchToken(tokens, TokenType.Asignador))
            return ResetIndex(startIndex, out inst);

        if (GetArithmeticExp(tokens, out IExpression<int>? num))
            return AssignInst(name, num!, out inst);
        ResetIndex(startIndex, out inst);
        if (GetBooleanExp(tokens, out IExpression<bool> Boolean))
            return AssignInst(name, Boolean, out inst);
        return ResetIndex(startIndex, out inst);
    }

    private bool GetFuncInst(Token[] tokens, out IInstruction? inst)
    {
        var startIndex = tokenIndex;
        if (!MatchFuntion(tokens, out inst, startIndex))
        {
            return ResetIndex(startIndex, out inst);
        }
        return true;
    }

    private static bool AssignInst<T>(string name, IExpression<T> value, out IInstruction inst)
    {
        inst = new AssignInstruction<T>(name, value);
        return true;
    }

    private bool GetArithmeticExp(Token[] tokens, out IExpression<int>? num)
    {
        return GetArithmeticExp(tokens, out num, 1);
    }

    private bool GetArithmeticExp(Token[] tokens, out IExpression<int>? num, int preceed)
    {
        int startIndex = tokenIndex;

        if (preceed == 4)
            return GetLiteral(tokens, out num);
        if (!GetArithmeticExp(tokens, out IExpression<int>? left, preceed + 1))
            return ResetIndex(startIndex, out num);
        return GetArithmeticExp(tokens, left, out num, preceed);
    }

    private bool GetLiteral(Token[] tokens, out IExpression<int>? num)
    {
        var token = tokens[tokenIndex];
        if (MatchToken(tokens, TokenType.Numero) && int.TryParse(token.Name, out int value))
        {
            num = new Literal<int>(value);
            return true;
        }
        if (MatchFuntion(tokens, out num, tokenIndex))
        {
            return true;
        }
        //el visitor despues tiene que revisar la variable para ver si contiene realmente un entero, lo mismo para la funcion
        if (MatchToken(tokens, TokenType.Identificador))
        {
            num = new Variable<int>(token.Name);
            return true;
        }
        if (MatchToken(tokens, TokenType.ParentesisAbierto) && GetArithmeticExp(tokens, out num, 1) && MatchToken(tokens, TokenType.ParentesisCerrado))
        {
            return true;
        }
        return false;
    }

    private bool MatchFuntion(Token[] tokens, out IExpression<int>? num, int startIndex)
    {
        if (!MatchToken(tokens, TokenType.Identificador) || !MatchToken(tokens, TokenType.ParentesisAbierto))
            return ResetIndex(startIndex, out num);
        if (!MatchParams(tokens, out string[] @params))
            return ResetIndex(startIndex, out num);
        num = new FunctionExp<int>(tokens[startIndex].Name, @params);
        return true;
    }
    private bool MatchFuntion(Token[] tokens, out IInstruction? num, int startIndex)
    {
        if (!MatchToken(tokens, TokenType.Identificador) || !MatchToken(tokens, TokenType.ParentesisAbierto))
            return ResetIndex(startIndex, out num);
        if (!MatchParams(tokens, out string[] @params))
        {
            num = default;
            return false;
        }
        num = new InstructionFunc(tokens[startIndex].Name, @params);
        return true;
    }


    private bool MatchParams(Token[] tokens, out string[] @params)
    {
        //BUG 
        //lugar donde deben ir las comas
        int paridad = (tokenIndex + 1) % 2;
        List<string> list = [];
        while (tokens[tokenIndex].Type != TokenType.ParentesisCerrado &&
        tokens[tokenIndex].Type != TokenType.EndOfLine && tokens[tokenIndex].Type != TokenType.EndOfFile)
        {
            if (tokenIndex % 2 == paridad && tokens[tokenIndex].Type != TokenType.Coma)
            {

                @params = default!;
                return false;
            }
            else
            {
                //guardo los nombres de los tokens en el array
                list.Add(tokens[tokenIndex].Name);
            }
            tokenIndex += 1;
        }
        if (tokens[tokenIndex].Type == TokenType.EndOfLine)
        {

            @params = default!;
            return false;
        }
        tokenIndex++;
        @params = list.ToArray();
        return true;
    }

    private void SkipLine(Token[] tokens)
    {
        while (tokens[tokenIndex].Type != TokenType.EndOfLine)
        {
            tokenIndex += 1;
        }
        tokenIndex += 1;
    }

    private bool GetArithmeticExp(Token[] tokens, IExpression<int>? left, out IExpression<int>? num, int preceed)
    {
        IExpression<int>? right;
        if (!MatchOperator(tokens, PreceedOrderInt[preceed], out TokenType type))
        {
            num = left;
            return true;
        }
        if (type is TokenType.Resta or TokenType.Division)
        {
            if (GetArithmeticExp(tokens, out right, preceed + 1))
            {
                var node = new BinaryIntExpression(left!, right!, type);
                return GetArithmeticExp(tokens, node, out num, preceed);
            }
            else
                throw new InvalidOperationException();
        }
        if (GetArithmeticExp(tokens, out right, preceed))
        {
            num = new BinaryIntExpression(left!, right!, type);
            return true;
        }
        else
            throw new InvalidOperationException();

    }

    private bool MatchOperator(Token[] tokens, TokenType[] tokenTypes, out TokenType type)
    {
        for (int i = 0; i < tokenTypes.Length; i++)
        {
            if (tokenTypes[i] == tokens[tokenIndex].Type)
            {
                tokenIndex++;
                type = tokenTypes[i];
                return true;
            }
        }
        type = default;
        return false;
    }

    //metodo portal, para pasarle la precedencia
    private bool GetBooleanExp(Token[] tokens, out IExpression<bool> boolean)
    {
        return GetBooleanExp(tokens, out boolean, 1);
    }

    private bool GetBooleanExp(Token[] tokens, out IExpression<bool> boolean, int preceed)
    {
        var startIndex = tokenIndex;
        if (preceed == 3)
        {
            if (GetIntToBoolExpression(tokens, out BinaryIntToBoolExpresion exp))
            {
                boolean = exp;
                return true;
            }
            boolean = default!;
            return false;
        }
        if (!GetBooleanExp(tokens, out IExpression<bool> left, preceed + 1))
            return ResetIndex(startIndex, out boolean!);
        if (!MatchOperator(tokens, PreceedOrderBool[preceed], out TokenType type))
        {
            boolean = left;
            return true;
        }
        if (!GetBooleanExp(tokens, out IExpression<bool> right, preceed))
        {
            throw new InvalidOperationException("Se esperaba una expresion booleana despues del operador.");
        }
        boolean = new BinaryBoolExpression(left, right, type);
        return true;

    }

    private bool GetIntToBoolExpression(Token[] tokens, out BinaryIntToBoolExpresion exp)
    {
        if (!GetArithmeticExp(tokens, out IExpression<int>? left) ||
        !MatchOperator(tokens, PreceedOrderBool[3], out TokenType type) ||
        !GetArithmeticExp(tokens, out IExpression<int>? right))
        {
            exp = default!;
            return false;
        }
        exp = new BinaryIntToBoolExpresion(left!, right!, type);
        return true;
    }

    private bool ResetIndex<T>(int startIndex, out T? inst)
    {
        tokenIndex = startIndex;
        inst = default;
        return false;
    }

    private bool MatchToken(Token[] tokens, TokenType type)
    {
        if (tokenIndex >= tokens.Length || tokens[tokenIndex].Type != type)
            return false;
        tokenIndex++;
        return true;
    }
    private bool MatchToken(Token[] tokens, TokenType type, out string name)
    {
        if (tokenIndex >= tokens.Length || tokens[tokenIndex].Type != type)
        {
            name = null!;
            return false;
        }
        name = tokens[tokenIndex].Name;
        tokenIndex++;
        return true;
    }
}

