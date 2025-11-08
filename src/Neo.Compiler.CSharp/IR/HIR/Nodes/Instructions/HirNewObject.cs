using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirNewObject : HirInst
{
    public HirNewObject(string typeName, IReadOnlyList<HirValue> arguments, HirStructType structType)
        : base(structType)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public string TypeName { get; }
    public IReadOnlyList<HirValue> Arguments { get; }
}
