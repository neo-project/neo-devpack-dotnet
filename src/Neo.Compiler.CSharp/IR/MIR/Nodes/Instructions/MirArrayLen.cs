using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirArrayLen : MirInst
{
    internal MirArrayLen(MirValue array)
        : base(MirType.TInt)
    {
        Array = array ?? throw new ArgumentNullException(nameof(array));
    }

    internal MirValue Array { get; }
}
