namespace Core.Language;

public interface IInstruction
{
    void Execute(Context context);
}

public interface IExpression<T>
{
    T Execute(Context context);
}