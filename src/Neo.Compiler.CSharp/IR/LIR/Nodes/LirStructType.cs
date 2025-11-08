using System.Collections.Generic;

namespace Neo.Compiler.LIR;

internal sealed record LirStructType(IReadOnlyList<LirType> Fields) : LirType;

