using Core.Enum;
using Core.Interface;
using Core.Language;
using Core.Language.Instructions;
using Core.Errors;

namespace Core.Model;


public class Parser
{
    public IEnumerable<ParserError> parserErrors = [];
    private int tokenIndex;
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

    private IEnumerable<ParserError> AddParserError(int Row, int startCol, int endCol, string message)
    {
        Location location = new(Row, startCol, endCol);
        return parserErrors.Append(new ParserError(message, location));
    }

    public IInstruction Parse(Token[] tokens)
    {
        tokenIndex = 0;
        return GetBlockInst(tokens);
    }

    private IInstruction GetBlockInst(Token[] tokens)
    {
        List<IInstruction> lines = [];
        if (tokens.Length == 0) return new BlockInstruction(lines);

        do
        {
            if (tokens[tokenIndex].Type == TokenType.EndOfLine ||
            tokens[tokenIndex].Type == TokenType.EndOfFile) { tokenIndex += 1; continue; }
            //aqui se ve si es una linea de asignacion, funcion, etiqueta o GoTo
            if (GetAssignInst(tokens, out IInstruction? line))
                lines.Add(line!);
            else if (GetFuncInst(tokens, out IInstruction? funcInst))
                lines.Add(funcInst!);
            else if (GetGoToLine(tokens, out GoToInst goTo))
                lines.Add(goTo);
            else if (GetLabel(tokens, out Label label))
                lines.Add(label);
            else
            {
                for (int i = tokenIndex; i < tokens.Length; i++)
                {
                    if (tokens[i].Type == TokenType.EndOfLine ||
                     tokens[i].Type == TokenType.EndOfFile)
                    {
                        tokenIndex = i;
                        parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, "Este tipo de instruccion no es valida");
                        break;
                    }
                }
            }
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
        if (!MatchGoTo(tokens, out goTo, startIndex))
            return ResetIndex(startIndex, out goTo!);
        return true;
    }

