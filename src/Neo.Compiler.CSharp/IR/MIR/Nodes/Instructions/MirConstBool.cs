using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirConstBool : MirInst
{
    internal MirConstBool(bool value)
        : base(MirType.TBool)
    {
        Value = value;
    }

    internal bool Value { get; }
}
