using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirTokenSeed : MirInst
{
    internal MirTokenSeed()
        : base(MirType.TToken)
    {
        Token = ProduceMemoryToken();
    }

    internal MirMemoryTokenValue Token { get; }
}
