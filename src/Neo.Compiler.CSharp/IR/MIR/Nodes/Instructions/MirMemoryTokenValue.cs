using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirMemoryTokenValue : MirValue
{
    internal MirMemoryTokenValue()
        : base(MirType.TToken)
    {
    }
}
