using System.Collections.Generic;

namespace Neo.Compiler.MIR;

internal sealed record MirStructType(IReadOnlyList<MirType> Fields) : MirType;

