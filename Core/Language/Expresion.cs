namespace Core.Language;

public interface IInstruction
{
    void Execute();
}

public interface IExpression<T>
{
    T Execute();
}
