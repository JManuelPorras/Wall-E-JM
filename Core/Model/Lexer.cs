using System.ComponentModel;
using System.Text;
using Core.Enum;
using Core.Errors;

namespace Core.Model;

public class Lexer
{
    public static string GetContentPath()
    {
        string path;
#if DEBUG
        // En modo DEBUG, subimos desde el directorio de ejecución hasta la raíz del proyecto
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        path = directory
            .Parent? // bin
            .Parent? // Debug
            .Parent? // netX.Y
            .Parent? // proyecto
            .FullName!;
#else
        // En Release usamos el directorio base normal
        path = AppDomain.CurrentDomain.BaseDirectory;
#endif
        return Path.Combine(path, "contents");
    }

    public static string ReadFile(string name)
    {
        var content = GetContentPath();
        var path = Path.Combine(content, name);
        return File.ReadAllText(path);
    }

    public static string[] ReadAllLines(string name)
    {
        var content = GetContentPath();
        var path = Path.Combine(content, name);
        return File.ReadAllLines(path);
    }

    public IEnumerable<LexerError> lexerErrors = [];
    public static readonly Dictionary<string, TokenType> Delimiter = new()
    {
        {"+", TokenType.Suma},
        {"*", TokenType.Multiplicacion},
        {"/", TokenType.Division},
        {"(", TokenType.ParentesisAbierto},
        {")", TokenType.ParentesisCerrado},
        {"<", TokenType.MenorQue},
        {">", TokenType.MayorQue},
        {"%", TokenType.Modulo},
        {"&", TokenType.MediaConjuncion},
        {"|", TokenType.MediaDisyuncion},
        {",", TokenType.Coma},
        {"[", TokenType.CorcheteAbierto},
        {"]", TokenType.CorcheteCerrado},
        {"=", TokenType.Igualdad}
    };

    public static readonly Dictionary<string, TokenType> DoubleDelimiters = new()
    {
        {"<=", TokenType.MenorOIgual},
        {">=", TokenType.MayorOIgual},
        {"==", TokenType.Igualdad},
        {"&&", TokenType.Conjuncion},
        {"||", TokenType.Disyuncion},
        {"<-", TokenType.Asignador},

    };

    public static readonly Dictionary<string, TokenType> KeyWords = new()
    {
        {"GoTo", TokenType.Jump},
        {"True", TokenType.Bool},
        {"False", TokenType.Bool}
    };

    public static readonly Dictionary<string, TokenType> ArithmeticsExp = new()
    {
        {"+", TokenType.Suma},
        {"*", TokenType.Multiplicacion},
        {"/", TokenType.Division},
        {"**", TokenType.Potencia},
        {"%", TokenType.Modulo},

    };

    public static readonly Dictionary<string, TokenType> BooleanExp = new()
    {
        {"<=", TokenType.MenorOIgual},
        {">=", TokenType.MayorOIgual},
        {"==", TokenType.Igualdad},
        {"&&", TokenType.Conjuncion},
        {"||", TokenType.Disyuncion}

    };

    public static readonly Dictionary<string, TokenType> Colores = new()
    {
        {"Red", TokenType.Color},
        {"Blue", TokenType.Color},
        {"Green", TokenType.Color},
        {"Yellow", TokenType.Color},
        {"Orange", TokenType.Color},
        {"Purple", TokenType.Color},
        {"Black", TokenType.Color},
        {"White", TokenType.Color},
        {"Transparent", TokenType.Color},

    };


    public List<Token> tokens = [];
    private StringBuilder builder = new();

    //esto tokeniza
    public Token[] Tokenizer(string[] lines)
    {

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "") continue;

