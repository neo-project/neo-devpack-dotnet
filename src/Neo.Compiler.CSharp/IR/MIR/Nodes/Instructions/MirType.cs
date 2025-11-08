using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal abstract record MirType
{
    internal static readonly MirUnknownType TUnknown = new();
    internal static readonly MirVoidType TVoid = new();
    internal static readonly MirBoolType TBool = new();
    internal static readonly MirIntType TInt = new();
    internal static readonly MirByteStringType TByteString = new();
    internal static readonly MirBufferType TBuffer = new();
    internal static readonly MirTokenType TToken = new();
}
