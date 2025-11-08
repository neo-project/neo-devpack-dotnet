using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirReturn : MirTerminator
{
    internal MirReturn(MirValue? value)
    {
        Value = value;
    }

    internal MirValue? Value { get; }
}
