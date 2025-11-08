namespace Neo.Compiler.HIR;

internal enum HirTypeKind : byte
{
    Unknown = 0,
    Void,
    Null,
    Bool,
    Int,
    ByteString,
    Buffer,
    Array,
    Struct,
    Map,
    Iterator,
    InteropHandle
}

