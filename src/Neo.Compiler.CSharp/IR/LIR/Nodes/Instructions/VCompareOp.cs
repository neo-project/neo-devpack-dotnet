using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal enum VCompareOp
{
    Eq,
    Ne,
    Lt,
    Le,
    Gt,
    Ge
}
