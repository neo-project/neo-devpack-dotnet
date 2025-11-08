using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal readonly record struct HirIntrinsicMetadata(
    string Category,
    string Name,
    HirEffect Effect,
    HirType ReturnType,
    IReadOnlyList<HirType> ParameterTypes,
    bool IsPure,
    bool IsDeterministic,
    bool RequiresMemoryToken,
    int? GasCostHint);
