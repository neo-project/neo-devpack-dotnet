using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirMapNew : MirInst
{
    internal MirMapNew(MirType keyType, MirType valueType)
        : base(new MirMapType(keyType, valueType))
    {
        KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
    }

    internal MirType KeyType { get; }
    internal MirType ValueType { get; }
}
