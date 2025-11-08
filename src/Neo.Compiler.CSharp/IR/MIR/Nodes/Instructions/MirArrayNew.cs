using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirArrayNew : MirInst
{
    internal MirArrayNew(MirValue length, MirType elementType)
        : base(new MirArrayType(elementType))
    {
        Length = length ?? throw new ArgumentNullException(nameof(length));
        ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
    }

    internal MirValue Length { get; }
    internal MirType ElementType { get; }
}
