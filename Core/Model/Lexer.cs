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
        {"Yelow", TokenType.Color},
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
            int size = lines[i].Length;

            for (int k = 0; k < lines[i].Length; k++)
            {
                string a = builder.ToString();
                string currentCharacter = lines[i][k].ToString();

                if (char.IsControl(lines[i][k]))
                {
                    // Ignorar caracteres de control como \n, \r, \t
                    continue;
                }


                if (currentCharacter == " ") size -= 1;

                if (currentCharacter == "\"")
                {
                    if (TextReader1(lines, i, ref k, tokens, ref builder))
                        continue;

                }
                if (k == lines[i].Length - 1 && !Delimiter.ContainsKey(currentCharacter) && currentCharacter != " ")
                {
                    builder.Append(lines[i][k]);
                    a = builder.ToString();
                }
                if (size == a.Length)
                {
                    AddToken(a, TokenType.Etiqueta, 0, i);
                    break;
                }

                //revisa si es un numero y lo agrega
                if (int.TryParse(currentCharacter, out var result) && (builder.Length == 0 || k == lines[i].Length - 1))
                {
                    NumReader(lines, i, ref k, tokens, result);
                    builder.Clear();
                    continue;
                }

                if (currentCharacter == "-" && builder.Length == 0)
                {
                    AddToken("-", TokenType.Resta, k, i);
                    continue;
                }

                //si llego a un delimitador lo convierte en un token
                if (Delimiter.ContainsKey(currentCharacter) || currentCharacter == " " || k == lines[i].Length - 1)
                {

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
        AddToken("EndOfFile", TokenType.EndOfFile, lines.Length - 1, lines[lines.Length - 1].Length - 1);
        return tokens.ToArray();
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

    private static void NumReader(string[] lines, int i, ref int k, List<Token> tokens, int result)
    {
        StringBuilder builder1 = new();

        for (int n = k; n < lines[i].Length; n++)
        {
            if (!int.TryParse(lines[i][n].ToString(), out _) || n == lines[i].Length - 1)
            {

                if (n == lines[i].Length - 1 && int.TryParse(lines[i][n].ToString(), out _)) builder1.Append(lines[i][n]);
                string b = builder1.ToString();
                Token token = new(b, TokenType.Numero, k, i);
                tokens.Add(token);
                k = n == lines[i].Length - 1 && int.TryParse(lines[i][n].ToString(), out _) ? n : n - 1;
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
}
