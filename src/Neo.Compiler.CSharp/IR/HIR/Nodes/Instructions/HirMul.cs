using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirMul : HirBinaryInst { public HirMul(HirValue l, HirValue r, HirType t) : base(l, r, t) { } }
