using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirConvert : MirInst
{
    internal enum Kind
    {
        SignExtend,
        ZeroExtend,
        Narrow,
        ToBool,
        ToByteString,
        ToBuffer
    }

    internal MirConvert(Kind conversionKind, MirValue value, MirType targetType)
        : base(targetType)
    {
        ConversionKind = conversionKind;
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    internal Kind ConversionKind { get; }
    internal MirValue Value { get; }
}
