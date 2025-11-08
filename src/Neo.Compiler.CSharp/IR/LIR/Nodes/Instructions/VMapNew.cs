using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VMapNew : VNode
{
    internal VMapNew(LirType keyType, LirType valueType)
        : base(new LirMapType(keyType, valueType))
    {
        KeyType = keyType;
        ValueType = valueType;
    }

    internal LirType KeyType { get; }
    internal LirType ValueType { get; }
}
