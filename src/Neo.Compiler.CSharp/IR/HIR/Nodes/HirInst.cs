namespace Neo.Compiler.HIR;

internal abstract class HirInst : HirValue
{
    protected HirInst(HirType type)
        : base(type)
    {
    }

    public virtual HirEffect Effect => HirEffect.None;
    public bool ConsumesMemoryToken { get; protected set; }
    public bool ProducesMemoryToken { get; protected set; }
}

