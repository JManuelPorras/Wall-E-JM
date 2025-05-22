using Core.Enum;

namespace Core.Model;

public class Token(string name, TokenType type, int col, int row)
{

    public string Name { get; set; } = name;
    public TokenType Type { get; set; } = type;
    public int Col { get; set; } = col;
    public int Row { get; set; } = row;
}