using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirMapNew : HirInst
{
    public HirMapNew(HirType keyType, HirType valueType)
        : base(new HirMapType(keyType, valueType))
    {
        KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
    }

    public HirType KeyType { get; }
    public HirType ValueType { get; }
}
