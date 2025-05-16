namespace Core.Language;

public class BlockInstruction : IInstruction
{
    public List<IInstruction> Lines { get; }

    public BlockInstruction(List<IInstruction> lines)
    {
        Lines = lines;
    }

    void IInstruction.Execute(Context context)
    {
        foreach (var item in Lines)
        {
            item.Execute(context);
        }
    }
}
