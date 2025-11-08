using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

/// <summary>
/// Algebraic representation of HIR types. These types preserve high-level semantics (nullability, struct layout,
/// container element types) so that downstream passes can reason about constraints while still mapping cleanly to MIR.
/// </summary>
internal abstract record HirType
{
    public static readonly HirType UnknownType = new HirUnknownType();
    public static readonly HirType VoidType = new HirVoidType();
    public static readonly HirType NullType = new HirNullType();
    public static readonly HirType BoolType = new HirBoolType();
    public static readonly HirType IntType = new HirIntType();
    public static readonly HirType ByteStringType = new HirByteStringType();
    public static readonly HirType BufferType = new HirBufferType();

    public HirTypeKind Kind => this switch
    {
        HirUnknownType => HirTypeKind.Unknown,
        HirVoidType => HirTypeKind.Void,
        HirNullType => HirTypeKind.Null,
        HirBoolType => HirTypeKind.Bool,
        HirIntType => HirTypeKind.Int,
        HirByteStringType => HirTypeKind.ByteString,
        HirBufferType => HirTypeKind.Buffer,
        HirArrayType => HirTypeKind.Array,
        HirStructType => HirTypeKind.Struct,
        HirMapType => HirTypeKind.Map,
        HirIteratorType => HirTypeKind.Iterator,
        HirInteropHandleType => HirTypeKind.InteropHandle,
        _ => HirTypeKind.Unknown
    };
}
