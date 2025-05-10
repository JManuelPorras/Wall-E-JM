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
        {"=", TokenType.Igualdad},
        {"(", TokenType.ParentesisAbierto},
        {")", TokenType.ParentesisCerrado},
    };

    public static readonly Dictionary<string, TokenType> KeyWords = new()
    {
        {"+", TokenType.Suma},
        {"*", TokenType.Multiplicacion},
        {"/", TokenType.Division},
        {"=", TokenType.Igualdad},
        {"(", TokenType.ParentesisAbierto},
        {")", TokenType.ParentesisCerrado},
        {"True", TokenType.Bool},
        {"False", TokenType.Bool}
    };

    //esto tokeniza
    public static List<Token> Tokenizer(string[] lines)
    {

        List<Token> tokens = [];
        StringBuilder builder = new();

        for (int i = 0; i < lines.Length; i++)
        {
            for (int k = 0; k < lines[i].Length; k++)
            {
                string a = builder.ToString();

                string currentCharacter = lines[i][k].ToString();
                // if (currentCharacter == "\"")
                // {
                //     TextReader1(lines, ref i, ref k, tokens, ref builder);
                //     continue;

                // }
                if (k == lines[i].Length - 1)
                {
                    builder.Append(lines[i][k]);
                    a = builder.ToString();
                }
                if (a.Length == lines[i].Length)
                {
                    Token token = new(a, TokenType.Etiqueta, 0, i);
                    tokens.Add(token);
                    builder.Clear();
                    break;
                }
                if (int.TryParse(currentCharacter, out var result) && builder.Length == 0)
                {
                    NumReader(lines, i, ref k, tokens, result);
                    builder.Clear();
                    continue;
                }

                if (currentCharacter == "-" && builder.Length == 0)
                {
                    Token token = new("-", TokenType.Resta, k, i);
                    tokens.Add(token);
                    builder.Clear();
                    continue;
                }

                //si llego a un delimitador lo convierte en un token
                if (Delimiter.ContainsKey(currentCharacter) || currentCharacter == " " || k == lines[i].Length - 1)
                {

                    if (KeyWords.TryGetValue(a, out TokenType value))
                    {
                        Token token = new(a, value, k - a.Length, i);
                        tokens.Add(token);
                        builder.Clear();

                    }

                    else if (a != "")
                    {
                        Token token = new(a, TokenType.Identificador, k - a.Length, i);
                        tokens.Add(token);
                        builder.Clear();
                    }

                    if (KeyWords.TryGetValue(currentCharacter, out TokenType type))
                    {
                        Token token1 = new(currentCharacter, type, k, i);
                        tokens.Add(token1);
                    }

                    //esto es por si se me olvida poner un delimitador en las palabras clave,
                    //que no sean las comillas o el espacio
                    else if (k != lines[i].Length - 1 && currentCharacter != " ") Console.WriteLine("No esta puesto {0} en las palabras claves", currentCharacter);
                }

                //si no sigue acumulando caracteres
                else
                {
                    builder.Append(lines[i][k]);
                }

            }
        }
        return tokens;
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

    // private static void TextReader1(string[] lines, ref int i, ref int k, List<Token> tokens, ref StringBuilder builder)
    // {
    //     StringBuilder builder1 = new();

    //     //esto va hasta el final de la linea, si no ha encontrado otra comilla salta de linea
    //     for (int m = i; m < lines.Length; m++)
    //     {
    //         for (int n = m == i ? k + 1 : 0; n < lines[i].Length; n++)
    //         {
    //             if (lines[m][n].ToString() == "\"")
    //             {

    //                 string b = builder1.ToString();
    //                 Token token = new(b, TokenType.String, k + 1, i);
    //                 tokens.Add(token);
    //                 builder.Clear();
    //                 k = n;
    //                 i = m;
    //                 return;
    //             }
    //             else
    //             {
    //                 builder1.Append(lines[m][n]);
    //             }
    //         }
    //     }
    //     // aqui tengo que lanzar un error que si llegue al final y no encontre otra comilla
    //     // es pq el string se abrio y no se cerro
    // }
}
