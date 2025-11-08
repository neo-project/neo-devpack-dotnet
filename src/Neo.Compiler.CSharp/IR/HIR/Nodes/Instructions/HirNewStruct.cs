using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirNewStruct : HirInst
{
    public HirNewStruct(IReadOnlyList<HirValue> fields, HirStructType type)
        : base(type)
    {
        Fields = fields ?? throw new ArgumentNullException(nameof(fields));
    }

    public IReadOnlyList<HirValue> Fields { get; }
}
