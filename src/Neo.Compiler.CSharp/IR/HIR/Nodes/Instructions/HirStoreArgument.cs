using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirStoreArgument : HirInst
{
    public HirStoreArgument(HirArgument argument, HirValue value)
        : base(HirType.VoidType)
    {
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public override HirEffect Effect => HirEffect.Memory;
    public HirArgument Argument { get; }
    public HirValue Value { get; set; }
}
