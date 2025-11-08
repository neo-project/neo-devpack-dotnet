using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal enum HirConvKind
{
    SignExtend,
    ZeroExtend,
    Narrow,
    ToBool,
    ToByteString,
    ToBuffer
}
