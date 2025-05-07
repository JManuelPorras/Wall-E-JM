using Core.Enum;

namespace Core.Model;

public class Token
{

    public string Name { get; set; }
    public TokenType Type { get; set; }
    public int Col { get; set; }
    public int Row { get; set; }

    public Token(string name, TokenType type, int col, int row)
    {
        Name = name;
        Type = type;
        Col = col;
        Row = row;
    }
}