namespace Neo.Compiler.HIR;

/// <summary>
/// SSA placeholder for sequencing side effects. Instructions that produce effects emit a memory token value so that
/// later passes can enforce ordering and batching.
/// </summary>
internal sealed class HirMemoryToken : HirValue
{
    private HirMemoryToken()
        : base(HirType.VoidType)
    {
    }

    public static HirMemoryToken Instance { get; } = new();
}

