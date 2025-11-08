using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirStructPack : MirInst
{
    internal MirStructPack(IReadOnlyList<MirValue> fields, MirStructType structType)
        : base(structType)
    {
        Fields = fields ?? throw new ArgumentNullException(nameof(fields));
    }

    internal IReadOnlyList<MirValue> Fields { get; }
}
