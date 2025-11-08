using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirArrayGet : MirInst
{
    internal MirArrayGet(MirValue array, MirValue index, MirType elementType)
        : base(elementType)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
        Index = index ?? throw new ArgumentNullException(nameof(index));
    }

    internal MirValue Array { get; }
    internal MirValue Index { get; }
}
