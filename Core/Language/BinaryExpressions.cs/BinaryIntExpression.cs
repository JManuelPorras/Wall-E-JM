using Core.Enum;
using Core.Interface;

namespace Core.Language;

public class BinaryIntExpression : BinaryExpression<int, int>, IExpression<int>
{
    public BinaryIntExpression(IExpression<int> left, IExpression<int> right, TokenType type) : base(left, right, type)
    {
    }

    public override int Execute(Context context)
    {
        switch (Type)
        {
            case TokenType.Suma:
                return Left.Execute(context) + Right.Execute(context);
            case TokenType.Multiplicacion:
                return Left.Execute(context) * Right.Execute(context);
            case TokenType.Resta:
                return Left.Execute(context) - Right.Execute(context);
            case TokenType.Division:
                return Left.Execute(context) / Right.Execute(context);
            case TokenType.Potencia:
                return (int)Math.Pow(Left.Execute(context), Right.Execute(context));
            case TokenType.Modulo:
                return Left.Execute(context) % Right.Execute(context);

            default:
                throw new NotImplementedException();
        }
    }
}