    private bool MatchGoTo(Token[] tokens, out GoToInst goTo, int startIndex)
    {
        if (!MatchToken(tokens, TokenType.Jump))
        {
            return ReturnFalse(out goTo!);
        }
        if (!MatchToken(tokens, TokenType.CorcheteAbierto))
        {
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, "Se esperaba un corchete abierto despues del GOTO");
            return ReturnFalse(out goTo!);
        }
        if (!MatchToken(tokens, TokenType.Identificador, out string name))
        {
            var a = "Se esperaba el nombre de una etiqueta luego del corchete abierto";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
            return ReturnFalse(out goTo!);
        }
        if (!MatchToken(tokens, TokenType.CorcheteCerrado))
        {
            var b = "Se esperaba un corchete cerrado luego del nombre de la etiqueta";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, b);
            return ReturnFalse(out goTo!);
        }
        if (!MatchToken(tokens, TokenType.ParentesisAbierto))
        {
            var c = "Se esperaba un parentesis abierto despues del corchete cerrado";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, c);
            return ReturnFalse(out goTo!);
        }
        if (!GetBooleanExp(tokens, out IExpression<bool> boolean))
        {
            var d = "Se esperaba una expresion booleana despues del parentesis abierto";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, d);
            return ReturnFalse(out goTo!);
        }
        if (!MatchToken(tokens, TokenType.ParentesisCerrado))
        {
            var e = "Se esperaba un parentesis cerrado despues de la expresion booleana";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, e);
            return ReturnFalse(out goTo!);
        }

        Location location = new(tokens[startIndex].Row, tokens[startIndex].Col, tokens[tokenIndex].Col);
        goTo = new GoToInst(name, boolean, location);
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
        if (GetBooleanExp(tokens, out IExpression<bool> Boolean))
            return AssignInst(name, Boolean, out inst);
        var a = "Se esperaba una expresion aritmetica o booleana luego del asignador";
        parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
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
        if (MatchToken(tokens, TokenType.Identificador))
        {
            Location location = new(tokens[tokenIndex - 1].Row, tokens[tokenIndex - 1].Col, tokens[tokenIndex - 1].Col);
            num = new Variable<int>(token.Name, location);
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
        if (!MatchToken(tokens, TokenType.ParentesisCerrado))
            return ResetIndex(startIndex, out num);

        Location location = new(tokens[startIndex].Row, tokens[startIndex].Col, tokens[tokenIndex].Col);
        num = new FunctionExp<int>(tokens[startIndex].Name, location, @params);
        return true;
    }
    private bool MatchFuntion(Token[] tokens, out IInstruction? num, int startIndex)
    {
        if (!MatchToken(tokens, TokenType.Identificador) || !MatchToken(tokens, TokenType.ParentesisAbierto))
            return ResetIndex(startIndex, out num);
        if (!MatchParams(tokens, out string[] @params))
            return ResetIndex(startIndex, out num);
        if(!MatchToken(tokens, TokenType.ParentesisCerrado))
            return ResetIndex(startIndex, out num);

        Location location = new(tokens[startIndex].Row, tokens[startIndex].Col, tokens[tokenIndex].Col);
        num = new InstructionFunc(tokens[startIndex].Name, location, @params);
        return true;
    }


    private bool MatchParams(Token[] tokens, out string[] @params)
    {
        int paridad = (tokenIndex + 1) % 2;
        List<string> list = [];
        while (tokens[tokenIndex].Type != TokenType.ParentesisCerrado &&
        tokens[tokenIndex].Type != TokenType.EndOfLine && tokens[tokenIndex].Type != TokenType.EndOfFile)
        {
            if (tokenIndex % 2 == paridad && tokens[tokenIndex].Type != TokenType.Coma)
            {
                var a = "No estan puestas correctamente las comas";
                parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
                return ReturnFalse(out @params!);
            }
            else if (tokenIndex % 2 != paridad)
            {
                //guardo los nombres de los tokens en el array
                list.Add(tokens[tokenIndex].Name);
            }
            tokenIndex += 1;
        }
        if (tokens[tokenIndex].Type == TokenType.EndOfLine ||
        tokens[tokenIndex].Type == TokenType.EndOfFile)
        {
            var a = "Falta un parentesis que cierre los parametros";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
            ReturnFalse(out @params!);
        }
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
            {
                var a = "Se esperaba una expresion aritmetica valida a la derecha del operador";
                parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
            }
            return ReturnFalse(out num);

        }
        if (GetArithmeticExp(tokens, out right, preceed))
        {
            num = new BinaryIntExpression(left!, right!, type);
            return true;
        }
        else
        {
            var a = "Se esperaba una expresion aritmetica valida a la derecha del operador";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
        }
        return ReturnFalse(out num);
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
        return ReturnFalse(out type);
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
            return ReturnFalse(out boolean!);
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
            var a = "Se esperaba una expresion booleana valida despues del operador";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
            return ReturnFalse(out boolean!);
        }
        boolean = new BinaryBoolExpression(left, right, type);
        return true;

    }

    private bool GetIntToBoolExpression(Token[] tokens, out BinaryIntToBoolExpresion exp)
    {
        if (!GetArithmeticExp(tokens, out IExpression<int>? left) ||
        !MatchOperator(tokens, PreceedOrderBool[3], out TokenType type))
        {
            return ReturnFalse(out exp!);
        }
        if (!GetArithmeticExp(tokens, out IExpression<int>? right))
        {
            var a = "Se esperaba una expresion aritmetica valida despues del comparador";
            parserErrors = AddParserError(tokens[tokenIndex].Row, tokens[tokenIndex].Col, tokens[tokenIndex].Col, a);
            return ReturnFalse(out exp!);
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
    private static bool ReturnFalse<T>(out T? inst)
    {
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

