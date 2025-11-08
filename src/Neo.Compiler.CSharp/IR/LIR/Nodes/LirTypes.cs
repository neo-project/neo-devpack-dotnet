using System.Collections.Generic;

namespace Neo.Compiler.LIR;

/// <summary>
/// Low-level IR type tags used for instruction selection and emission. These are intentionally lightweight and only
/// capture information necessary to pick NeoVM opcodes and validate stack discipline.
/// </summary>
internal abstract record LirType
{
    internal static readonly LirVoidType TVoid = new();
    internal static readonly LirBoolType TBool = new();
    internal static readonly LirIntType TInt = new();
    internal static readonly LirByteStringType TByteString = new();
    internal static readonly LirBufferType TBuffer = new();
}
