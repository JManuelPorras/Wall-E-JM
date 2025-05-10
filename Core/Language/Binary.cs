namespace Core.Language;

public abstract class BinaryExpression<T, K> : IExpression<T>
{
    public BinaryExpression(IExpression<K> left, IExpression<K> right)
    {
        Left = left;
        Right = right;
    }

    public IExpression<K> Left { get; }
    public IExpression<K> Right { get; }

    public abstract T Execute();
}

public class Comparer<T> : BinaryExpression<bool, T>, IExpression<bool>
    where T : IComparable<T>
{
    public Comparer(IExpression<T> left, IExpression<T> right) : base(left, right) { }

    public override bool Execute()
    {
        return Left.Execute().CompareTo(Right.Execute()) > 0;
    }
}

public class Suma : BinaryExpression<int, int>, IExpression<int>
{
    public Suma(IExpression<int> left, IExpression<int> right) : base(left, right) { }

    public override int Execute()
    {
        return Left.Execute() + Right.Execute();
    }
}