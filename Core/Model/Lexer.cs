using System.Text;
using Core.Enum;

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
        {",", TokenType.Coma}
    };

    public static readonly Dictionary<string, TokenType> DoubleDelimiters = new()
    {
        {"<=", TokenType.MenorOIgual},
        {">=", TokenType.MayorOIgual},
        {"==", TokenType.Igualdad},
        {"&&", TokenType.Conjuncion},
        {"||", TokenType.Disyuncion}
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


    static List<Token> tokens = [];
    static StringBuilder builder = new();

    //esto tokeniza
    public static List<Token> Tokenizer(string[] lines)
    {


        for (int i = 0; i < lines.Length; i++)
        {
            for (int k = 0; k < lines[i].Length; k++)
            {
                string a = builder.ToString();

                string currentCharacter = lines[i][k].ToString();
                if (currentCharacter == "\"")
                {
                    TextReader1(lines, ref i, ref k, tokens, ref builder);
                    continue;

                }
                if (k == lines[i].Length - 1)
                {
                    builder.Append(lines[i][k]);
                    a = builder.ToString();
                }
                if (a.Length == lines[i].Length)
                {
                    AddToken(a, TokenType.Etiqueta, 0, i);
                    break;
                }

                //revisa si es un numero y lo agrega
                if (int.TryParse(currentCharacter, out var result) && builder.Length == 0)
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
                        if (TryDoubleCaracter(lines, currentCharacter, ref k, i)) ;
                        else
                            AddToken(currentCharacter, type, k, i);
                    }

                    //esto es por si se me olvida poner un delimitador en las palabras clave,
                    //que no sean las comillas o el espacio
                    else if (k != lines[i].Length - 1 && currentCharacter != " ") Console.WriteLine("No esta puesto {0} en las palabras claves", currentCharacter);
                }

                //si no sigue acumulando caracteres
                else if (char.IsLetterOrDigit(lines[i][k]) || lines[i][k] == '-')
                    builder.Append(lines[i][k]);
                else throw new InvalidDataException("Se esperaba una letra, un numero o '-'.");

            }
            AddToken("EndOfFile", TokenType.EndOfFile, lines[i].Length, i);
        }
        return tokens;
    }

    private static bool TryDoubleCaracter(string[] lines, string currentCharacter, ref int k, int i)
    {
        StringBuilder builder1 = new();
        builder1.Append(currentCharacter);
        if (k != lines[i].Length - 1)
        {
            builder1.Append(lines[i][k].ToString());
            string a = builder1.ToString();
            if (DoubleDelimiters.TryGetValue(a, out TokenType value))
            {
                AddToken(a, value, k, i);
                k += 1;
                return true;
            }
            else return false;
        }
        else return false;

    }

    private static void AddToken(string name, TokenType tokenType, int col, int row)
    {
        Token token = new(name, tokenType, col, row);
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

                if (n == lines[i].Length - 1) builder1.Append(lines[i][n]);
                string b = builder1.ToString();
                Token token = new(b, TokenType.Numero, k, i);
                tokens.Add(token);
                k = n == lines[i].Length - 1 ? n : n - 1;
                return;
            }

            else
            {
                builder1.Append(lines[i][n]);
            }
        }

    }

    private static void TextReader1(string[] lines, ref int i, ref int k, List<Token> tokens, ref StringBuilder builder)
    {
        StringBuilder builder1 = new();

        //esto va hasta el final de la linea, si no ha encontrado otra comilla salta de linea
        for (int m = i; m < lines.Length; m++)
        {
            for (int n = m == i ? k + 1 : 0; n < lines[i].Length; n++)
            {
                if (lines[m][n].ToString() == "\"")
                {

                    string b = builder1.ToString();
                    Token token = new(b, TokenType.String, k + 1, i);
                    tokens.Add(token);
                    builder.Clear();
                    k = n;
                    i = m;
                    return;
                }
                else
                {
                    builder1.Append(lines[m][n]);
                }
            }
        }
        // aqui tengo que lanzar un error que si llegue al final y no encontre otra comilla
        // es pq el string se abrio y no se cerro
    }
}
