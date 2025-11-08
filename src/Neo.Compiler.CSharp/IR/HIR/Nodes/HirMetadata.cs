using System;
using System.Collections.Generic;

namespace Neo.Compiler.HIR;

internal sealed record HirSignature(
    IReadOnlyList<HirType> ParameterTypes,
    HirType ReturnType,
    IReadOnlyList<HirAttribute> Attributes);
