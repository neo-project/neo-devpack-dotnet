using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirDiv : HirBinaryInst { public HirDiv(HirValue l, HirValue r, HirType t) : base(l, r, t) { } }
