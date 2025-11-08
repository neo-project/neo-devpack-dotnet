using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirArrayNew : HirInst
{
    public HirArrayNew(HirValue length, HirType elementType)
        : base(new HirArrayType(elementType))
    {
        Length = length ?? throw new ArgumentNullException(nameof(length));
        ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
    }

    public HirValue Length { get; }
    public HirType ElementType { get; }
}
