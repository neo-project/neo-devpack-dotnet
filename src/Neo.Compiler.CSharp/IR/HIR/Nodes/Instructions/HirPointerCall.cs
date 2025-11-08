using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirPointerCall : HirInst
{
    public HirPointerCall(HirValue? pointer, IReadOnlyList<HirValue> arguments, HirType returnType, HirCallSemantics semantics = HirCallSemantics.Effectful, bool isTailCall = false, byte? callTableIndex = null)
        : base(returnType)
    {
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        Semantics = semantics;
        IsTailCall = isTailCall;
        CallTableIndex = callTableIndex;
        Pointer = pointer;

        if (Pointer is null && CallTableIndex is null)
            throw new ArgumentException("Pointer calls require either a pointer operand or a call-table index.", nameof(pointer));

        if (semantics != HirCallSemantics.Pure)
        {
            ConsumesMemoryToken = true;
            ProducesMemoryToken = true;
        }
    }

    public HirValue? Pointer { get; }
    public IReadOnlyList<HirValue> Arguments { get; }
    public HirCallSemantics Semantics { get; }
    public bool IsTailCall { get; }
    public byte? CallTableIndex { get; }

    public override HirEffect Effect => Semantics switch
    {
        HirCallSemantics.Pure => HirEffect.None,
        HirCallSemantics.ReadOnly => HirEffect.StorageRead,
        _ => HirEffect.Memory
    };
}
