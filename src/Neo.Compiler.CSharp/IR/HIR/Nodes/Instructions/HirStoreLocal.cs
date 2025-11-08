using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirStoreLocal : HirInst
{
    public HirStoreLocal(HirLocal local, HirValue value)
        : base(HirType.VoidType)
    {
        Local = local ?? throw new ArgumentNullException(nameof(local));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override HirEffect Effect => HirEffect.None;
    public HirLocal Local { get; }
    public HirValue Value { get; set; }
}
