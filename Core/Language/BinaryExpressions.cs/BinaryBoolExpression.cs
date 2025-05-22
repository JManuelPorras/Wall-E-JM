using Core.Enum;
using Core.Errors;
using Core.Interface;

namespace Core.Language;

public class BinaryBoolExpression : BinaryExpression<bool, bool>, IExpression<bool>, ICheckSemantic
{
    public BinaryBoolExpression(IExpression<bool> left, IExpression<bool> right, TokenType type) : base(left, right, type)
    {
    }

    public override bool Execute(Context context)
    {
        switch (Type)
        {
            case TokenType.Conjuncion:
                return Left.Execute(context) && Left.Execute(context);
            case TokenType.Disyuncion:
                return Left.Execute(context) || Right.Execute(context);

            default:
                throw new NotImplementedException();
        }
    }

}

