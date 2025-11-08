using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirAdd : HirBinaryInst { public HirAdd(HirValue l, HirValue r, HirType t) : base(l, r, t) { } }