            for (int k = 0; k < lines[i].Length; k++)
            {
                string a = builder.ToString();
                string currentCharacter = lines[i][k].ToString();

                // 1. Si es un carácter de control, agrega el token pendiente y salta
                if (char.IsControl(lines[i][k]))
                {
                    if (builder.Length > 0)
                    {
                        if (KeyWords.TryGetValue(a, out TokenType value))
                            AddToken(a, value, k - a.Length, i);
                        else
                            AddToken(a, TokenType.Identificador, k - a.Length, i);
                        builder.Clear();
                    }
                    continue;
                }
                if (currentCharacter == "\"")
                {
                    if (TextReader1(lines, i, ref k, tokens, ref builder))
                        continue;
                }

                //revisa si es resto/numero negativo
                if (currentCharacter == "-" && builder.Length == 0)
                {
                    // Verifica si es un número negativo
                    if (IsNegativeNumber(lines, i, k))
                    {
                        NumReader(lines, i, ref k, tokens, true);
                        builder.Clear();
                        continue;
                    }
                    else
                    {
                        AddToken("-", TokenType.Resta, k, i);
                        continue;
                    }
                }

                //revisa si es un numero y lo agrega
                if (int.TryParse(currentCharacter, out var result) && builder.Length == 0)
                {
                    NumReader(lines, i, ref k, tokens, false);
                    builder.Clear();
                    continue;
                }

                //si llego a un delimitador lo convierte en un token
                if (IsDelimiter(i, k, lines) || k == lines[i].Length - 1)
                {
                    // Si es el último carácter y no es delimitador, agregar al builder primero
                    if (k == lines[i].Length - 1 && !IsDelimiter(i, k, lines))
                    {
                        builder.Append(lines[i][k]);
                        a = builder.ToString();
                    }

                    if (KeyWords.TryGetValue(a, out TokenType value))
                    {
                        AddToken(a, value, k - a.Length, i);
                    }
                    else if (a != "")
                    {
                        AddToken(a, TokenType.Identificador, k - a.Length, i);
                    }

                    if (Delimiter.TryGetValue(currentCharacter, out TokenType type))
                    {
                        if (TryDoubleCaracter(lines, currentCharacter, ref k, i))
                        { }
                        else
                            AddToken(currentCharacter, type, k, i);
                    }

                }

                //si no sigue acumulando caracteres
                else if (char.IsLetterOrDigit(lines[i][k]) || lines[i][k] == '_')
                    builder.Append(lines[i][k]);
                else
                {
                    string text1 = "Se introdujo un caracter que no es valido en este lenguaje";
                    lexerErrors = AddLexerError(i, k, k, text1);
                }
            }
            if (i != lines.Length - 1) AddToken("EndOfLine", TokenType.EndOfLine, lines[i].Length, i);
        }
        AddToken("EndOfFile", TokenType.EndOfFile, lines.Length - 1,
                 lines.Length > 0 && lines[lines.Length - 1].Length > 0 ? lines[lines.Length - 1].Length - 1 : 0);
        return tokens.ToArray();
    }

    private bool IsDelimiter(int i, int k, string[] lines)
    {
        var currentChar = lines[i][k].ToString();
        if (Delimiter.ContainsKey(currentChar) || currentChar == " ")
            return true;
        else return false;
    }

    private IEnumerable<LexerError> AddLexerError(int Row, int startCol, int endCol, string message)
    {
        Location location = new(Row, startCol, endCol);
        return lexerErrors.Append(new LexerError(message, location));
    }

    private bool TryDoubleCaracter(string[] lines, string currentCharacter, ref int k, int i)
    {
        StringBuilder builder1 = new();
        builder1.Append(currentCharacter);
        if (k != lines[i].Length - 1)
        {
            k += 1;
            builder1.Append(lines[i][k].ToString());
            string a = builder1.ToString();
            if (DoubleDelimiters.TryGetValue(a, out TokenType value))
            {
                AddToken(a, value, k, i);
                return true;
            }
            else
            {
                k -= 1;
                return false;
            }
        }
        else return false;

    }

    private void AddToken(string name, TokenType tokenType, int col, int row)
    {
        Token token = new(name, tokenType, row, col);
        tokens.Add(token);
        builder.Clear();
    }
    private static void NumReader(string[] lines, int i, ref int k, List<Token> tokens, bool negative)
    {
        StringBuilder builder1 = new();
        int startPos = k;

        // Si es negativo, empezamos desde el siguiente carácter (saltamos el '-')
        if (negative)
        {
            startPos = k + 1;
        }

        for (int n = startPos; n < lines[i].Length; n++)
        {
            if (!char.IsDigit(lines[i][n]) || n == lines[i].Length - 1)
            {
                // Si llegamos al final y es un dígito, lo incluimos
                if (n == lines[i].Length - 1 && char.IsDigit(lines[i][n]))
                    builder1.Append(lines[i][n]);

                string b = builder1.ToString();
                if (negative) b = "-" + builder1.ToString();

                Token token = new(b, TokenType.Numero, k, i);
                tokens.Add(token);

                // Ajustamos k para apuntar al último carácter procesado
                k = n == lines[i].Length - 1 && char.IsDigit(lines[i][n]) ? n : n - 1;
                return;
            }
            else
            {
                builder1.Append(lines[i][n]);
            }
        }
    }

    private bool TextReader1(string[] lines, int i, ref int k, List<Token> tokens, ref StringBuilder builder)
    {
        StringBuilder builder1 = new();
        builder.Clear();

        for (int n = k + 1; n < lines[i].Length; n++)
        {
            if (lines[i][n].ToString() == "\"")
            {

                string b = builder1.ToString();
                if (!Colores.ContainsKey(b))
                {
                    lexerErrors = AddLexerError(i, k, n, "Se esperaba un color valido en esta cadena de texto");
                }
                Token token = new(b, TokenType.Color, k + 1, i);
                tokens.Add(token);
                k = n;
                return true;
            }
            else
            {
                builder1.Append(lines[i][n]);
            }
        }
        lexerErrors = AddLexerError(i, k, lines[i].Length - 1, "Falta la comilla para cerrar la cadena de texto en la linea");
        k = lines[i].Length - 1;
        return false;
    }

    //poner continue para saltar la linea
    private void GetJumpLine(char CurrentChar, ref int col, int row)
    {
        if (char.IsControl(CurrentChar)) // Windows
        {
            AddToken("EndOfLine", TokenType.EndOfLine, col, row);
            col += 1;
        }
        else // Mac o Unix
        {
            AddToken("EndOfLine", TokenType.EndOfLine, col, row);
        }
    }

    // Método para determinar si un '-' es parte de un número negativo
    private bool IsNegativeNumber(string[] lines, int i, int k)
    {
        // Verifica si hay un siguiente carácter y si es un dígito
        if (k + 1 >= lines[i].Length || !char.IsDigit(lines[i][k + 1]))
            return false;

        // Si es el primer carácter de la línea, es un número negativo
        if (k == 0)
            return true;

        // Verifica el token anterior para determinar el contexto
        if (tokens.Count == 0)
            return true;

        var lastToken = tokens.Last();

        // Es número negativo si el token anterior es:
        // - Un delimitador de apertura: (, [, ,
        // - Un operador: +, -, *, /, %, &&, ||, <, >, <=, >=, ==
        // - Un asignador: <-
        // - Fin de línea
        return lastToken.Type switch
        {
            TokenType.ParentesisAbierto or
            TokenType.CorcheteAbierto or
            TokenType.Coma or
            TokenType.Suma or
            TokenType.Resta or
            TokenType.Multiplicacion or
            TokenType.Division or
            TokenType.Modulo or
            TokenType.Potencia or
            TokenType.MenorQue or
            TokenType.MayorQue or
            TokenType.MenorOIgual or
            TokenType.MayorOIgual or
            TokenType.Igualdad or
            TokenType.Conjuncion or
            TokenType.Disyuncion or
            TokenType.MediaConjuncion or
            TokenType.MediaDisyuncion or
            TokenType.Asignador or
            TokenType.EndOfLine => true,
            _ => false
        };
    }
}
