namespace Core.Language;

public class InstructionFunc : IInstruction
{
    public InstructionFunc(string name, params object[] @params)
    {
        Name = name;
        Params = @params;
    }

    public string Name { get; }
    public object[] Params { get; }

    //tengo que implementar esto todavia
    public void Execute(Context context) => throw new NotImplementedException();
}