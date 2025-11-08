using System.Collections.Generic;

namespace Neo.Compiler.HIR;

internal sealed record HirStructType(IReadOnlyList<HirField> Fields) : HirType;

