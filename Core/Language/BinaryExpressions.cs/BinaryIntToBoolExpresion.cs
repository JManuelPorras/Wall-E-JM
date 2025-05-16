using Core.Enum;

namespace Core.Language;

public class BinaryIntToBoolExpresion : BinaryExpression<bool, int>, IExpression<bool>
{
    public BinaryIntToBoolExpresion(IExpression<int> left, IExpression<int> right, TokenType type) : base(left, right, type)
    {
    }

    public override bool Execute(Context context)
    {
        switch (Type)
        {
            case TokenType.Igualdad:
                return Left.Execute(context) == Left.Execute(context);
            case TokenType.MenorQue:
                return Left.Execute(context) < Right.Execute(context);
            case TokenType.MayorQue:
                return Left.Execute(context) > Right.Execute(context);
            case TokenType.MayorOIgual:
                return Left.Execute(context) >= Right.Execute(context);
            case TokenType.MenorOIgual:
                return Left.Execute(context) <= Right.Execute(context);

            default:
                throw new NotImplementedException();
        }
    }
}

