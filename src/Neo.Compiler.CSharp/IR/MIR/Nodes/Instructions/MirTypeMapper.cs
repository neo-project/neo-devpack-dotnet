using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal static class MirTypeMapper
{
    internal static MirType FromHirType(HirType type)
    {
        return type switch
        {
            HirBoolType => MirType.TBool,
            HirNullType => MirType.TUnknown,
            HirVoidType => MirType.TVoid,
            HirByteStringType => MirType.TByteString,
            HirBufferType => MirType.TBuffer,
            HirUnknownType => MirType.TUnknown,
            HirIntType intType => new MirIntType(intType.WidthHintBits, intType.IsSigned),
            HirArrayType array => new MirArrayType(FromHirType(array.ElementType)),
            HirStructType structType => new MirStructType(structType.Fields.Select(f => FromHirType(f.Type)).ToArray()),
            HirMapType mapType => new MirMapType(FromHirType(mapType.KeyType), FromHirType(mapType.ValueType)),
            HirIteratorType iterator => new MirHandleType(iterator.ElementType is null ? "Iterator" : $"Iterator<{iterator.ElementType}>"),
            HirInteropHandleType handle => new MirHandleType(handle.Name),
            _ => MirType.TUnknown
        };
    }
}
